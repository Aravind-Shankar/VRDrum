using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class DrumNote : MonoBehaviour {

	public int drumIndex;
	public DrumNote nextNote;
	public int pauseTime;
	public bool isStartOfLoop;
	public Vector3 highlightScale;
	public int autoDrumIndex = -1;

	private RectTransform rect;
	private Image noteImage;

	private Vector3 initScale;
	private Color initImageColor;

	void Awake() {
		rect = GetComponent<RectTransform> ();
		initScale = rect.localScale;
		noteImage = GetComponent<Image> ();
		initImageColor = noteImage.color;
	}

	public void Highlight(Color color) {
		rect.localScale = highlightScale;
		noteImage.color = color;
	}

	public void UndoHighlight() {
		rect.localScale = initScale;
		noteImage.color = initImageColor;
	}

}
