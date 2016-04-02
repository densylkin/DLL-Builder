using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace UnityHelpers
{
    public static class RandomHelpers
    {
        private static readonly Random rnd = new Random((int)DateTime.Now.Ticks);

        #region Random values

        /// <summary>
        /// Get random integer value between min and max
        /// </summary>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max)
        {
            return rnd.Next(min, max);
        }

        /// <summary>
        /// Get random integer value between 0 and max
        /// </summary>
        /// <param name="max">Maximum</param>
        /// <returns></returns>
        public static int GetRandomInt(int max)
        {
            return GetRandomInt(0, max);
        }

        /// <summary>
        /// Get random float value between min and max
        /// </summary>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns></returns>
        public static float GetRandomFloat(float min, float max)
        {
            return (float)rnd.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Get random float value between 0 and max
        /// </summary>
        /// <param name="max">Maximum</param>
        /// <returns></returns>
        public static float GetRandomFloat(float max)
        {
            return GetRandomFloat(0f, max);
        }

        /// <summary>
        /// Get random Vector2 with components between min and max
        /// </summary>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns></returns>
        public static Vector2 GetRandomVector2(float min, float max)
        {
            return new Vector2(
                GetRandomFloat(min, max),
                GetRandomFloat(min, max)
                );
        }

        /// <summary>
        /// Get random normalized Vector2
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetRandomVector2()
        {
            return GetRandomVector2(0, 1);
        }

        /// <summary>
        /// Get random Vector3 with components between min and max
        /// </summary>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns></returns>
        public static Vector3 GetRandomVector3(float min, float max)
        {
            return new Vector3(
                GetRandomFloat(min, max),
                GetRandomFloat(min, max),
                GetRandomFloat(min, max)
                );
        }

        /// <summary>
        /// Get random normalized Vector3
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomVector3()
        {
            return GetRandomVector3(0, 1);
        }

        /// <summary>
        /// Get random Color with specofied alpha
        /// </summary>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color GetRandomColor(float alpha)
        {
            return new Color(
                GetRandomFloat(0, 1),
                GetRandomFloat(0, 1),
                GetRandomFloat(0, 1),
                alpha
                );
        }

        /// <summary>
        /// Get random opaque color
        /// </summary>
        /// <returns></returns>
        public static Color GetRandomColor()
        {
            return GetRandomColor(1);
        }

        #endregion

        #region Random sequnces

        /// <summary>
        /// Get sequnce of random numbers
        /// </summary>
        /// <param name="count"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] GetRandomNumbers(int count, int min, int max)
        {
            return GetRandomNumbersExcepts(count, min, max, new int[] { });
        }

        /// <summary>
        /// Get sequence of random numbers excepts excludedNumbers
        /// </summary>
        /// <param name="count"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="excludedNumbers"></param>
        /// <returns></returns>
        public static int[] GetRandomNumbersExcepts(int count, int min, int max, int[] excludedNumbers)
        {
            if (max <= min || count < 0 || (count > max - min && max - min > 0))
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max + " (" + ((Int64)max - (Int64)min) +
                                                      " values), or count " + count + " is illegal");

            var candidates = new HashSet<int>();

            for (var top = max - count; top < max; top++)
            {
                var rn = rnd.Next(min, top + 1);
                if (excludedNumbers.Contains(rn) || !candidates.Add(rnd.Next(min, top + 1)))
                    candidates.Add(top);
            }

            var result = new int[count];
            candidates.CopyTo(result);
            result.Shuffle();
            return result;

        }

        #endregion

        #region Collections extentions

        /// <summary>
        /// Shuffle list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                var k = (rnd.Next(0, n) % n);
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Get random object from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Target listn</param>
        /// <returns></returns>
        public static T GetRandomObject<T>(this IList<T> list)
        {
            return list[rnd.Next(list.Count)];
        }

        #endregion
    }
    
}