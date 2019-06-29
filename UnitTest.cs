using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralUnitTest
{

    /// <summary>
    /// https://www.hackerrank.com/challenges/magic-square-forming/problem
    /// </summary>
    [TestClass]
    public class UnitTest
    {

        public const int MAGIC_NUMBER = 15;

        /// <summary>
        /// matrix, expected result
        /// </summary>
        private static List<(int[][] matrix, int result)> _inputMatrices = new List<(int[][] matrix, int result)>
        {
            (new[] { new[] { 4, 9, 2 },
                     new[] { 3, 5, 7 },
                     new[] { 8, 1, 5 } }, 1), // bottom right cell increased by 1
            (new[] { new[] { 5, 3, 4 }, // 5 -> 8
                     new[] { 1, 5, 8 }, // 8 -> 9
                     new[] { 6, 4, 2 } }, 7), // 4 -> 7
            (new[] { new[] { 4, 8, 2 }, // 8 -> 9
                     new[] { 4, 5, 7 }, // 4 -> 3
                     new[] { 6, 1, 6 } }, 4), // 6 -> 8
        };

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void MagicSquare(int testMatrixIndex)
        {
            var result = formingMagicSquare(_inputMatrices[testMatrixIndex].matrix);
            Assert.AreEqual(_inputMatrices[testMatrixIndex].result, result);
        }

       /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static int formingMagicSquare(int[][] s)
        {
            return GetMagicSquare(new Matrix(s), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentMatrix"></param>
        /// <param name="result">The sum of value changes made to individual cells required to complete the puzzle.</param>
        /// <returns></returns>
        static int GetMagicSquare(Matrix currentMatrix, int result)
        {
            ReplaceLine missingNumberReplacement = MissingNumberReplacement(currentMatrix);
            ReplaceLine duplicateReplacement = currentMatrix.GetSubstituteForNextDuplicate();

            (Matrix matrix, int result) nextCallFromMissingNumber = (currentMatrix.Clone(missingNumberReplacement.LineRef, missingNumberReplacement.ReplacementValue), result + missingNumberReplacement.AdjustValue);
            (Matrix matrix, int result) nextCallFromDuplicate = (currentMatrix.Clone(duplicateReplacement.LineRef, duplicateReplacement.ReplacementValue), result + duplicateReplacement.AdjustValue);

            /// Find the lesser replacement of the 2 techniques.  Tie goes to duplicate technique.
            if ((missingNumberReplacement.LineRef != null) && (duplicateReplacement.LineRef != null))
            {
                if (missingNumberReplacement.AdjustValue < duplicateReplacement.AdjustValue)
                {
                    return GetMagicSquare(nextCallFromMissingNumber.matrix, nextCallFromMissingNumber.result);
                } else if (duplicateReplacement.AdjustValue > missingNumberReplacement.AdjustValue)
                {
                    return GetMagicSquare(nextCallFromDuplicate.matrix, nextCallFromDuplicate.result);
                }
                else
                {
                    return GetMagicSquare(nextCallFromDuplicate.matrix, nextCallFromDuplicate.result);
                }
            } else if (missingNumberReplacement.LineRef != null)
            {
                return GetMagicSquare(nextCallFromMissingNumber.matrix, nextCallFromMissingNumber.result);
            }
            else if (duplicateReplacement.LineRef != null)
            {
                return GetMagicSquare(nextCallFromDuplicate.matrix, nextCallFromDuplicate.result);
            }
            else
            {
                // If neither method return anything, we're done.
                return result;
            }
        }

        private static ReplaceLine MissingNumberReplacement(Matrix currentMatrix)
        {
            ReplaceLine result = default;
            int? nextMissingNumber = currentMatrix.NextMissingNumber();
            if (nextMissingNumber.HasValue)
            {
                return currentMatrix.GetSubstituteForMissingFromHorzLines(nextMissingNumber.Value);
            }
            return result;
        }

    }

}