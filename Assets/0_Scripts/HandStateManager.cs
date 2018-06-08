using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Definitions;

public class HandStateManager : MonoBehaviour {
	public static HandStateManager Instance { private set; get; }

	public HandState state = HandState.Idle;

	private void Awake() {
		Instance = this;
	}
}