﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableLine : EditableThing, IUpdatable {
	private EditableObject parent;
	public EditableVertex point0, point1;

	public List<int> tIndices = new List<int>(4);

	public void Initialize(EditableObject parent, EditableVertex point0, EditableVertex point1, int tIndex) {
		this.parent = parent;
		this.point0 = point0;
		this.point1 = point1;

		tIndices.Add(tIndex);

		transform.parent = parent.transform;
		CalculateTransform();

		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.enabled = false;
	}

	public void UpdateChild() {
		CalculateTransform();
	}

	private void CalculateTransform() {
		transform.position = (point0.transform.position + point1.transform.position) / 2f;
		Vector3 goalUp = point0.transform.position - point1.transform.position;

		transform.up = goalUp;
		//transform.rotation = Quaternion.LookRotation(goalUp, -transform.forward);
		//transform.Rotate(Vector3.right, 90f);

		Vector3 scale = transform.localScale;
		scale.y = Vector3.Distance(point0.transform.position, point1.transform.position) / 2f;
		transform.localScale = scale;
	}

	public override void UpdatePosition(Vector3 newPosition) {
		point0.ReportUpdate(newPosition - transform.position);
		point1.ReportUpdate(newPosition - transform.position);
	}
}