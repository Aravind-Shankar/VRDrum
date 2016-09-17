using UnityEngine;
using System.Collections;

public class DrumTester : MonoBehaviour {

	public float repeatTimeSeconds = 2.0f;

	private DrumController drumController;

	// Use this for initialization
	void Start () {
		drumController = GetComponent<DrumController> ();
		StartCoroutine (RandomHit ());
	}
	
	IEnumerator RandomHit() {
		while (true) {
			yield return new WaitForSeconds (repeatTimeSeconds);
			int partIndex = Random.Range (0, drumController.allActiveParts.Length);
			int force = Random.Range (1, DrumController.MAX_FORCE + 1);
			drumController.Hit (partIndex, force);
			Debug.LogFormat ("Part index: {0}\tForce: {1}", partIndex, force);
		}
	}
}
