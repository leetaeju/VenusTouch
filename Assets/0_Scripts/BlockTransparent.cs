using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Definitions;
using Leap;
using Leap.Unity;

public class BlockTransparent : MonoBehaviour {
	private float colliderRadius = 0.04f;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Block")) {
			other.transform.parent.GetComponent<Block>().HandEnter();
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Block")) {
			other.transform.parent.GetComponent<Block>().HandExit();
		}
	}

	public void ExitAllColliders() {
		Collider[] cols = Physics.OverlapSphere(transform.position, colliderRadius, Values.BlockLayerMask);
		foreach (var col in cols) {
			col.transform.parent.GetComponent<Block>().HandExit();
		}
	}
}
