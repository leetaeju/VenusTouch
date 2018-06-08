using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjExporter : MonoBehaviour {
	[SerializeField]
	private Transform meshParent;
	[SerializeField]
	private MeshFilter combinedMeshFilter;

	public void ExportMesh() {
		MeshFilter[] filters = meshParent.GetComponentsInChildren<MeshFilter>();
		Mesh combinedMesh = new Mesh();

		CombineInstance[] combiners = new CombineInstance[filters.Length];

		for (int i = 0; i < filters.Length; i++) {
			combiners[i].subMeshIndex = 0;
			combiners[i].mesh = filters[i].sharedMesh;
			combiners[i].transform = filters[i].transform.localToWorldMatrix;
		}

		combinedMesh.CombineMeshes(combiners);
		combinedMeshFilter.sharedMesh = combinedMesh;

		string fileName = "Model" + System.DateTime.Now.ToString("ddhhmmss");
		string path = Application.persistentDataPath + "/" + fileName + ".obj";
		print(path);
		MeshToFile(combinedMeshFilter, path);

		combinedMeshFilter.sharedMesh = null;
	}

	private string MeshToString(MeshFilter mf) {
		Mesh m = mf.sharedMesh;
		Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

		StringBuilder sb = new StringBuilder();

		sb.Append("g ").Append(mf.name).Append("\n");
		foreach (Vector3 v in m.vertices) {
			sb.Append(string.Format("v {0} {1} {2}\n", v.z, v.x, v.y));
		}
		sb.Append("\n");
		foreach (Vector3 v in m.normals) {
			sb.Append(string.Format("vn {0} {1} {2}\n", v.z, v.x, v.y));
		}
		sb.Append("\n");
		foreach (Vector3 v in m.uv) {
			sb.Append(string.Format("vt {0} {1}\n", v.z, v.x));
		}
		for (int material = 0; material < m.subMeshCount; material++) {
			sb.Append("\n");
			sb.Append("usemtl ").Append(mats[material].name).Append("\n");
			sb.Append("usemap ").Append(mats[material].name).Append("\n");

			int[] triangles = m.GetTriangles(material);
			for (int i = 0; i < triangles.Length; i += 3) {
				sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
				   triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
			}
		}
		return sb.ToString();
	}

	private void MeshToFile(MeshFilter mf, string filename) {
		using (StreamWriter sw = new StreamWriter(filename)) {
			sw.Write(MeshToString(mf));
		}
	}

	private void MeshToSTLFile(Mesh mesh, string filename) {
		using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create))) {
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

				// specification says attribute byte count should be set to 0.
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