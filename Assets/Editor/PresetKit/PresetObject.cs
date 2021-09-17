using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace PresetKit
{
    public class PresetObject : ScriptableObject
    {
        [Tooltip("处理路径")]
        public string path;

        [Tooltip("处理规则")]
        public PresetRule[] rules;

        private PresetRule FindMatchRule(AssetImporter importer)
        {
            return rules?.FirstOrDefault(rule => rule.IsMatch(importer));
        }

        public void Apply(AssetImporter importer)
        {
            PresetRule preset = FindMatchRule(importer);
            if (preset != null)
            {
                preset.Apply(path, importer);
            }
            else
            {
                Debug.LogWarningFormat("asset not found match rule. path={0}", importer.assetPath);
            }
        }
    }


    public enum EMatchType
    {
        Regex = 0,
        Extension = 1
    }

    [Serializable]
    public class PresetRule
    {
        [SerializeField]
        public Preset preset;

        public EMatchType type;

        [SerializeField]
        public string pattern;

        public bool IsMatch(AssetImporter importer)
        {
            string assetPath = importer.assetPath;
            string assetName = Path.GetFileName(assetPath);
            string assetExt = Path.GetExtension(assetPath);

            if(type == EMatchType.Regex)
            {
                Regex reg = new Regex(pattern);
                return reg.IsMatch(assetName);
            }
            else
            {
                return assetExt.Equals(pattern);
            }
        }

        public void Apply(string path, AssetImporter importer)
        {
            if (preset == null)
            {
                Debug.LogWarningFormat("This preset is null or empty! path:{0}", path);
                return;
            }
            if (preset.CanBeAppliedTo(importer))
            {
                preset.ApplyTo(importer);
            }
            else
            {
                Debug.LogWarningFormat("preset not apply for asset. path={0}", importer.assetPath);
            }
        }
    }
}
