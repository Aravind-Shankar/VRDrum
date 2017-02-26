using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressBar;

public class NoteController : MonoBehaviour {

	public ProgressBarBehaviour progressBar;
	public DrumController drumController;

	public CanvasGroup sheet1;
	public CanvasGroup sheet2;

	public DrumNote firstNote;

	public Text sheetText;
	public Text correctNotesText;
	public Text finishNotice;

	public Color highlightColor;

	protected int s = 15;
	protected int totalNotes;
	protected int totalSheets;
	protected DrumNote currentNote;
	protected int numSheetsDone;
	protected int numNotesPlayed;
	protected int numNotesCorrect;

	void Start () {
		ComputeNotesAndSheets ();

		numSheetsDone = numNotesPlayed = numNotesCorrect = 0;
		currentNote = firstNote;
		UpdateUI ();
	}

	public virtual void ProgressUpdate(int drumIndex)
	{
		if (currentNote) {
			++numNotesPlayed;
			if (currentNote.drumIndex == drumIndex) {
				currentNote.UndoHighlight ();
				++numNotesCorrect;
				if (s >= 100)
					SwitchSheets ();
				else
					s = s + 6;

				progressBar.SetFillerSizeAsPercentage (s);

				currentNote = currentNote.nextNote;
			}
			UpdateUI ();
		}
	}

	void ComputeNotesAndSheets() {
		totalNotes = totalSheets = 0;

		currentNote = firstNote;
		DrumNote prevNote = null;
		while (currentNote) {
			++totalNotes;
			if (!prevNote || currentNote.transform.parent != prevNote.transform.parent)
				++totalSheets;
			
			prevNote = currentNote;
			currentNote = currentNote.nextNote;
		}
	}

	void SwitchSheets() {
		++numSheetsDone;
		if (numSheetsDone < totalSheets) {
			sheet1.alpha = 0;
			sheet2.alpha = 1;
			s = 15;
		} else {
			finishNotice.enabled = true;
		}
		UpdateUI ();
	}

	protected virtual void UpdateUI() {
		sheetText.text = string.Format ("Sheets {0}/{1}", numSheetsDone, totalSheets);
		correctNotesText.text = string.Format ("Notes: {0} correct of {1} played; {2} total",
											numNotesCorrect, numNotesPlayed, totalNotes);
		if (currentNote) {
			currentNote.Highlight (highlightColor);
			if (drumController)
				drumController.Highlight (currentNote.drumIndex);
		}
	}
		
}
