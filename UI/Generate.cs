
namespace UI
{
    using SudokuUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Generate
    {
        private static readonly Random Random = new Random();
        private static readonly object Synclock = new object();
        private List<List<Number>> Matrix { get; set; }
        public List<List<Number>> SudokuAnswer { get; private set; }
        public List<List<Number>> SudokuQues { get; private set; }


        public Generate(Level level)
        {
            //Easy 40 - 47
            //Medium 47 - 50
            //Hard 53 - 55
            //Expert 57 - 59
            LevelConstants lv;
            int diff = 0;
            Enum.TryParse(level.Name, out lv);
            switch (lv)
            {
                case LevelConstants.Easy:
                    diff = GetRandomPos(44, 47);
                    break;
                case LevelConstants.Medium:
                    diff = GetRandomPos(47, 50);
                    break;
                case LevelConstants.Hard:
                    diff = GetRandomPos(50, 55);
                    break;
                case LevelConstants.Expert:
                    diff = GetRandomPos(55, 59);
                    break;
            }
            if (!diff.Equals(0))
            {
                this.Matrix = CreateMatrix();
                FillMatrix();
                RemoveDigit(diff);
            }
        }

        private void RemoveDigit(int countEmptyCell)
        {

            while (countEmptyCell != 0)
            {
                int posCol = GetRandomPos(0, 9);
                int posRow = GetRandomPos(0, 9);
                if (!Matrix[posRow][posCol].Value.Equals(0))
                {
                    Matrix[posRow][posCol].Value = 0;
                    Matrix[posRow][posCol].CanEdit = true;
                    countEmptyCell--;
                }
            }

            SudokuQues = Matrix;
        }

        public static bool IsSudoku(List<List<Number>> inputMatrix)
        {
            if (inputMatrix.Count < 9) return false;
            if (inputMatrix.Any(x => x.Count < 9))
                return false;

            //row
            if (inputMatrix.Any(x => x.Select(k => x.Distinct()).Count() < 9))
                return false;

            // columns
            for (int i = 0; i < 9; i++)
            {
                if (inputMatrix.Select(row
                        => row[i].Value).Distinct().Count() < 9)
                    return false;
            }

            // sub-grids
            for (int i = 1; i <= 3; i++) //row
            {
                for (int j = 1; j <= 3; j++)//column
                {
                    if (inputMatrix.Where((_, pos) => ((pos + 3) / 3) == i)
                            .SelectMany(lst => lst.Where((_, pos) => (pos + 3) / 3 == j).Select(k => k.Value))
                            .Distinct().Count() < 9)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create Matrix 9x9 only 0
        /// </summary>
        /// <returns>Matrix 9x9 only 0</returns>
        private List<List<Number>> CreateMatrix()
        {
            List<List<Number>> result = new List<List<Number>>();
            for (int i = 0; i < 9; i++)
            {
                List<Number> temp = new List<Number>();
                for (int j = 0; j < 9; j++)
                    temp.Add(new Number() { Value = 0, CanEdit = false });
                result.Add(temp);
            }
            return result;
        }

        private void FillMatrix()
        {
            for (int row = 0; row < 9; row++)
            {
                List<int> lstNumber = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                for (int column = 0; column < 9; column++)
                {
                    List<int> lstFillCell = ListCanFillCell(lstNumber, row, column);
                    if (lstFillCell.Count.Equals(0))
                    {
                        int k = 0;
                        while (IsFill(row))
                        {
                            if (k.Equals(0))
                            {
                                for (int i = 8; i >= 0; i--)
                                {
                                    if (Matrix[row][i].Value.Equals(0))
                                        k = i;
                                }
                            }

                            for (int i = 8; i >= k; i--)
                            {
                                if (!Matrix[row][i].Value.Equals(0))
                                {
                                    lstNumber.Add(Matrix[row][i].Value);
                                    Matrix[row][i].Value = 0;
                                }
                            }

                            for (int i = k; i < 9; i++)
                            {
                                var temp = ListCanFillCell(lstNumber, row, i);
                                if (temp.Count > 0)
                                {
                                    int pos = GetRandomPos(0, temp.Count);
                                    Matrix[row][i].Value = temp[pos];
                                    lstNumber.Remove(Matrix[row][i].Value);
                                }
                            }

                            if (k > 0)
                                k--;
                            else k = 0;
                        }
                    }
                    else
                    {
                        int pos = GetRandomPos(0, lstFillCell.Count);
                        Matrix[row][column].Value = lstFillCell[pos];
                        lstNumber.Remove(Matrix[row][column].Value);
                    }
                }
            }

            SudokuAnswer = Matrix;
        }

        /// <summary>
        /// Check in current row exists 0 in Matrix
        /// </summary>
        /// <param name="posRow">Input current row</param>
        /// <returns>True if has 0 in row, otherwise return false</returns>
        private bool IsFill(int posRow)
        {
            return Matrix[posRow].Any(x => x.Value.Equals(0));
        }

        private List<int> ListCanFillCell(List<int> inputList, int posRow, int posCol)
        {
            List<int> result = new List<int>(inputList);
            //Check value on row remove if exists
            for (int i = posRow; i >= 0; i--)
            {
                if (result.Contains(Matrix[i][posCol].Value))
                    result.Remove(Matrix[i][posCol].Value);
            }
            //Check value on box remove if exists
            var box = GetsubMatrix(posCol, posRow);
            foreach (var t in box)
            {
                for (int j = 0; j < box.Count; j++)
                {
                    if (result.Contains(t[j].Value))
                        result.Remove(t[j].Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Get random number from min to max
        /// </summary>
        /// <param name="min">Input min value</param>
        /// <param name="max">Input max value</param>
        /// <returns>Random number</returns>
        private int GetRandomPos(int min, int max)
        {
            lock (Synclock)
            {
                return Random.Next(min, max);
            }
        }

        /// <summary>
        /// Get matrix 3x3 from position column and position row
        /// </summary>
        /// <param name="posCol">Position column</param>
        /// <param name="posRow">Position row</param>
        /// <returns>Matrix 3x3</returns>
        private List<List<Number>> GetsubMatrix(int posCol, int posRow)
        {
            var result = new List<List<Number>>();
            int col = -1, row = -1;
            for (int i = 2; i <= 8; i += 3)
            {
                col = posCol <= i && col.Equals(-1) ? (i + 1) / 3 : col;
                row = posRow <= i && row.Equals(-1) ? (i + 1) / 3 : row;
                if (!col.Equals(-1) && !row.Equals(-1)) break;
            }

            for (int i = (row * 3) - 3; i < (row * 3); i++)
            {
                List<Number> tempList = new List<Number>();
                for (int j = (col * 3) - 3; j < (col * 3); j++)
                {
                    tempList.Add(Matrix[i][j]);
                }
                result.Add(tempList);
            }
            return result;
        }

        private List<List<Number>> FillBox()
        {
            List<int> number = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<List<Number>> result = new List<List<Number>>();
            for (int i = 0; i < 3; i++)
            {
                List<Number> temp = new List<Number>();
                for (int j = 0; j < 3; j++)
                {
                    int pos = GetRandomPos(0, number.Count);
                    temp.Add(new Number()
                    {
                        Value = number[pos],
                        CanEdit = true
                    });
                    number.RemoveAt(pos);
                }
                result.Add(temp);
            }
            return result;
        }
    }
}
