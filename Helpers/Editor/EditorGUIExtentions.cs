using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityHelpers.GUI;
using UnityObject = UnityEngine.Object;

public static class EditorGUIExtentions
{
    /// <summary>
    /// Creates toggleable button
    /// </summary>
    /// <param name="label">Button text</param>
    /// <param name="value"></param>
    /// <param name="options">GUILayout options</param>
    /// <returns>Current button state</returns>
    public static bool ToggleButton(string label, bool value, params GUILayoutOption[] options)
    {
        return GUILayout.Toggle(value, label, EditorStyles.miniButton, options);
    }

    /// <summary>
    /// Draw a searchfield
    /// </summary>
    /// <param name="searchStr"></param>
    /// <param name="options">GUILayout options</param>
    /// <returns></returns>
    public static string SearchField(string searchStr, params GUILayoutOption[] options)
    {
        searchStr = GUILayout.TextField(searchStr, "ToolbarSeachTextField", options);
        if (GUILayout.Button("", "ToolbarSeachCancelButton"))
        {
            searchStr = "";
            GUI.FocusControl(null);
        }
        return searchStr;
    }

    /// <summary>
    /// Draw a dropzone
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefabType"></param>
    /// <param name="callback"></param>
    /// <param name="style"></param>
    /// <param name="options">GUILayout options</param>
    public static void DropZone<T>(PrefabType prefabType, Action<IEnumerable<T>> callback, GUIStyle style, params GUILayoutOption[] options) where T : UnityObject
    {
        GUILayout.Box("Drop prefabs here", style, GUILayout.ExpandWidth(true), GUILayout.Height(35));

        var eventType = Event.current.type;
        var isAccepted = false;

        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                isAccepted = true;
            }
            Event.current.Use();
        }

        if (!isAccepted) 
            return;

        var objects = DragAndDrop.objectReferences
            .Where(obj => obj is T)
            .Cast<T>()
            .Where(obj => PrefabUtility.GetPrefabType(obj) == prefabType);

        if(callback != null)
            callback(objects);
    }

    /// <summary>
    /// Label with browse button
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    /// <param name="extension"></param>
    /// <param name="options">GUILayout options</param>
    /// <returns></returns>
    public static string FileLabel(string name, string path, string extension, params  GUILayoutOption[] options)
    {
        using (new HorizontalBlock(options))
        {
            GUILayout.Label(name, GUILayout.ExpandWidth(true));
            var filepath = EditorGUILayout.TextField(path);

            if (GUILayout.Button("Browse", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                filepath = EditorUtility.OpenFilePanel(name, path, extension);

            return filepath;
        }
    }

    /// <summary>
    /// Textfield woth browse button
    /// </summary>
    /// <param name="name"></param>
    /// <param name="labelWidth"></param>
    /// <param name="path"></param>
    /// <param name="options">GUILayout options</param>
    /// <returns></returns>
    public static string FolderLabel(string name, float labelWidth, string path,  params  GUILayoutOption[] options)
    {
        using (new HorizontalBlock(options))
        {
            var filepath = EditorGUILayout.TextField(name, path, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
            {
                filepath = EditorUtility.SaveFolderPanel(name, path, "Folder");
            }
            return filepath;
        }
    }
}
