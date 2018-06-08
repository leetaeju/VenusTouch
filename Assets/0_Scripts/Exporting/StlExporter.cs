using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StlExporter : MonoBehaviour {
	[SerializeField]
	private Transform meshParent;
	[SerializeField]
	private MeshFilter combinedMeshFilter;

	public string ExportMesh() {
		Vector3 prevScale = meshParent.localScale;
		meshParent.localScale *= 90f;

		Block[] blocks = meshParent.GetComponentsInChildren<Block>();

		Mesh combinedMesh = new Mesh();

		CombineInstance[] combiners = new CombineInstance[blocks.Length];

		for (int i = 0; i < blocks.Length; i++) {
			MeshFilter filter = blocks[i].transform.GetChild(0).GetComponent<MeshFilter>();

			combiners[i].subMeshIndex = 0;
			combiners[i].mesh = filter.sharedMesh;
			combiners[i].transform = filter.transform.localToWorldMatrix;
		}

		combinedMesh.CombineMeshes(combiners);
		combinedMeshFilter.sharedMesh = combinedMesh;

		//string fileName = "result" + System.DateTime.Now.ToString("ddhhmmss");
		//string path = Application.persistentDataPath + "/" + fileName + ".stl";
		string fileName = "STLs/" + System.DateTime.Now.ToString("ddhhmmss") + ".stl";

		MeshToFile(combinedMeshFilter.mesh, fileName);

		combinedMeshFilter.sharedMesh = null;

		meshParent.localScale = prevScale;
		return fileName;
	}

	private void MeshToFile(Mesh mesh, string fileName) {
		using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create))) {
			// 80 byte header
			writer.Write(new byte[80]);

			uint totalTriangleCount = (uint)(mesh.triangles.Length / 3);

			// unsigned long facet count (4 bytes)
			writer.Write(totalTriangleCount);
			Vector3[] v = mesh.vertices;
			Vector3[] n = mesh.normals;

			int[] t = mesh.triangles;

			int triangleCount = t.Length;

			for (int i = 0; i < triangleCount; i += 3) {
				int a = t[i], b = t[i + 1], c = t[i + 2];

				Vector3 normal = CalculateNormal(n[a], n[b], n[c]);

				writer.Write(normal.z);
				writer.Write(normal.x);
				writer.Write(normal.y);

				writer.Write(v[a].z);
				writer.Write(v[a].x);
				writer.Write(v[a].y);

				writer.Write(v[b].z);
				writer.Write(v[b].x);
				writer.Write(v[b].y);

				writer.Write(v[c].z);
				writer.Write(v[c].x);
				writer.Write(v[c].y);

				writer.Write((ushort)0);
			}
		}
	}

	private Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c) {
		return new Vector3(
			(a.x + b.x + c.x) / 3f,
			(a.y + b.y + c.y) / 3f,
			(a.z + b.z + c.z) / 3f);
	}
}