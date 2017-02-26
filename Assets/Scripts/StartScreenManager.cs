using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour {

	public GameObject mainCanvas;
	public GameObject vrEntryCanvas;

	public BTReceiver receiver;

	void Start() {
		receiver.enabled = false;
	}

	public void SetupForVR(string methodName) {
		mainCanvas.SetActive (false);
		vrEntryCanvas.SetActive(true);

		receiver.connectionStateText = vrEntryCanvas.transform.Find ("Connection State").GetComponent<Text>();
		receiver.OnBTModuleConnected.AddListener(() => {
			SendMessage(methodName);
		});
		receiver.enabled = true;
	}

	public void StartFreestyle() {
		SceneManager.LoadScene ("Freestyle");
	}

	public void StartReverseKaraoke() {
		SceneManager.LoadScene ("ReverseKaraoke");
	}

	public void QuitApp() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit ();
	}

}
