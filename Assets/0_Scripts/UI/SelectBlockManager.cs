using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBlockManager : MonoBehaviour {
	[SerializeField]
	private Transform blockPanel;
	[SerializeField]
	private Transform[] blockButtons;

	[SerializeField]
	private Vector3 panelVisibleScale;
	[SerializeField]
	private Vector3 buttonVisibleScale;

	private int transitionFrame = 10;
	private bool isShowing = true;
	private bool isTransitioning = false;

	public void OnClick() {
		if (isTransitioning) return;
		isShowing = !isShowing;

		StartCoroutine("StartTransitionCo");
	}

	private int doneCount;
	private IEnumerator StartTransitionCo() {
		isTransitioning = true;
		doneCount = 0;
		Vector3 origin = isShowing ? Vector3.zero : panelVisibleScale;
		Vector3 target = isShowing ? panelVisibleScale : Vector3.zero;

		for (int f = 1; f <= transitionFrame; f++) {
			blockPanel.localScale = Vector3.Lerp(origin, target, (float)f / transitionFrame);
			if ((f >= 7)) {
				StartCoroutine("ButtonTransitionCo", f - 7);
			}
			yield return null;
		}
		while (doneCount != 4) {
			yield return null;
		}
		isTransitioning = false;
	}

	private IEnumerator ButtonTransitionCo(int i) {
		Vector3 origin = isShowing ? Vector3.zero : buttonVisibleScale;
		Vector3 target = isShowing ? buttonVisibleScale : Vector3.zero;

		for (int f = 1; f <= transitionFrame; f++) {
			blockButtons[i].localScale = Vector3.Lerp(origin, target, (float)f / transitionFrame);
			yield return null;
		}
		doneCount++;
	}
}