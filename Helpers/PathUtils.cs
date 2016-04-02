using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

namespace UnityHelpers
{
    public static class PathUtils
    {
        /// <summary>
        /// Convert global path to relative
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GlobalPathToRelative(string path)
        {
            if (path.StartsWith(Application.dataPath))
                return "Assets" + path.Substring(Application.dataPath.Length);
            else
                throw new ArgumentException("Incorrect path. PAth doed not contain Application.datapath");
        }

        /// <summary>
        /// Convert relative path to global
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RelativePathToGlobal(string path)
        {
            return Path.Combine(Application.dataPath, path);
        }

        /// <summary>
        /// Convert path from unix style to windows
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string UnixToWindowsPath(string path)
        {
            return path.Replace("/", "\\");
        }

        /// <summary>
        /// Convert path from windows style to unix
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string WindowsToUnixPath(string path)
        {
            return path.Replace("\\", "/");
        }
    } 
}
