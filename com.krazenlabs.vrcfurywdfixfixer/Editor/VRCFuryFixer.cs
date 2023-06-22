using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

[InitializeOnLoad]
public static class VRCFuryWDFixFixer
{
    static VRCFuryWDFixFixer()
    {
        ApplyFix();
    }

    [MenuItem("Tools/KrazenLabs/Apply VRCFury WD Fix Fix")]
    public static void ApplyFix()
    {
        Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] Trying to patch VRCFury...");
        string projectRoot = Directory.GetParent(Application.dataPath).ToString();
        string filePath = Path.Combine(projectRoot, "Packages\\com.vrcfury.vrcfury\\Editor\\VF\\Feature\\FixWriteDefaultsBuilder.cs");

        if (!File.Exists(filePath)) return;
        Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] File found. Reading contents...");
        var fileContents = File.ReadAllLines(filePath);

        for (int i = 0; i < fileContents.Length; i++)
        {
            // Check if the fix is already applied
            if (fileContents[i].Contains("return;") && i < fileContents.Length - 1 && fileContents[i + 1].Contains("applyToUnmanagedLayers = false;"))
            {
                Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] Patch already applied");
                return;
            }

            // Look for the three lines pattern in the file as a dirty way to guess whether relevant parts of the code might have changed in a recent version
            if (i < fileContents.Length - 2 && fileContents[i].Trim() == "} else {" && fileContents[i + 1].Trim() == "applyToUnmanagedLayers = false;" && fileContents[i + 2].Trim() == "useWriteDefaults = shouldBeOnIfWeAreNotInControl;")
            {
                Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] Found code block at line " + i);
                // Add "return;" before the line
                fileContents[i + 1] = "                return;\n" + fileContents[i + 1];
                break;
            }
        }

        Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] Modification done. Writing to file...");
        // Rewrite the file with the modification
        File.WriteAllLines(filePath, fileContents, Encoding.UTF8);

        Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] Refresh Asset Database");
        // Refresh the AssetDatabase to make the changes appear in the Unity Editor
        AssetDatabase.Refresh();
        Debug.Log("[KrazenLabs] [VRCFury WD Fix Fix] Done");
    }
}
