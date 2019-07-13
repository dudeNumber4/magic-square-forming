using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GeneralUnitTest
{

    /// <summary>
    /// https://www.hackerrank.com/challenges/magic-square-forming/problem
    /// </summary>
    [TestClass]
    public class UnitTest
    {

        private static int MagicNumber;
        private static int CenterNumber;

        private static int[][] _testArray;
        private static int[][] _solution;

        /// <summary>
        /// matrix, expected result
        /// </summary>
        private static List<Tuple<int[][], int>> _inputMatrices = new List<Tuple<int[][], int>>
        {
            //new Tuple<int[][], int>( new int[][] { new int[] {4,9,2},
            //                                       new int[] {3,5,7},
            //                                       new int[] {8,1,5}}, 1), // bottom right cell increased by 1
            //new Tuple<int[][], int>( new int[][] { new int[] {5,3,4}, // 5 -> 8 (cost 3)
            //                                       new int[] {1,5,8}, // 8 -> 9 (cost 1)
            //                                       new int[] {6,4,2}}, 7), // 4 -> 7 (cost 3)
            //new Tuple<int[][], int>( new int[][] { new int[] {4,8,2}, // 8 -> 9
            //                                       new int[] {4,5,7}, // 4 -> 3
            //                                       new int[] {6,1,6}}, 4), // 6 -> 8
            //new Tuple<int[][], int>( new int[][] { new int[] {4,5,8},
            //                                       new int[] {2,4,1},
            //                                       new int[] {1,9,7}}, 14),
            //new Tuple<int[][], int>( new int[][] { new int[] {2,9,8},
            //                                       new int[] {4,2,7},
            //                                       new int[] {5,6,7}}, 21),
            new Tuple<int[][], int>( new int[][] { new int[] {3,1,8},
                                                   new int[] {2,5,8},
                                                   new int[] {1,9,7}}, int.MaxValue), // an intermediate state from another matrix
        };

        [TestMethod]
        [DataRow(0)]
        //[DataRow(1)]
        //[DataRow(2)]
        //[DataRow(3)]
        public void UnitTest1(int testMatrixIndex)
        {
            var result = formingMagicSquare(_inputMatrices[testMatrixIndex].Item1);
            Assert.AreEqual(_inputMatrices[testMatrixIndex].Item2, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static int formingMagicSquare(int[][] s)
        {
            Initialize(s);
            return Calc();
        }

        private static int Calc()
        {
            var sums = new List<int>(4);
            foreach (var i in Enumerable.Range(1, 4))
            {
                PrintDebug();
                int cost = CostDifference(_testArray);
                sums.Add(cost);
                Debug.Print($"Cost: {cost}");
                _solution = RotateArray(_solution);
            }
            return sums.Min();
        }

        static private void Initialize(int[][] s)
        {
            _testArray = s;
            IEnumerable<int> validNumbers = Enumerable.Range(1, 9);
            MagicNumber = validNumbers.Sum() / s.Length;
            CenterNumber = validNumbers.Count() / 2 + 1;
            _solution = new int[][]{ new int[] {CenterNumber + 1,                                     MagicNumber - (CenterNumber + 1 + CenterNumber + 3),     CenterNumber + 3},
                                     new int[] { MagicNumber - (CenterNumber + 1 + CenterNumber - 3), CenterNumber,                                            MagicNumber - (CenterNumber + 3 + CenterNumber - 1)},
                                     new int[] {CenterNumber - 3,                                     MagicNumber - ((CenterNumber - 3) + (CenterNumber - 1)), CenterNumber - 1}};
        }

        static private void PrintDebug()
        {
            Debug.Print("--------");
            Debug.Print($"{_testArray[0][0]}|{_testArray[0][1]}|{_testArray[0][2]} - {_solution[0][0]}|{_solution[0][1]}|{_solution[0][2]}");
            Debug.Print($"{_testArray[1][0]}|{_testArray[1][1]}|{_testArray[1][2]} - {_solution[1][0]}|{_solution[1][1]}|{_solution[1][2]}");
            Debug.Print($"{_testArray[2][0]}|{_testArray[2][1]}|{_testArray[2][2]} - {_solution[2][0]}|{_solution[2][1]}|{_solution[2][2]}");
        }

        /// <summary>
        /// Return a new matrix rotated 90 to the left
        /// </summary>
        /// <returns></returns>
        private static int[][] RotateArray(int[][] arr)
        {
            return new int[][] { new int[] {arr[0][2], arr[1][2], arr[2][2]},
                                 new int[] {arr[0][1], arr[1][1], arr[2][1]},
                                 new int[] {arr[0][0], arr[1][0], arr[2][0]} };
        }

        private static int CostDifference(int[][] other)
        {
            return Math.Abs(_solution[0][0] - other[0][0]) +
                   Math.Abs(_solution[0][1] - other[0][1]) +
                   Math.Abs(_solution[0][2] - other[0][2]) +
                   Math.Abs(_solution[1][0] - other[1][0]) +
                   Math.Abs(_solution[1][1] - other[1][1]) +
                   Math.Abs(_solution[1][2] - other[1][2]) +
                   Math.Abs(_solution[2][0] - other[2][0]) +
                   Math.Abs(_solution[2][1] - other[2][1]) +
                   Math.Abs(_solution[2][2] - other[2][2]);
        }

    }

}