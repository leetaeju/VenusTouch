using UnityEngine;

namespace Definitions {
	public enum HandState {
		Idle,
		Creating,
		Drawing,
		Grabing,
		Moving,
		Editing
	};

	public static class Values {
		public static LayerMask BlockLayerMask { get { return 1 << 8; } }
		public static LayerMask EditableLayerMask { get { return 1 << 9; } }
	}
}
