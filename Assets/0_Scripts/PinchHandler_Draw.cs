using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using System;
using Definitions;

public class PinchHandler_Draw : PinchHandler {
	[SerializeField]
	private Block drawPrefab;

	private Block creatingBlock = null;
	private bool[] isDrawing = new bool[] { false, false };

	[SerializeField]
	private Material _material;

	[SerializeField]
	private float _smoothingDelay = 0.01f;

	[SerializeField]
	private float _drawRadius = 0.2f;

	[SerializeField]
	private int _drawResolution = 8;

	[SerializeField]
	private float _minSegmentLength = 0.005f;

	private DrawState[] _drawStates;

	public override void ChangePinchState(int type, bool pinchStarted) {
		if (pinchStarted) {
			if (HandStateManager.Instance.state == HandState.Drawing ||
				HandStateManager.Instance.state == HandState.Idle) {
				HandStateManager.Instance.state = HandState.Drawing;
				isDrawing[type] = true;
				_drawStates[type].BeginNewLine(pinchTransforms[type].position);
			}
		}
		else if (isDrawing[type]) {
			if (!isDrawing[1 - type]) HandStateManager.Instance.state = HandState.Idle;
			isDrawing[type] = false;
			_drawStates[type].FinishLine();
		}
	}

	public override void DoUpdate() {
		for (int i = 0; i < 2; i++) {
			if (isDrawing[i]) {
				_drawStates[i].UpdateLine(pinchTransforms[i].position);
			}
		}
	}

	private void Start() {
		_drawStates = new DrawState[2];
		for (int i = 0; i < 2; i++) {
			_drawStates[i] = new DrawState(this);
		}
	}

	private class DrawState {
		private List<Vector3> _vertices = new List<Vector3>();
		private List<int> _tris = new List<int>();
		private List<Vector2> _uvs = new List<Vector2>();
		private List<Color> _colors = new List<Color>();

		private PinchHandler_Draw _parent;

		private int _rings = 0;

		private Vector3 _prevRing0 = Vector3.zero;
		private Vector3 _prevRing1 = Vector3.zero;

		private Vector3 _prevNormal0 = Vector3.zero;

		private Mesh _mesh;
		private SmoothedVector3 _smoothedPosition;

		private Block currentBlock;

		public DrawState(PinchHandler_Draw parent) {
			_parent = parent;

			_smoothedPosition = new SmoothedVector3();
			_smoothedPosition.delay = parent._smoothingDelay;
			_smoothedPosition.reset = true;
		}

		public Block BeginNewLine(Vector3 startPosition) {
			_rings = 0;
			_vertices.Clear();
			_tris.Clear();
			_uvs.Clear();
			_colors.Clear();

			_smoothedPosition.reset = true;

			_mesh = new Mesh();
			_mesh.name = "Line Mesh";
			_mesh.MarkDynamic();

			// WTF?
			Block newLine = Instantiate(_parent.drawPrefab, Vector3.zero, Quaternion.identity);
			newLine.transform.position = Vector3.zero;
			GameObject child = newLine.transform.GetChild(0).gameObject;
			child.GetComponent<MeshFilter>().mesh = _mesh;
			child.GetComponent<MeshRenderer>().sharedMaterial = _parent._material;

			currentBlock = newLine;
			return newLine;
		}

		public void UpdateLine(Vector3 position) {
			_smoothedPosition.Update(position, Time.deltaTime);

			bool shouldAdd = false;

			shouldAdd |= _vertices.Count == 0;
			shouldAdd |= Vector3.Distance(_prevRing0, _smoothedPosition.value) >= _parent._minSegmentLength;

			if (shouldAdd) {
				addRing(_smoothedPosition.value);
				updateMesh();
			}
		}

		public void FinishLine() {
			_mesh.UploadMeshData(false);
			BoxCollider collider = currentBlock.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
			//collider.convex = true;
			collider.isTrigger = true;
			currentBlock.SetMaterialColor(PinchHandler_Place.Instance.placedColor);
			//collider.sharedMesh = _mesh;
			currentBlock.PlaceBlock();
		}

		private void updateMesh() {
			_mesh.SetVertices(_vertices);
			_mesh.SetColors(_colors);
			_mesh.SetUVs(0, _uvs);
			_mesh.SetIndices(_tris.ToArray(), MeshTopology.Triangles, 0);
			_mesh.RecalculateBounds();
			_mesh.RecalculateNormals();
		}

		private void addRing(Vector3 ringPosition) {
			_rings++;

			if (_rings == 1) {
				addVertexRing();
				addVertexRing();
				addTriSegment();
			}

			addVertexRing();
			addTriSegment();

			Vector3 ringNormal = Vector3.zero;
			if (_rings == 2) {
				Vector3 direction = ringPosition - _prevRing0;
				float angleToUp = Vector3.Angle(direction, Vector3.up);

				if (angleToUp < 10 || angleToUp > 170) {
					ringNormal = Vector3.Cross(direction, Vector3.right);
				}
				else {
					ringNormal = Vector3.Cross(direction, Vector3.up);
				}

				ringNormal = ringNormal.normalized;

				_prevNormal0 = ringNormal;
			}
			else if (_rings > 2) {
				Vector3 prevPerp = Vector3.Cross(_prevRing0 - _prevRing1, _prevNormal0);
				ringNormal = Vector3.Cross(prevPerp, ringPosition - _prevRing0).normalized;
			}

			if (_rings == 2) {
				updateRingVerts(0,
								_prevRing0,
								ringPosition - _prevRing1,
								_prevNormal0,
								0);
			}

			if (_rings >= 2) {
				updateRingVerts(_vertices.Count - _parent._drawResolution,
								ringPosition,
								ringPosition - _prevRing0,
								ringNormal,
								0);
				updateRingVerts(_vertices.Count - _parent._drawResolution * 2,
								ringPosition,
								ringPosition - _prevRing0,
								ringNormal,
								1);
				updateRingVerts(_vertices.Count - _parent._drawResolution * 3,
								_prevRing0,
								ringPosition - _prevRing1,
								_prevNormal0,
								1);
			}

			_prevRing1 = _prevRing0;
			_prevRing0 = ringPosition;

			_prevNormal0 = ringNormal;
		}

		private void addVertexRing() {
			for (int i = 0; i < _parent._drawResolution; i++) {
				_vertices.Add(Vector3.zero);  //Dummy vertex, is updated later
				_uvs.Add(new Vector2(i / (_parent._drawResolution - 1.0f), 0));
				//_colors.Add(_parent._drawColor);
			}
		}

		//Connects the most recently added vertex ring to the one before it
		private void addTriSegment() {
			for (int i = 0; i < _parent._drawResolution; i++) {
				int i0 = _vertices.Count - 1 - i;
				int i1 = _vertices.Count - 1 - ((i + 1) % _parent._drawResolution);

				_tris.Add(i0);
				_tris.Add(i1 - _parent._drawResolution);
				_tris.Add(i0 - _parent._drawResolution);

				_tris.Add(i0);
				_tris.Add(i1);
				_tris.Add(i1 - _parent._drawResolution);
			}
		}

		private void updateRingVerts(int offset, Vector3 ringPosition, Vector3 direction, Vector3 normal, float radiusScale) {
			direction = direction.normalized;
			normal = normal.normalized;

			for (int i = 0; i < _parent._drawResolution; i++) {
				float angle = 360.0f * (i / (float)(_parent._drawResolution));
				Quaternion rotator = Quaternion.AngleAxis(angle, direction);
				Vector3 ringSpoke = rotator * normal * _parent._drawRadius * radiusScale;
				_vertices[offset + i] = ringPosition + ringSpoke;
			}
		}
	}
}