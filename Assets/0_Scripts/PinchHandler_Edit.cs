using Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchHandler_Edit : PinchHandler
{
	public Transform[] _fingers;
	private Transform[,] fingers;

	public float editRadius;
	private int currentHandType;
	private EditableThing currentEditing;

	public EditableDetector detector1;
	public EditableDetector detector2;

	public static PinchHandler_Edit Instance { private set; get; }

	void Awake()
	{
		fingers = new Transform[2, 2];
		fingers[0, 0] = _fingers[0];
		fingers[0, 1] = _fingers[1];
		fingers[1, 0] = _fingers[2];
		fingers[1, 1] = _fingers[3];
	}

	public override void ChangePinchState(int type, bool pinchStarted)
	{
		if (pinchStarted)
		{
			if (HandStateManager.Instance.state == HandState.Idle)
			{
				Vector3 point = (fingers[type, 0].position + fingers[type, 1].position) / 2f;
				Collider[] cols = Physics.OverlapSphere(point, editRadius, Values.EditableLayerMask);
				if (cols != null && cols.Length >= 1)
				{
					Collider col = null;
					float minMag = 99999f;

					foreach (var _col in cols)
					{
						float mag = Vector3.Magnitude(point - _col.transform.position);
						if (mag < minMag)
						{
							col = _col;
							minMag = mag;
						}
					}

					// Start edit
					currentHandType = type;
					currentEditing = col.GetComponent<EditableThing>();
					currentEditing.SetOnSelected(true);
					HandStateManager.Instance.state = HandState.Editing;
				}
			}
		}
		else
		{
			if (currentHandType == type && currentEditing != null)
			{
				// Stop edit
				currentEditing.SetOnSelected(true);
				currentEditing = null;
				HandStateManager.Instance.state = HandState.Idle;
			}
		}
	}

	public override void DoUpdate()
	{
		if (HandStateManager.Instance.state == HandState.Editing)
		{
			currentEditing.UpdatePosition((fingers[currentHandType, 0].position + fingers[currentHandType, 1].position) / 2f);
		}
	}

	public override void OnActiveChange(bool on)
	{
		if (!on)
		{
			detector1.ExitAllColliders();
			detector2.ExitAllColliders();
		}
		detector1.interalActive = on;
		detector2.interalActive = on;

		detector1.gameObject.SetActive(on);
		detector2.gameObject.SetActive(on);
	}
}