using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityHelpers;

[Serializable]
public class References
{
    public List<string> Paths = new List<string>();
    public readonly List<bool> Include = new List<bool>();
    public readonly List<FileInfo> Files = new List<FileInfo>();

    public int Count { get { return Files.Count; } }

    private static readonly string[] _defaultIncludes =
    {
        @"Managed\UnityEngine.dll",
        @"Managed\UnityEditor.dll", 
        @"UnityExtensions\Unity\GUISystem\UnityEngine.UI.dll" 
    };

    public void Add(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        if (Paths.Contains(path))
            return;

        Paths.Add(path);
        Include.Add(true);
        Files.Add(new FileInfo(path));
        Save();
    }

    public void Remove(int index)
    {
        if (index >= Count)
            return;

        Paths.RemoveAt(index);
        Include.RemoveAt(index);
        Files.RemoveAt(index);
        Save();
    }

    public void Load()
    {
        foreach (var s in _defaultIncludes)
        {
            var path = Path.Combine(PathUtils.UnixToWindowsPath(EditorApplication.applicationContentsPath), s);
            Add(path);
        }

        var refsJson = EditorPrefs.GetString("references");
        var refs = string.IsNullOrEmpty(refsJson) ? new References() : JsonUtility.FromJson<References>(refsJson);
        Paths = refs.Paths;
        foreach (var path in refs.Paths)
        {
            if (Paths.Contains(path))
                continue;
            Files.Add(new FileInfo(path));
            Include.Add(true);
        }
    }

    public void Save()
    {
        EditorPrefs.SetString("references", JsonUtility.ToJson(this));
    }
}