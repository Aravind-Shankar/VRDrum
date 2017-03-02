using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressBar;

public class RKController : NoteController {

	public AudioSource source;
	public float bpm;
	public float initSheetBeatOffset;
	public float beatEpsilonPercent;

	private float fillerPercent;
	private int totalBeats;
	private float secondsPerBeat;
	private float beatEpsilon;
	private float initSheetTimeOffset;
	private float sheetTimeOffset;
	private int numBeatsDone;
	private int deltaBeats;
	private float beatDeltaTime;

	void Start () {
		ComputeParameters ();

		numBeatsDone = deltaBeats = numSheetsDone = numNotesCorrect = numNotesPlayed = 0;
		sheetTimeOffset = initSheetTimeOffset;
		currentNote = firstNote;

		source.Play ();
		UpdateUI ();

		// use InvokeRepeating() as Update() is too slow for normal BPMs (WWRY is 162bpm)
		InvokeRepeating ("BeatUpdate",
			(initSheetTimeOffset > 0) ? initSheetTimeOffset : 0,
			secondsPerBeat);
	}
	
	void Update () {
		fillerPercent = 100f * (source.time - sheetTimeOffset) * totalSheets / source.clip.length;
		if (fillerPercent >= 100)
			SwitchSheets ();
		progressBar.SetFillerSizeAsPercentage (fillerPercent);
	}

	void BeatUpdate() {
		if (currentNote && numBeatsDone < totalBeats) {
			if (deltaBeats == 0)
				drumController.Hit (currentNote.autoDrumIndex, 1, false);
			
			++numBeatsDone;
			++deltaBeats;
			beatDeltaTime = -Time.realtimeSinceStartup;

			if (currentNote.pauseTime == deltaBeats - 1) {
				deltaBeats = 0;
				currentNote.UndoHighlight ();
				currentNote = currentNote.nextNote;
				UpdateUI ();
			}
		} else
			CancelInvoke ("BeatUpdate");
	}

	public override void ProgressUpdate (int drumIndex)
	{
		if (currentNote) {
			++numNotesPlayed;
			if (currentNote.drumIndex == drumIndex) {
				beatDeltaTime += Time.realtimeSinceStartup;
				if (beatDeltaTime <= beatEpsilon) {
					++numNotesCorrect;
				}
			} else if (currentNote.nextNote &&
			         currentNote.nextNote.drumIndex == drumIndex) {
				beatDeltaTime += Time.realtimeSinceStartup;
				if (beatDeltaTime >= (secondsPerBeat - beatEpsilon)) {
					++numNotesCorrect;
				}
			}
			currentNote.UndoHighlight ();
			deltaBeats -= (1 + currentNote.pauseTime);
			currentNote = currentNote.nextNote;
			UpdateUI ();
		}
	}

	protected override void ComputeParameters ()
	{
		totalSheets = totalNotes = 0;
		totalBeats = (int) (source.clip.length * bpm / 60f);
		secondsPerBeat = (float) (source.clip.length) / totalBeats;
		initSheetTimeOffset = initSheetBeatOffset * secondsPerBeat;
		beatEpsilon = secondsPerBeat * beatEpsilonPercent / 100;

		currentNote = firstNote;
		numBeatsDone = totalBeats;
		DrumNote prevNote = null;
		while (numBeatsDone > 0) {
			++totalNotes;
			if (!prevNote ||
				currentNote.isStartOfLoop ||
				currentNote.transform.parent != prevNote.transform.parent)
				++totalSheets;

			numBeatsDone -= (1 + currentNote.pauseTime);
			prevNote = currentNote;
			currentNote = currentNote.nextNote;
		}
	}

	protected override void SwitchSheets ()
	{
		++numSheetsDone;
		if (numSheetsDone < totalSheets) {
			fillerPercent = 0;
			sheetTimeOffset = initSheetTimeOffset + source.time;
			if (numSheetsDone == 1) {
				sheet1.alpha = 0;
				sheet2.alpha = 1;
			}
		} else {
			finishNotice.enabled = true;
			this.enabled = false;
			CancelInvoke ("BeatUpdate");
		}
		UpdateUI ();
	}

	protected override void UpdateUI ()
	{
		correctNotesText.text = string.Format("Beats: {0}/{1}, notes: {2}/{3}",
			numBeatsDone, totalBeats, numNotesCorrect, numNotesPlayed);
		sheetText.text = string.Format ("Sheets {0}/{1}", numSheetsDone, totalSheets);
		if (currentNote) {
			currentNote.Highlight (highlightColor);
		}
	}
}
