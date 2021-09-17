using System.IO;
using System.Linq;
using UnityEditor;

namespace PresetKit
{
    public class PresetKitPostProcessor : AssetPostprocessor
    {
        private static PresetObject FindRuleForAsset(string path)
        {
            return SearchRecursive(path);
        }

        private static PresetObject SearchRecursive(string path)
        {
            string[] guids = AssetDatabase.FindAssets("t:PresetObject", new[] { Path.GetDirectoryName(path)});
            //no matches
            if (guids == null || guids.Length <= 0) return null;
            return guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Select(p => AssetDatabase.LoadAssetAtPath<PresetObject>(p)).FirstOrDefault();
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

        private static void HandleAssets(string str)
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

