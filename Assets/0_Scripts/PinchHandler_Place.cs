using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Definitions;

public class PinchHandler_Place : PinchHandler {
	[SerializeField]
	private Block[] blockPrefabs;

	private int currentBlockIndex = 0;

	public Material placingMaterial;
	public Color placedColor;

	private Block creatingBlock = null;
	private Transform creatingMesh = null;

	private const float createThreshold = 0.1f;

	public static PinchHandler_Place Instance { private set; get; }
	private void Awake() {
		Instance = this;
	}

	public void ChangeBlockType(int type) {
		currentBlockIndex = type;
	}

	public override void ChangePinchState(int type, bool pinchStarted) {
		if (type == 0) isLeftPinching = pinchStarted;
		else isRightPinching = pinchStarted;

		if (pinchStarted) {
			if (isLeftPinching && isRightPinching && HandStateManager.Instance.state == HandState.Idle) {
				float magnitude = Vector3.Magnitude(pinchTransformLeft.position - pinchTransformRight.position);

				if (magnitude < createThreshold) {
					// Create block
					creatingBlock = Instantiate(blockPrefabs[currentBlockIndex], (pinchTransformLeft.position + pinchTransformRight.position) / 2f, Quaternion.identity);
					creatingMesh = creatingBlock.GetComponent<EditableObject>().meshTransform;
					creatingMesh.localScale = new Vector3(0.001f, 0.001f, 0.001f);

					// Set placing material
					creatingBlock.SetMateiral(placingMaterial);
					HandStateManager.Instance.state = HandState.Creating;
				}
			}
		}
		else {
			if (HandStateManager.Instance.state == HandState.Creating) {
				// Really instantiate
				//Vector3 scale = creatingBlock.transform.localScale;

				PlaceBlock(creatingBlock);
				creatingBlock = null;
				HandStateManager.Instance.state = HandState.Idle;
			}
		}
	}

	public void PlaceBlock(Block block) {
		block.GetComponent<EditableObject>().Init();
		block.SetMaterialColor(placedColor);
		block.PlaceBlock();
	}

	public override void DoUpdate() {
		if (HandStateManager.Instance.state == HandState.Creating) {
			creatingBlock.transform.position = (pinchTransformLeft.position + pinchTransformRight.position) / 2f;
			Vector3 difference = pinchTransformLeft.position - pinchTransformRight.position;
			creatingMesh.localScale = new Vector3(difference.x, difference.y, difference.z);
		}
	}
}