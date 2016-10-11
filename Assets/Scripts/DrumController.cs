using UnityEngine;
using System.Collections;

public class DrumController : MonoBehaviour {

	public const int MAX_FORCE = 3;

	public float volumeScaleMultiplier = 1.0f;
	public DrumPart[] allActiveParts;

	void Start() {
		for (int i = 0; i < allActiveParts.Length; ++i) {
			allActiveParts [i].hitLight = allActiveParts [i].part.GetComponent<Light> ();
			allActiveParts [i].hitLight.enabled = false;

			allActiveParts [i].coloringMaterial = allActiveParts [i].coloringPart.GetComponent<Renderer> ().material;
			allActiveParts [i].defaultColor = allActiveParts [i].coloringMaterial.color;

			allActiveParts [i].hitSoundSource = allActiveParts [i].part.GetComponent<AudioSource> ();
		}
	}

	public void Hit(int partIndex, int force) {
		if (
			(partIndex >= 0 && partIndex < allActiveParts.Length) &&
			(force > 0 && force <= MAX_FORCE)
			)
		{
			DrumPart hitPart = allActiveParts [partIndex];
			if (hitPart.sound != null && hitPart.hitSoundSource != null)
				hitPart.hitSoundSource.PlayOneShot (
					hitPart.sound, hitPart.baseVolume * force * volumeScaleMultiplier
				);

			if (hitPart.hitLight != null) {
				StopCoroutine (DelayAndUndoHit(hitPart));
				hitPart.hitLight.enabled = true;
				hitPart.coloringMaterial.color = hitPart.hitLight.color;
				StartCoroutine (DelayAndUndoHit(hitPart));
			}
		}
	}

	IEnumerator DelayAndUndoHit(DrumPart part) {
		yield return new WaitForSeconds (0.5f);
		part.hitLight.enabled = false;
		part.coloringMaterial.color = part.defaultColor;
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
		public AudioSource hitSoundSource;
		[HideInInspector]
		public Material coloringMaterial;
		[HideInInspector]
		public Color defaultColor;
	}
}
