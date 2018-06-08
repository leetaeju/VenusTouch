using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour {
	[SerializeField]
	private ButtonWorker[] buttons;
	private int selectedIndex = 0;

	private void Start() {
		buttons[0].Select(false);
	}

	public void ClickButton(int index) {
		if (index == selectedIndex) return;

		buttons[selectedIndex].Deselect();
		buttons[index].Select();
		selectedIndex = index;
	}
}