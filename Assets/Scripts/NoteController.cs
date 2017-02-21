using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class NoteController : MonoBehaviour {
	private int s=15;
    
	public ProgressBarBehaviour progressBarBehaviour;

	public AudioSource source;

	private int totalSamples;

	public CanvasGroup sheet1;
	public CanvasGroup sheet2;

	public DrumNote firstNote;
	private DrumNote currentNote;


	public void ProgressUpdate(int drumIndex)
	{
		if (currentNote.drumIndex == drumIndex) {
			if (s >= 100) {
				sheet1.alpha = 0;
				sheet2.alpha = 1;
				s = 15;
			} else
				s = s + 6;

			progressBarBehaviour.SetFillerSizeAsPercentage (s);
			currentNote = firstNote.nextNote;
		}


	}

	// Use this for initialization
	void Start () {
		//source.Play ();
		currentNote = firstNote;
	}

	// Update is called once per frame
	void Update () {
		
//			if (Input.GetKeyDown ("space")) 
//			{
//			
//			progressBarBehaviour.SetFillerSizeAsPercentage (100 * source.timeSamples / source.clip.samples);
//			progressBarBehaviour.SetFillerSizeAsPercentage (s);
//			s = s + 6;
//			}

	}


}
