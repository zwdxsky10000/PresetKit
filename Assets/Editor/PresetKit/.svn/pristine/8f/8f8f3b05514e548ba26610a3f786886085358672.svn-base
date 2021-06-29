using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetImportCop : AssetPostprocessor
{
    static PresetRule FindRuleForAsset(string path)
    {
        return SearchRecursive(path);
    }

    static PresetRule SearchRecursive(string path)
    {
        foreach (var findAsset in AssetDatabase.FindAssets("t:PresetRule", new[] {Path.GetDirectoryName(path)}))
        {
            var p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
            if (p == Path.GetDirectoryName(path))
            {
                Debug.Log("Found AssetRule for Asset Rule" + AssetDatabase.GUIDToAssetPath(findAsset));
                {
                    return AssetDatabase.LoadAssetAtPath<PresetRule>(AssetDatabase.GUIDToAssetPath(findAsset));
                }
            }
        }

        //no match so go up a level
        path = Directory.GetParent(path).FullName;
        path = path.Replace('\\','/');
        path = path.Remove(0, Application.dataPath.Length);
        path = path.Insert(0, "Assets");
        Debug.Log("Searching: " + path);
       if (path != "Assets")
            return SearchRecursive(path);

        //no matches
        return null;
    }

    private void OnPreprocessTexture()
    {
        PresetRule rule = FindRuleForAsset(assetImporter.assetPath);

        if (rule == null)
        {
            return;
        }
        rule.Apply(assetImporter);
    }

    private void OnPreprocessModel()
    {
        PresetRule rule = FindRuleForAsset(assetImporter.assetPath);

        if (rule == null)
        {
            return;
        }
        rule.Apply(assetImporter);
    }

    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //foreach (string str in importedAsset)
        //{
        //    Debug.Log("importedAsset = " + str);
        //}

        //foreach (string str in deletedAssets)
        //{
        //    Debug.Log("deletedAssets = " + str);
        //}

        foreach (string str in movedAssets)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(str);
            PresetRule rule = FindRuleForAsset(assetImporter.assetPath);

            if (rule == null)
            {
                return;
            }
            rule.Apply(assetImporter);
        }

        //foreach (string str in movedFromAssetPaths)
        //{
        //    Debug.Log("movedFromAssetPaths = " + str);
        //}
    }
}
