using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Definitions;

public class GrabListener : MonoBehaviour {
	private const int LEFT = 0, RIGHT = 1;

	[SerializeField]
	private Transform[] grabTransforms;
	private Vector3[] prevPosition = new Vector3[2];

	[SerializeField]
	private Transform meshTransform;

	public Transform[] thingToMove;

	private Block[] grabbingBlocks = new Block[2];
	private Transform[] prevParent = new Transform[2];
	private bool[] isGrabbing = new bool[] { false, false };

	public void ThumbUp(int type) {
		Thumb(type, true);
	}

	public void ThumbDown(int type) {
		Thumb(type, false);
	}

	private void Thumb(int type, bool up) {
		if (isGrabbing[1 - type]) {
			if (up) {
				// Copy
				if (grabbingBlocks[1 - type] != null) {
					Block newBlock = Instantiate(grabbingBlocks[1 - type].gameObject).GetComponent<Block>();
					PinchHandler_Place.Instance.PlaceBlock(newBlock);
				}
			}
			else {
				// Delete
				EndGrab(1 - type, true);
			}
		}
	}

	public void StartGrab(int type) {
		isGrabbing[type] = true;
		if ((HandStateManager.Instance.state != HandState.Idle) &&
			(HandStateManager.Instance.state != HandState.Grabing)) return;

		if (isGrabbing[1 - type]) {
			// Both grabbing
			EndGrab(1 - type);
			isGrabbing[1 - type] = true;
			HandStateManager.Instance.state = HandState.Moving;
			RecordPosition();
			StartCoroutine("MovingCo");
		}
		else {
			Collider[] cols = Physics.OverlapSphere(grabTransforms[type].position, 0.04f, Values.BlockLayerMask);

			if (cols != null && cols.Length >= 1) {
				Collider col = null;
				float minMag = 99999f;

				foreach (var _col in cols) {
					Vector3 closestPoint = _col.ClosestPoint(grabTransforms[type].position);
					float mag = Vector3.Magnitude(grabTransforms[type].position - closestPoint);
					if (mag < minMag) {
						col = _col;
						minMag = mag;
					}
				}

				Block blockToGrab = col.transform.parent.GetComponent<Block>();
				HandStateManager.Instance.state = HandState.Grabing;
				grabbingBlocks[type] = blockToGrab;
				prevParent[type] = blockToGrab.transform.parent;
				blockToGrab.transform.parent = grabTransforms[type];
			}
		}
	}

	public void EndGrab(int type, bool delete = false) {
		isGrabbing[type] = false;
		StopCoroutine("MovingCo");
		if (HandStateManager.Instance.state == HandState.Moving) {
			HandStateManager.Instance.state = HandState.Idle;
		}

		if (grabbingBlocks[type] != null) {
			grabbingBlocks[type].transform.parent = prevParent[type];
			if (delete) {
				Destroy(grabbingBlocks[type].gameObject);
			}
			grabbingBlocks[type] = null;

			if (grabbingBlocks[1 - type] == null) {
				HandStateManager.Instance.state = HandState.Idle;
			}
		}
	}

	private IEnumerator MovingCo() {
		while (true) {
			Vector3 delta0 = grabTransforms[0].position - prevPosition[0];
			Vector3 delta1 = grabTransforms[1].position - prevPosition[1];

			if (Vector3.Angle(delta0, delta1) > 135f) {
				// Rotate
				//Quaternion rotation = Quaternion.FromToRotation(prevPosition[1] - prevPosition[0], grabTransforms[1].position - grabTransforms[0].position);
				//for (int i = 0; i < meshTransform.childCount; i++) {
				//	meshTransform.GetChild(i).rotation = rotation * meshTransform.GetChild(i).rotation;
				//}
			}
			else {
				// Move
				Vector3 moveVector = (delta0 + delta1) * 0.8f;
				//if (Vector3.Magnitude(moveVector) > 0.003f) {

				//meshTransform.position += moveVector;
				foreach (var thing in thingToMove) {
					thing.position -= moveVector;
				}

				//}
			}

			RecordPosition();
			yield return null;
		}
	}

	private void RecordPosition() {
		prevPosition[0] = grabTransforms[0].position;
		prevPosition[1] = grabTransforms[1].position;
	}
}