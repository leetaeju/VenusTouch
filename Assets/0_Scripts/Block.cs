using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	private static Transform parent = null;

	private int handOverCount = 0;

	private Renderer _renderer;

	private void Awake() {
		if (parent == null) {
			parent = GameObject.Find("MeshParent").transform;
		}
		_renderer = transform.GetChild(0).GetComponent<Renderer>();
	}

	public void HandEnter() {
		if (++handOverCount == 1) {
			SetTransparent(true);
		}
	}

	public void HandExit() {
		if (--handOverCount == 0) {
			SetTransparent(false);
		}
	}

	private void SetTransparent(bool transparent) {
		Color color = _renderer.material.color;
		color.a = transparent ? 0.5f : 1f;
		_renderer.material.color = color;
	}

	public void SetMateiral(Material mat) {
		_renderer.material = mat;
	}

	public void PlaceBlock() {
		GetComponentInChildren<Collider>().enabled = true;
		transform.parent = parent;
	}

	public void SetMaterialColor(Color color) {
		_renderer.material.color = color;
	}
}