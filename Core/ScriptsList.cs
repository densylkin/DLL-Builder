using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ScriptsList
{
    public List<FileInfo> MainFiles = new List<FileInfo>();
    public List<FileInfo> EditorFiles = new List<FileInfo>();

    public List<bool> MainFilesToInclude = new List<bool>();
    public List<bool> EditorFilesToInclude = new List<bool>(); 

    private string _path = "";
    public string Path { get { return _path; } }

    public bool FilesNotEmpty { get { return MainFiles.Count > 0 || EditorFiles.Count > 0; } }

    public int FilesCount { get { return MainFilesCount + EditorFilesCount; } }
    public int MainFilesCount { get { return MainFiles.Count; } }
    public int EditorFilesCount { get { return EditorFiles.Count; } }

    public void SetRoot(string path)
    {
        _path = path;
        Scan();
    }

    public void Scan()
    {
        var dir = new DirectoryInfo(Path);
        var files = dir.GetFiles("*.cs", SearchOption.AllDirectories);

        var editor = files.Where(f => f.FullName.Contains("Editor")).OrderBy(f => f.FullName.Length);
        var main = files.Except(editor).OrderBy(f => f.FullName.Length);

        MainFiles.AddRange(main);
        EditorFiles.AddRange(editor);

        MainFiles.ForEach(f => MainFilesToInclude.Add(true));
        EditorFiles.ForEach(f => EditorFilesToInclude.Add(true));
    }

    public string[] GetSources()
    {
        return MainFiles.Select(f => f.FullName.Replace(" ", "-")).ToArray();
    }

    public string[] GetEditorSources()
    {
        return EditorFiles.Select(f => f.FullName.Replace(" ", "-")).ToArray();
    }
}