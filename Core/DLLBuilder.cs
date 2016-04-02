using System;
using System.CodeDom.Compiler;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using UnityEditor;
using UnityHelpers;

public class DLLBuilder
{
    public References References = new References();
    public Defines Defines = new Defines();

    public bool Compile(bool editor, string name, string path, ScriptsList list)
    {
        if (!Directory.Exists(path))
            return false;

        var sources = list.GetSources();
        var editorSources = list.GetEditorSources();

        var codeProvider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.0" } });
        var parameters = PrepareCompileParams(editor);

        var outPath = PathUtils.UnixToWindowsPath(path + "/" + name + ".dll");
        parameters.OutputAssembly = outPath;
    }

    private CompilerParameters PrepareCompileParams(bool isEditor)
    {
        var compilerParams = new CompilerParameters();
        foreach (var path in References.Paths)
        {
            if(!isEditor && path.Contains("Editor"))
                continue;
            compilerParams.ReferencedAssemblies.Add(path);
        }

        compilerParams.CompilerOptions += "/optimize";
        compilerParams.CompilerOptions += "/define:" + string.Join(";", Defines.List.ToArray());
        return compilerParams;
    }
}

[Serializable]
public class Defines
{
    public List<string> List = new List<string>
    {
        "UNITY_EDITOR"
    };

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

[Serializable]
public class References
{
    public List<string> Paths = new List<string>();

    [NonSerialized]
    public List<bool> Include = new List<bool>();
    [NonSerialized]
    public List<FileInfo> Files = new List<FileInfo>();

    public int Count { get { return Files.Count; } }

    private static readonly string[] _defaultIncludes =
    {
        @"Managed\UnityEngine.dll",
        @"Managed\UnityEditor.dll", 
        @"UnityExtensions\Unity\GUISystem\UnityEngine.UI.dll" 
    };

    public void Add(string path)
    {
        if(string.IsNullOrEmpty(path))
            return;
        if(Paths.Contains(path))
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
            if(Paths.Contains(path))
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