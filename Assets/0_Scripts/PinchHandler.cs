using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PinchHandler : MonoBehaviour {
	protected Transform pinchTransformLeft;
	protected Transform pinchTransformRight;
	protected Transform[] pinchTransforms;

	protected bool isLeftPinching = false;
	protected bool isRightPinching = false;

	public void Init(Transform left, Transform right) {
		pinchTransformLeft = left;
		pinchTransformRight = right;
		pinchTransforms = new Transform[] { left, right };
	}

	public virtual void OnActiveChange(bool on)
	{

	}

	public abstract void ChangePinchState(int type, bool pinching);
	public abstract void DoUpdate();
}