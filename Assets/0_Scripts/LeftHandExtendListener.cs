using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeftHandExtendListener : MonoBehaviour {
	[SerializeField]
	private UnityEvent showUI;

	[SerializeField]
	private UnityEvent hideUI;

	private bool isHandOpen = false;
	private bool isFacing = false;

	public void ChangeHandState(bool open) {
		isHandOpen = open;
		CheckUIState();
	}

	public void ChangeFacingCamera(bool facing) {
		isFacing = facing;
		CheckUIState();
	}

	private void CheckUIState() {
		if (isHandOpen && isFacing) {
			showUI.Invoke();
		}
		else {
			try {
				hideUI.Invoke();
			}
			catch (Exception) {

			}
		}
	}
}