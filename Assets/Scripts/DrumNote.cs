using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class DrumNote : MonoBehaviour {

	public int drumIndex;
	public DrumNote nextNote;
	public Vector3 highlightScale;

	private RectTransform rect;
	private Image noteImage;

	private Vector3 initScale;
	private Color initImageColor;

	void Start() {
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
