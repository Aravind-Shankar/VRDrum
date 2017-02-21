using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class RKController : MonoBehaviour {

	public ProgressBarBehaviour progressBarBehaviour;

	public AudioSource source;

	// Use this for initialization
	void Start () {
		source.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		progressBarBehaviour.SetFillerSizeAsPercentage (100 * source.timeSamples / source.clip.samples);
	}
}
