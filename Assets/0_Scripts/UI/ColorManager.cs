using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {
	public Color color;

	public MeshRenderer colorShower;
	private Material colorShowerMat;

	[SerializeField]
	private Transform colorPanel;

	private int transitionFrame = 16;
	private bool isShowing = false;
	private bool isTransitioning = false;

	public InteractionSlider[] sliders;

	public static ColorManager Instance { private set; get; }

	private void Awake() {
		Instance = this;
		color.a = 1f;
		colorShowerMat = colorShower.material;

		transform.Rotate(0, -180f, 0);
		foreach (var slider in sliders) {
			slider.enabled = false;
		}
	}

	public void OnClick() {
		if (isTransitioning) return;
		isShowing = !isShowing;

		StartCoroutine("StartTransitionCo");

		foreach (var slider in sliders) {
			slider.enabled = isShowing;
		}
	}

	private IEnumerator StartTransitionCo() {
		isTransitioning = true;
		float target = isShowing ? -180f : 180f;

		for (int f = 1; f <= transitionFrame; f++) {
			colorPanel.Rotate(0, target / transitionFrame, 0);
			yield return null;
		}
		isTransitioning = false;
	}

	public void UpdateColor(int index, float value) {
		color[index] = value;
		colorShowerMat.color = color;
		PinchHandler_Place.Instance.placedColor = color;
	}
}