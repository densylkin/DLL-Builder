using UnityEngine;
using System.IO;

namespace UnityHelpers
{
#if UNITY_EDITOR
    using UnityEditor;

    public static class ScriptableObjectUtils
    {
        /// <summary>
        /// Create asset of Type T at pat
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        /// <summary>
        /// Create asset of type T at current project window location
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T CreateAssetProjectWindow<T>(string filename) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, filename + ".asset");
            return asset;
        }

        /// <summary>
        /// Load asset of Type T at pat
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string path) where T : ScriptableObject
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        /// <summary>
        /// Load or create asset of Type T at path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            var temp = LoadAsset<T>(path);
            return temp ?? CreateAsset<T>(path);
        }

        /// <summary>
        /// Delete Asset at path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DeleteAsset(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists)
                return false;

            file.Delete();
            return true;
        }
    }

#endif
}