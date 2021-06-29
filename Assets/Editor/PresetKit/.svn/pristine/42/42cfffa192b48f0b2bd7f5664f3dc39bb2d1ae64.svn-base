using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class PresetRule : ScriptableObject
{
    [Tooltip("处理路径")]
    [SerializeField]
    public string path;

    [Tooltip("是否递归子文件夹")]
    [SerializeField]
    public bool recursive;

    [Tooltip("处理规则")]
    [SerializeField]
    public PresetRuleItem[] presets;

    public PresetRuleItem FindMatchRule(AssetImporter importer)
    {
        if(presets != null)
        {
            foreach(var preset in presets)
            {
                if(preset.IsMatch(importer))
                {
                    return preset;
                }
            }
        }
        return null;
    }

    public void Apply(AssetImporter importer)
    {
        PresetRuleItem preset = FindMatchRule(importer);
        if (preset != null)
        {
            preset.Apply(path, importer);
        }
    }
}

[Serializable]
public class PresetRuleItem
{
    [SerializeField]
    public Preset preset;

    [SerializeField]
    public string regex;

    [SerializeField]
    public string extension;

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
            return string.IsNullOrEmpty(extension) || extension.Equals(assetExt);
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
}