using System.IO;
using UnityEditor;
using UnityEngine;

namespace PresetKit
{
    public class PresetKitPostProcessor : AssetPostprocessor
    {
        static PresetObject FindRuleForAsset(string path)
        {
            return SearchRecursive(path);
        }

        static PresetObject SearchRecursive(string path)
        {
            foreach (var findAsset in AssetDatabase.FindAssets("t:PresetRule", new[] { Path.GetDirectoryName(path) }))
            {
                var p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
                if (p == Path.GetDirectoryName(path))
                {
                    //Debug.Log("Found AssetRule for Asset Rule" + AssetDatabase.GUIDToAssetPath(findAsset));
                    return AssetDatabase.LoadAssetAtPath<PresetObject>(AssetDatabase.GUIDToAssetPath(findAsset));
                }
            }

            //no match so go up a level
            path = Directory.GetParent(path).FullName;
            path = path.Replace('\\', '/');
            path = path.Remove(0, Application.dataPath.Length);
            path = path.Insert(0, "Assets");
            if (path != "Assets")
                return SearchRecursive(path);

            //no matches
            return null;
        }

        private void OnPreprocessTexture()
        {
            PresetObject rule = FindRuleForAsset(assetImporter.assetPath);

            if (rule == null)
            {
                return;
            }
            rule.Apply(assetImporter);
        }

        private void OnPreprocessModel()
        {
            PresetObject rule = FindRuleForAsset(assetImporter.assetPath);

            if (rule == null)
            {
                return;
            }
            rule.Apply(assetImporter);
        }

        public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAsset)
            {
                //Debug.Log("importedAsset = " + str);
            }

            foreach (string str in deletedAssets)
            {
                Debug.Log("deletedAssets = " + str);
            }

            foreach (string str in movedAssets)
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(str);
                PresetObject rule = FindRuleForAsset(assetImporter.assetPath);

                if (rule == null)
                {
                    return;
                }
                rule.Apply(assetImporter);
            }

            foreach (string str in movedFromAssetPaths)
            {
                Debug.Log("movedFromAssetPaths = " + str);
            }
        }
    }
}

