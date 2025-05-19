using UnityEngine;
using UnityEditor;
using System.IO;

public class DebugLogAutoCommenter : EditorWindow
{
    [MenuItem("Tools/Comment Out Debug.Logs")]
    public static void ShowWindow()
    {
        if (EditorUtility.DisplayDialog("Comment Debug.Logs",
                "This will comment out all Debug.Log, Debug.LogWarning, and Debug.LogError lines in your scripts.\n\nAre you sure?",
                "Yes, Comment All", "Cancel"))
        {
            CommentDebugLogs();
        }
    }

    private static void CommentDebugLogs()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int modifiedFiles = 0;
        int totalLinesCommented = 0;

        foreach (string file in files)
        {
            string[] lines = File.ReadAllLines(file);
            bool fileModified = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].TrimStart();

                if ((trimmed.StartsWith("Debug.Log(") || 
                     trimmed.StartsWith("Debug.LogWarning(") || 
                     trimmed.StartsWith("Debug.LogError(")) &&
                    !trimmed.StartsWith("//"))
                {
                    lines[i] = "// " + lines[i];
                    fileModified = true;
                    totalLinesCommented++;
                }
            }

            if (fileModified)
            {
                File.WriteAllLines(file, lines);
                modifiedFiles++;
            }
        }

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Done",
            $"✅ Commented out {totalLinesCommented} Debug.Log lines in {modifiedFiles} script files.",
            "OK");
    }
}