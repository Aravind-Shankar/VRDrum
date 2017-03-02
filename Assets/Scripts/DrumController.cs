using UnityEngine;
using System.Collections;

public class DrumController : MonoBehaviour {

	public const int MAX_FORCE = 3;

	public float volumeScaleMultiplier = 1.0f;
	public NoteController noteController;
	public float hitUndoDelay = 0.5f;
	public float highlightUndoDelay = 1.5f;

	public DrumPart[] allActiveParts;

	void Start() {
		foreach (DrumPart activePart in allActiveParts) {
			activePart.hitLight = activePart.part.GetComponent<Light> ();
			activePart.defaultLightColor = activePart.hitLight.color;
			activePart.hitLight.enabled = false;

			activePart.coloringMaterial = activePart.coloringPart.GetComponent<Renderer> ().material;
			activePart.defaultMaterialColor = activePart.coloringMaterial.color;

			activePart.hitSoundSource = activePart.part.GetComponent<AudioSource> ();
		}
	}

	public void Highlight(int partIndex) {
		if (partIndex >= 0 && partIndex < allActiveParts.Length) {
			DrumPart hitPart = allActiveParts [partIndex];
			if (hitPart.hitLight != null) {
				StopCoroutine (DelayAndUndoHit(hitPart, highlightUndoDelay, true));
				hitPart.hitLight.enabled = true;
				hitPart.coloringMaterial.color = hitPart.hitLight.color = noteController.highlightColor;
				StartCoroutine (DelayAndUndoHit(hitPart, highlightUndoDelay, true));
			}
		}
	}

	public void Hit(int partIndex, int force) {
		Hit (partIndex, force, true);
	}

	public void Hit(int partIndex, int force, bool highlight) {
		if (	
			(partIndex >= 0 && partIndex < allActiveParts.Length) &&
			(force >= 0 && force <= MAX_FORCE)
			)
		{
			DrumPart hitPart = allActiveParts [partIndex];
			if (hitPart.sound && hitPart.hitSoundSource)
				hitPart.hitSoundSource.PlayOneShot (
					hitPart.sound, hitPart.baseVolume * (force > 0 ? force : 1) * volumeScaleMultiplier
				);

			if (highlight && hitPart.hitLight) {
				StopCoroutine (DelayAndUndoHit(hitPart, hitUndoDelay, false));
				hitPart.hitLight.enabled = true;
				hitPart.coloringMaterial.color = hitPart.hitLight.color;
				StartCoroutine (DelayAndUndoHit(hitPart, hitUndoDelay, false));
			}
			if (noteController)
				noteController.ProgressUpdate (partIndex);
		}
	}

	IEnumerator DelayAndUndoHit(DrumPart part, float delay, bool resetLightAlso) {
		yield return new WaitForSeconds (delay);
		part.hitLight.enabled = false;
		if (resetLightAlso) {
			part.hitLight.color = part.defaultLightColor;
		}
		part.coloringMaterial.color = part.defaultMaterialColor;
	}

	[System.Serializable]
	public class DrumPart {
		public GameObject part;
		public GameObject coloringPart;
		public AudioClip sound;
		public float baseVolume = 1.0f;

		[HideInInspector]
		public Light hitLight;
		[HideInInspector]
		public Color defaultLightColor;
		[HideInInspector]
		public AudioSource hitSoundSource;
		[HideInInspector]
		public Material coloringMaterial;
		[HideInInspector]
		public Color defaultMaterialColor;
	}
}
