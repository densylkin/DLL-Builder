using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[Serializable]
public class Defines
{
    public List<string> List = new List<string>
    {
        "UNITY_EDITOR"
    };

    public int Count { get { return List.Count; } }

    public void Load()
    {
        var json = EditorPrefs.GetString("defines");
        if (string.IsNullOrEmpty(json))
            return;
        List = JsonUtility.FromJson<Defines>(json).List;
    }

    public void Save()
    {
        EditorPrefs.SetString("defines", JsonUtility.ToJson(this));
    }
}