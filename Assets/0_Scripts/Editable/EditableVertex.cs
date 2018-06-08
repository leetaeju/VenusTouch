﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableVertex : EditableThing
{
	private EditableObject parent;
	public List<int> indices;

	public void Initialize(EditableObject parent, List<int> indices) {
		this.parent = parent;
		this.indices = indices;

		transform.parent = parent.transform;

		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.enabled = false;
	}

	public void ReportUpdate(Vector3 deltaPosition) {
		parent.UpdateVertex(this, indices, deltaPosition);
		transform.position += deltaPosition;
	}

	public override void UpdatePosition(Vector3 newPosition)
	{
		ReportUpdate(newPosition - transform.position);
	}
}