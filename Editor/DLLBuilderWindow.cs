using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.Networking;
using UnityHelpers;
using UnityHelpers.GUI;

public class DLLBuilderWindow : EditorWindow
{
    private readonly ScriptsList _list = new ScriptsList();
    private readonly References _references = new References();
    private readonly Defines _defines = new Defines();

    private string _path = "";
    private string _outputPath = "";
    private string _newRefPath = "";
    private string _editor = "";
    private string _newDefineName = "";

    private bool _editorDll;
    private string _mainDllName = "Assembly";
    private string _editorDllName = "Assembly.Editor";

    private Vector2 _settingsScroll;
    private Vector2 _mainScroll;
    private Vector2 _editorScroll;

    [MenuItem("Tools/DLL Builder")]
    public static void Init()
    {
        var window = GetWindow<DLLBuilderWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        _defines.Load();
        _references.Load();
    }

    private void OnGUI()
    {
        using (new HorizontalBlock())
        {
            FilesPanel();
            SettingsPanel();
        }
    }

    private void FilesPanel()
    {
        using (new VerticalBlock(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            using (new HorizontalBlock(GUILayout.ExpandWidth(true)))
            {
                GUILayout.Label("Main sources path");
                _path = EditorGUILayout.TextField(_path);
                if (GUILayout.Button("browse", GUILayout.ExpandWidth(false)))
                {
                    _path = EditorUtility.OpenFolderPanel("", "Assets", "");
                    if (!string.IsNullOrEmpty(_path))
                        _list.SetRoot(_path);
                }
                GUILayout.FlexibleSpace();
            }
            FilesList();
        }
    }

    private void FilesList()
    {
        if (_list == null)
            return;
        if(_list.MainFilesCount == 0)
            return;
        using (new VerticalBlock(GUI.skin.box))
        {
            using (new ScrollviewBlock(ref _mainScroll))
            {
                DrawFiles(_list.MainFiles, _list.MainFilesToInclude);   
            }
        }
        using (new VerticalBlock(GUI.skin.box))
        {
            using (new ScrollviewBlock(ref _editorScroll))
            {
                GUI.enabled = _editorDll;
                DrawFiles(_list.EditorFiles, _list.EditorFilesToInclude);
                GUI.enabled = true;
            }
        }
    }

    private void DrawFiles(List<FileInfo> files, List<bool> bools)
    {
        for (var i = 0; i < files.Count; i++)
        {
            var filename = PathUtils.GlobalPathToRelative(PathUtils.WindowsToUnixPath(files[i].FullName));
            using (new HorizontalBlock())
            {
                bools[i] = EditorGUILayout.ToggleLeft(filename, bools[i]);
            }
        }
    }

    private void SettingsPanel()
    {
        using (new VerticalBlock(GUI.skin.box, GUILayout.Width(position.width/3), GUILayout.ExpandHeight(true)))
        {
            using (new ScrollviewBlock(ref _settingsScroll))
            {
                using (new VerticalBlock(GUI.skin.box))
                {
                    using (new HorizontalBlock())
                    {
                        GUILayout.Label("Main dll name", GUILayout.ExpandWidth(false));
                        _mainDllName = GUILayout.TextField(_mainDllName);
                    }
                    _editorDll = EditorGUILayout.Toggle("Editor dll", _editorDll);
                    using (new HorizontalBlock())
                    {
                        GUI.enabled = false;
                        GUILayout.Label("Editor dll name", GUILayout.ExpandWidth(false));
                        GUILayout.TextField(_mainDllName + ".Editor");
                        GUI.enabled = true;
                    }
                    GUILayout.Label("Output path");
                    using (new HorizontalBlock())
                    {
                        _outputPath = EditorGUILayout.TextField(_outputPath);
                        if (GUILayout.Button("browse", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                        {
                            _outputPath = EditorUtility.SaveFolderPanel("", "", "");
                        }
                    }
                    if (GUILayout.Button("Build"))
                    {
                        var e = Build();
                        while (e.MoveNext()) ;
                    }
                }
                
                ReferencesPanel();
                DefinesList();
            }
        }
    }

    private void ReferencesPanel()
    {
        using (new VerticalBlock(GUI.skin.box))
        {
            GUILayout.Label("References");
            using (new HorizontalBlock())
            {
                _newRefPath = EditorGUILayout.TextField(_newRefPath);
                if (GUILayout.Button("browse", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false)))
                {
                    _newRefPath = EditorUtility.OpenFilePanel("", "Assets", "dll");
                }
                if (GUILayout.Button("add", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
                {
                    _references.Add(_newRefPath);
                    _newRefPath = "";
                    GUI.FocusControl(null);
                }
            }

            GUILayout.Label(_references.Count.ToString());

            for (var i = 0; i < _references.Count; i++)
            {
                var name = _references.Files[i].Name;
                using (new HorizontalBlock())
                {
                    _references.Include[i] = EditorGUILayout.ToggleLeft(name, _references.Include[i]);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
                    {
                        _references.Remove(i);
                    }
                }
            }
        }
    }

    private void DefinesList()
    {
        using (new VerticalBlock(GUI.skin.box))
        {
            using (new HorizontalBlock())
            {
                _newDefineName = EditorGUILayout.TextField(_newDefineName, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("add", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    _defines.List.Add(_newDefineName);
                    _newDefineName = "";
                    GUI.FocusControl(null);
                }
            }
            for (var i = 0; i < _defines.List.Count; i++)
            {
                using (new HorizontalBlock())
                {
                    GUILayout.Label(_defines.List[i]);
                    if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                    {
                        _defines.List.RemoveAt(i);
                    }
                }
            }

        }
    }

    private IEnumerator Build()
    {
        var path = PathUtils.UnixToWindowsPath(_outputPath);

        var builder = new DLLBuilder(_outputPath, _mainDllName, _list, _references, _defines, _editorDll);
        if (builder.Build(false))
        {
            yield return new WaitForSeconds(1f);
            if (builder.Build(true))
                EditorUtility.DisplayDialog("Success", "Ready", "ok");
        }
    }
}
