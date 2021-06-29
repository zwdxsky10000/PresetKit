using System;
using System.IO;
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

        [Tooltip("是否递归子文件夹")]
        public bool recursive;

        [Tooltip("处理规则")]
        public PresetRule[] rules;

        public PresetRule FindMatchRule(AssetImporter importer)
        {
            if (rules != null)
            {
                foreach (var preset in rules)
                {
                    if (preset.IsMatch(importer))
                    {
                        return preset;
                    }
                }
            }
            return null;
        }

        public void Apply(AssetImporter importer)
        {
            PresetRule preset = FindMatchRule(importer);
            if (preset != null)
            {
                preset.Apply(path, importer);
            }
        }
    }

    public enum AssetType
    {
        Png = 0,
        Jpg = 1,
        Prefab = 2,
    }

    [Serializable]
    public class PresetRule
    {
        [SerializeField]
        public Preset preset;

        [SerializeField]
        public string regex;

        [SerializeField]
        public AssetType extension;

        public bool IsMatch(AssetImporter importer)
        {
            string assetPath = importer.assetPath;
            string assetName = Path.GetFileName(assetPath);
            string assetExt = Path.GetExtension(assetPath);

            if (!string.IsNullOrEmpty(regex))
            {
                Regex reg = new Regex(regex);
                return reg.IsMatch(assetName);
            }
            else
            {
                string ext = GetExtension(extension);
                return ext.Equals(assetExt);
            }
        }

        public void Apply(string path, AssetImporter importer)
        {
            if (preset == null)
            {
                Debug.LogErrorFormat("This preset is null or empty! path:{0}", path);
                return;
            }
            if (preset.CanBeAppliedTo(importer))
            {
                preset.ApplyTo(importer);
            }
        }

        static string GetExtension(AssetType type)
        {
            string ext = string.Empty;
            switch (type)
            {
                case AssetType.Png:
                    ext = ".png";
                    break;
                case AssetType.Jpg:
                    ext = ".jpg";
                    break;
                case AssetType.Prefab:
                    ext = ".prefab";
                    break;
            }
            return ext;
        }
    }
}
