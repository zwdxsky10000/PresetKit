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
            string[] guids = AssetDatabase.FindAssets("t:PresetObject", new[] { Path.GetDirectoryName(path).Replace('\\','/') });
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

        private void OnPostprocessModel()
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
        }
    }
}

