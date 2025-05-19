using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class DebugLogScanner : EditorWindow
{
    private Vector2 scrollPosition;
    private List<string> foundLogs = new List<string>();

    [MenuItem("Tools/Scan for Debug.Logs")]
    public static void ShowWindow()
    {
        GetWindow<DebugLogScanner>("Debug Log Scanner");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Scan Project for Debug Logs"))
        {
            ScanProject();
        }

        GUILayout.Label("Found Debug.Log calls:", EditorStyles.boldLabel);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (string line in foundLogs)
        {
            GUILayout.Label(line, EditorStyles.label);
        }
        GUILayout.EndScrollView();
    }

    private void ScanProject()
    {
        foundLogs.Clear();

        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("Debug.Log") || line.Contains("Debug.LogWarning") || line.Contains("Debug.LogError"))
                {
                    string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace("\\", "/");
                    foundLogs.Add($"{relativePath} (Line {i + 1}): {line.Trim()}");
                }
            }
        }

        if (foundLogs.Count == 0)
        {
//             Debug.Log("✅ No Debug.Log calls found in project.");
        }
        else
        {
//             Debug.LogWarning($"⚠️ Found {foundLogs.Count} Debug.Log-related calls. Check the 'Debug Log Scanner' window.");
        }
    }
}
