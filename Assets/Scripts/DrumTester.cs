using UnityEngine;
using System.Collections;

public class DrumTester : MonoBehaviour {

	public bool isRandomTest = true;
	public float repeatTimeSeconds = 2.0f;
	public KeyCode[] drumKeys;

	private DrumController drumController;

	// Use this for initialization
	void Start () {
		drumController = GetComponent<DrumController> ();

		if (isRandomTest)
			StartCoroutine (RandomHit ());
	}

	void Update() {
		if (!isRandomTest) {
			for (int i = 0; i < drumKeys.Length; ++i)
				if (Input.GetKeyDown(drumKeys[i]))
					drumController.Hit(i, 1, true);
		}
	}
	
	IEnumerator RandomHit() {
		while (true) {
			yield return new WaitForSeconds (repeatTimeSeconds);
			int partIndex = Random.Range (0, drumController.allActiveParts.Length);
			int force = Random.Range (1, DrumController.MAX_FORCE + 1);
			drumController.Hit (partIndex, force, true);
			Debug.LogFormat ("Part index: {0}\tForce: {1}", partIndex, force);
		}
	}
}
