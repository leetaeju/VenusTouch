using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonWorker : MonoBehaviour {
	[SerializeField]
	private UnityEvent onSelect;

	[SerializeField]
	private UnityEvent onDeselect;

	[SerializeField]
	private Material normalMaterial;

	[SerializeField]
	private Material selectedMaterial;

	private Renderer _renderer;

	private void Awake() {
		_renderer = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
	}

	public void Select(bool doEvent = true) {
		_renderer.material = selectedMaterial;
		if (doEvent) onSelect.Invoke();
	}

	public void Deselect() {
		_renderer.material = normalMaterial;
		onDeselect.Invoke();
	}
}