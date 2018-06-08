using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditableThing : MonoBehaviour
{
	public abstract void UpdatePosition(Vector3 newPosition);

	protected MeshRenderer meshRenderer;
	private int handOverCount = 0;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

	public void EnterVisibleRange()
	{
		if (++handOverCount == 1)
		{
			meshRenderer.enabled = true;
		}
	}

	public void ExitVisibleRange()
	{
		if (--handOverCount == 0)
		{
			meshRenderer.enabled = false;
		}
	}

	public void Reset()
	{
		handOverCount = 0;
		meshRenderer.enabled = false;
	}

	public void SetOnSelected(bool selected)
	{

	}
}