using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class NoteController : MonoBehaviour {
	private int s=15;
    
	public ProgressBarBehaviour progressBarBehaviour;

	public CanvasGroup sheet1;
	public CanvasGroup sheet2;

	public DrumNote firstNote;

	private DrumNote currentNote;

	void Start () {
		currentNote = firstNote;
	}

	public void ProgressUpdate(int drumIndex)
	{
		if (currentNote && currentNote.drumIndex == drumIndex) {
			if (s >= 100)
				SwitchSheets ();
			else
				s = s + 6;

			progressBarBehaviour.SetFillerSizeAsPercentage (s);
			currentNote = currentNote.nextNote;
			if (currentNote && currentNote.transform.parent != firstNote.transform.parent)
				SwitchSheets ();
		}
	}

	void SwitchSheets() {
		sheet1.alpha = 0;
		sheet2.alpha = 1;
		s = 15;
	}
		
}
