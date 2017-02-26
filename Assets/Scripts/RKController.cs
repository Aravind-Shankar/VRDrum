using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class RKController : NoteController {

	public AudioSource source;

	// Use this for initialization
	void Start () {
		source.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		progressBar.SetFillerSizeAsPercentage (100 * source.timeSamples / source.clip.samples);
	}

	public override void ProgressUpdate (int drumIndex)
	{
		
	}

	protected override void UpdateUI ()
	{
		base.UpdateUI ();
	}
}
