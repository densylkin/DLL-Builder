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
    private readonly string _path;
    private readonly string _name;
    private readonly string[] _sources;
    private readonly string[] _editorSources;
    private readonly References _references;
    private readonly Defines _defines;

    private string MainDllPAth { get { return _path + "/" + _name + ".dll"; } }
    private string EditorDllPath { get { return _path + "/" + _name + "_Editor" + ".dll"; } }

    public DLLBuilder(string path, string name, ScriptsList list, References refs, Defines defs, bool buildEditor)
    {
        _path = path;
        _name = name;
        _sources = list.GetSources();
        _editorSources = list.GetEditorSources();
        _references = refs;
        _defines = defs;
    }

    public bool Build(bool editor)
    {
        if (!Directory.Exists(_path))
            return false;

        var compilerParams = PrepareCompileParams(editor);
        var provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.0" } });
        var sources = editor ? _editorSources : _sources;

        compilerParams.OutputAssembly = editor ? EditorDllPath : MainDllPAth;
        var results = provider.CompileAssemblyFromFile(compilerParams, sources);

        if (results.Errors.Count > 0)
        {
            Debug.LogError("Error when creating " + _name + " dll");
            foreach (var error in results.Errors)
            {
                Debug.Log(error.ToString());
            }
            return false;
        }
        return true;
    }

    private CompilerParameters PrepareCompileParams(bool isEditor)
    {
        var compilerParams = new CompilerParameters();
        compilerParams.ReferencedAssemblies.Add("System.dll");
        foreach (var file in _references.Files)
        {
            if(!isEditor && file.Name.Contains("Editor"))
                continue;
            compilerParams.ReferencedAssemblies.Add(file.FullName);
        }
        if (isEditor)
        {
            if (File.Exists(MainDllPAth))
            {
                compilerParams.ReferencedAssemblies.Add(MainDllPAth);
            }
        }

        compilerParams.TreatWarningsAsErrors = true;
        compilerParams.WarningLevel = 1;

        compilerParams.CompilerOptions = "/optimize";
        if(isEditor)
            compilerParams.CompilerOptions += "\n/define:UNITY_EDITOR";
        //compilerParams.CompilerOptions += "/define:" + string.Join(";", _defines.List.ToArray());
        return compilerParams;
    }
}