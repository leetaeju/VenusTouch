using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PrintManager : MonoBehaviour {
	private StlExporter stlExporter;
	private bool printed = false;

	private void Awake() {
		stlExporter = GetComponent<StlExporter>();
	}

	public void StartPrint() {
		if (printed) return;
		printed = true;

		string fileName = stlExporter.ExportMesh();

		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo();

		// Hide console
		startInfo.CreateNoWindow = true;
		startInfo.WindowStyle = ProcessWindowStyle.Hidden;
		startInfo.UseShellExecute = false;

		startInfo.FileName = Directory.GetCurrentDirectory() + "/pronsole.exe";
		startInfo.Arguments = "-e \"connect com4\" -e \"slice " + fileName + "\" -e \"print\"";
		process.StartInfo = startInfo;
		process.Start();
	}
}