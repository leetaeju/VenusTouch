using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditManager : MonoBehaviour {
	public EditableVertex editableVertexPrefab;
	public EditableLine editableLinePrefab;

	public static EditManager Instance { private set; get; }

	void Awake()
	{
		Instance = this;
	}
}
