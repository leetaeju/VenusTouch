using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchListener : MonoBehaviour
{
	private const int LEFT = 0, RIGHT = 1;

	[SerializeField]
	private Transform pinchTransformLeft;
	[SerializeField]
	private Transform pinchTransformRight;

	[SerializeField]
	private PinchHandler[] handlers;
	private int currentHandlerIndex = 0;

	public void ChangeHandler(int type)
	{
		EndPinch(0);
		EndPinch(1);
		handlers[currentHandlerIndex].OnActiveChange(false);
		currentHandlerIndex = type;
		handlers[currentHandlerIndex].OnActiveChange(true);
	}

	public void StartPinch(int type)
	{
		handlers[currentHandlerIndex].ChangePinchState(type, true);
	}

	public void EndPinch(int type)
	{
		handlers[currentHandlerIndex].ChangePinchState(type, false);
	}

	private void Awake()
	{
		foreach (var handler in handlers)
		{
			handler.Init(pinchTransformLeft, pinchTransformRight);
		}
	}

	private void Update()
	{
		handlers[currentHandlerIndex].DoUpdate();
	}
}