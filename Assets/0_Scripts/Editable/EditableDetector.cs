using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Definitions;

public class EditableDetector : MonoBehaviour {
	private float colliderRadius = 0.09f;
	LayerMask editableMask;
	public bool interalActive;

	void Awake()
	{
		colliderRadius = GetComponent<SphereCollider>().radius;
		editableMask = Values.EditableLayerMask;
		interalActive = false;
	}

	public void CheckActive()
	{
		gameObject.SetActive(interalActive);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Editable"))
		{
			other.GetComponent<EditableThing>().EnterVisibleRange();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Editable"))
		{
			other.GetComponent<EditableThing>().ExitVisibleRange();
		}
	}

	public void ExitAllColliders()
	{
		Collider[] cols = Physics.OverlapSphere(transform.position, colliderRadius, editableMask);
		foreach (var col in cols)
		{
			col.GetComponent<EditableThing>().ExitVisibleRange();
		}
	}
}
