using System.IO;
using UnityEditor;

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
            string[] guids = AssetDatabase.FindAssets("t:PresetObject", new[] { Path.GetDirectoryName(path)});
            if(guids != null && guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<PresetObject>(p);
                }
            }
            //no matches
            return null;
        }

        public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAsset)
            {
                HandleAssets(str);
            }

            foreach (string str in movedAssets)
            {
                HandleAssets(str);
            }
        }

        static void HandleAssets(string str)
        {
            if (AssetDatabase.IsValidFolder(str))
            {
                return;
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath(str);
            PresetObject rule = FindRuleForAsset(assetImporter.assetPath);

            if (rule == null)
            {
                return;
            }
            rule.Apply(assetImporter);
        }
    }
}

