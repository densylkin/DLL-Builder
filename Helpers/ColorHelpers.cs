using UnityEngine;
using System.Collections;

namespace UnityHelpers
{
    public static class ColorHelpers
    {
        /// <summary>
        /// Get random GUIStyle with colored background
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle MakeBackgroudnStyle(Color color)
        {
            return new GUIStyle { normal = { background = MakeTex(color) } };
        }

        /// <summary>
        /// Creates 1*1 texture with specified color in hex
        /// </summary>
        /// <param name="colorHex"></param>
        public static void MakeTex(string colorHex)
        {
            MakeTex(HexToColor(colorHex));
        }

        /// <summary>
        /// Creates 1*1 texture with specified color
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Texture2D MakeTex(Color col)
        {
            var pix = new Color[1 * 1];

            for (var i = 0; i < pix.Length; i++)
                pix[i] = col;

            var result = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            result.hideFlags = HideFlags.HideAndDontSave;
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        /// <summary>
        /// Convert color to hex string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(this Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }

        /// <summary>
        /// Convert hex string to color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
    
}