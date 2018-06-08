using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity.Interaction;

public class ColorSlider : MonoBehaviour {
	public int colorIndex;

	[SerializeField]
	private MeshRenderer backPanel;
	private Material backPanelMaterial;

	private void Awake() {
		backPanelMaterial = backPanel.material;
	}

	public void ValueChanged(InteractionSlider slider) {
		Color newColor = new Color();
		newColor[colorIndex] = slider.HorizontalSliderValue;
		backPanelMaterial.color = newColor;

		ColorManager.Instance.UpdateColor(colorIndex, slider.HorizontalSliderValue);
	}
}
