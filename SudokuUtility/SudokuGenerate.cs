
namespace SudokuUtility
{
    using SudokuUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public class SudokuGenerate
    {
        //private static int countTry = 0;
        //private readonly CancellationTokenSource cancelToken = new CancellationTokenSource();
        //private readonly Stopwatch _stopwatch;
        // private Task _Task;
        private static readonly Random Random = new Random();
        private static readonly object Synclock = new object();
        private List<List<int>> Matrix = new List<List<int>>();
        public List<List<int>> Question { get; }
        public List<List<int>> Answer { get; set; }

        public SudokuGenerate(Level level)
        {
            //Easy 40 - 47
            //Medium 47 - 50
            //Hard 53 - 55
            //Expert 57 - 59
            int diff = level.Difficult();
            if (!diff.Equals(0))
            {
               
                CreateMatrix(ref this.Matrix);
                Task task = Task.Run(() => { FillMatrix(ref this.Matrix); });
                task.Wait();
                this.Answer = this.Matrix;
                RemoveDigit(diff, this.Matrix);
                this.Question = this.Matrix;
                //MessageBox.Show(countTry.ToString());
            }
        }

        public SudokuGenerate(Level level, string Solved)
        {
            if (!string.IsNullOrEmpty(Solved))
            {
                this.Matrix = new List<List<int>>(StringToMatrix(Solved));
                int diff = level.Difficult();
                if (diff != 0)
                {
                    this.Answer = new List<List<int>>();
                    for (int i = 0; i < 9; i++)
                    {
                        List<int> row = new List<int>();
                        for (int j = 0; j < 9; j++)
                        {
                            row.Add(this.Matrix[i][j]);
                        }
                        this.Answer.Add(row);
                    }
                    RemoveDigit(diff, this.Matrix);
                    this.Question = new List<List<int>>(this.Matrix);
                }

            }
            else return;
        }

        private List<List<int>> StringToMatrix(string inputSolved)
        {
            if (!string.IsNullOrEmpty(inputSolved))
            {
                List<List<int>> result = new List<List<int>>();
                string[] lines = inputSolved.Split('\n');
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] cells = line.Split(' ');
                        List<int> row = new List<int>();
                        foreach (string cell in cells)
                        {
                            if (!string.IsNullOrEmpty(cell))
                            {
                                row.Add(Convert.ToInt32(cell));
                            }
                            if (row.Count.Equals(9))
                            {
                                result.Add(row);
                                break;
                            }
                        }
                    }
                }


                return result;
            }
            else return null;
        }

        private void RemoveDigit(int countEmptyCell, List<List<int>> matrix)
        {
            while (countEmptyCell != 0)
            {
                int posCol = GetRandomPos(0, 9);
                int posRow = GetRandomPos(0, 9);
                if (!matrix[posRow][posCol].Equals(0))
                {
                    matrix[posRow][posCol] = 0;
                    countEmptyCell--;
                }
            }
        }

        public static bool IsSudoku(List<List<int>> inputMatrix)
        {
            if (inputMatrix.Count < 9) return false;
            if (inputMatrix.Any(x => x.Count < 9))
                return false;

            //row
            //if (inputMatrix.Any(x => x.Select(k => x.Distinct()).Count() < 9))
            //    return false;
            if (inputMatrix.Any(x => x.Distinct().Count() < 9))
                return false;

            // columns
            for (int i = 0; i < 9; i++)
            {
                if (inputMatrix.Select(row => row[i]).Distinct().Count() < 9)
                    return false;
                //if (inputMatrix.Select(row
                //        => row[i].Value).Distinct().Count() < 9)
                //    return false;
            }

            // sub-grids
            for (int i = 1; i <= 3; i++) //row
            {
                for (int j = 1; j <= 3; j++)//column
                {
                    if (inputMatrix.Where((_, pos) => ((pos + 3) / 3) == i)
                            .SelectMany(lst => lst.Where((_, pos) => (pos + 3) / 3 == j))
                            .Distinct().Count() < 9)
                        return false;
                    //if (inputMatrix.Where((_, pos) => ((pos + 3) / 3) == i)
                    //.SelectMany(lst => lst.Where((_, pos) => (pos + 3) / 3 == j).Select(k => k.Value)
                    //.Distinct().Count() < 9)
                    //                return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create Matrix 9x9 only 0
        /// </summary>
        /// <returns>Matrix 9x9 only 0</returns>
        private void CreateMatrix(ref List<List<int>> matrix)
        {
            if (matrix.Count.Equals(0))
                for (int i = 0; i < 9; i++)
                {
                    List<int> temp = new List<int>();
                    for (int j = 0; j < 9; j++)
                        temp.Add(0);
                    matrix.Add(temp);
                }
        }

        private void FillMatrix(ref List<List<int>> matrix)
        {
            for (int row = 0; row < 9; row++)
            {
                List<int> lstNumber = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                for (int column = 0; column < 9; column++)
                {
                    List<int> lstFillCell = ListCanFillCell(lstNumber, row, column);
                    if (lstFillCell.Count.Equals(0))
                    {
                        //int k = -1;
                        //int RemainValue = 1;
                        //bool valid = true;

                        #region OldMethod
                        //while (IsFill(row))
                        //{
                        //    if (k.Equals(0))
                        //    {
                        //        for (int i = 8; i >= 0; i--)
                        //        {
                        //            if (matrix[row][i].Equals(0))
                        //                k = i;
                        //        }
                        //    }

                        //    for (int i = 8; i >= k; i--)
                        //    {
                        //        if (!matrix[row][i].Equals(0))
                        //        {
                        //            lstNumber.Add(Matrix[row][i]);
                        //            matrix[row][i] = 0;
                        //        }
                        //    }

                        //    for (int i = k; i < 9; i++)
                        //    {
                        //        var temp = ListCanFillCell(lstNumber, row, i);
                        //        if (temp.Count > 0)
                        //        {
                        //            int pos = GetRandomPos(0, temp.Count);
                        //            matrix[row][i] = temp[pos];
                        //            lstNumber.Remove(matrix[row][i]);
                        //        }
                        //    }

                        //    if (k > 0)
                        //        k--;
                        //    else k = 0;
                        //}
                        #endregion

                        #region NewMethod
                        //Find 0 in row if not exist => exit
                        //while (matrix[row].Contains(0))
                        //{
                        //    var listSeed = lstNumber;
                        //    if (listSeed.Count == 0)
                        //    {
                        //        lastPosition--;
                        //        lstNumber.Add(matrix[row][lastPosition]);
                        //        matrix[row][lastPosition] = 0;
                        //    }
                        //    else
                        //    {
                        //        while (listSeed.Count != 0)
                        //        {
                        //            if (matrix[row][lastPosition] == 0)
                        //            {
                        //                var listfill = ListCanFillCell(listSeed, row, lastPosition);
                        //                if (listfill.Count > 0)
                        //                {
                        //                    int ranSeed = GetRandomPos(0, listfill.Count);
                        //                    matrix[row][lastPosition] = listfill[ranSeed];
                        //                    lstNumber.Remove(matrix[row][lastPosition]);
                        //                    listSeed.Remove(matrix[row][lastPosition]);
                        //                }
                        //            }

                        //            for (int i = lastPosition + 1; i < 9; i++)
                        //            {
                        //                var listFill = ListCanFillCell(lstNumber, row, i);
                        //                if (listFill.Count == 0 && i == 8)
                        //                {
                        //                    //Clean
                        //                    for (int j = 8; j >= lastPosition; j--)
                        //                    {
                        //                        if (j == lastPosition)
                        //                        {
                        //                            listSeed.Remove(matrix[row][j]);
                        //                            lstNumber.Add(matrix[row][j]);
                        //                            matrix[row][j] = 0;
                        //                        }
                        //                        else if (matrix[row][j] != 0)
                        //                        {
                        //                            lstNumber.Add(matrix[row][j]);
                        //                            matrix[row][j] = 0;
                        //                        }
                        //                    }
                        //                    break;
                        //                }
                        //                else
                        //                {
                        //                    int ranPos = GetRandomPos(0, listFill.Count);
                        //                    matrix[row][i] = listFill[ranPos];
                        //                    lstNumber.Remove(matrix[row][i]);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion

                        #region TestMethod

                        //int lastpos = column - 1;
                        //if (lastpos < 0) lastpos = 0;
                        //lstNumber.Add(matrix[row][lastpos]);
                        //matrix[row][lastpos] = 0;
                        //bool IsBack = false;

                        //List<int> listSeed = new List<int>(lstNumber);
                        //while (matrix[row].Contains(0))
                        //{
                        //    //Fill listseed
                        //    if (IsBack)
                        //    {
                        //        if (matrix[row][lastpos] != 0)
                        //        {
                        //            lstNumber.Add(matrix[row][lastpos]);
                        //            matrix[row][lastpos] = 0;
                        //            listSeed = null;
                        //            listSeed = new List<int>(lstNumber);
                        //            IsBack = false;
                        //        }
                        //    }

                        //    //Fill seed 
                        //    if (matrix[row][lastpos] == 0)
                        //    {
                        //        int ranPos = GetRandomPos(0, listSeed.Count);
                        //        var temp = ListCanFillCell(listSeed, row, lastpos);
                        //        if (temp.Count() > 0)
                        //        {
                        //            matrix[row][lastpos] = listSeed[ranPos];
                        //            listSeed.Remove(matrix[row][lastpos]);
                        //            lstNumber.Remove(matrix[row][lastpos]);
                        //        }
                        //        else
                        //        {
                        //            listSeed.Clear();
                        //        }
                        //    }
                        //    if (listSeed.Count == 0)
                        //    {
                        //        if (lastpos > 0)
                        //        {
                        //            lastpos--;
                        //            IsBack = true;
                        //        }
                        //    }
                        //    else
                        //        for (int i = lastpos + 1; i < 9; i++)
                        //        {
                        //            var temp = ListCanFillCell(lstNumber, row, i);
                        //            if (temp.Count() > 0)
                        //            {
                        //                int ranPos = GetRandomPos(0, temp.Count);
                        //                matrix[row][i] = temp[ranPos];
                        //                lstNumber.Remove(temp[ranPos]);
                        //            }
                        //            else if (temp.Count() == 0)
                        //            {
                        //                //Clean
                        //                for (int j = lastpos; j < 9; j++)
                        //                {

                        //                    if (matrix[row][j] != 0)
                        //                    {
                        //                        lstNumber.Add(matrix[row][j]);
                        //                        matrix[row][j] = 0;
                        //                    }
                        //                }
                        //                break;
                        //            }
                        //        }

                        //}
                        #endregion

                        int lastpos = column;
                        for (int i = lastpos; i >= 0; i--)
                        {
                            if (matrix[row][i] != 0)
                            {
                                lstNumber.Add(matrix[row][i]);
                                matrix[row][i] = 0;
                                lastpos = i;
                                break;
                            }
                        }
                        List<int> listSeed = new List<int>(lstNumber);

                        while (matrix[row].Contains(0))
                        {
                            for (int i = lastpos; i < 9; i++)
                            {
                                if (i == lastpos)
                                {
                                    bool flag = true;
                                    while (matrix[row][i] == 0)
                                    {
                                        var cell = ListCanFillCell(listSeed, row, i);
                                        if (cell.Count > 0)
                                        {
                                            int pos = GetRandomPos(0, listSeed.Count);
                                            matrix[row][i] = listSeed[pos];
                                            lstNumber.Remove(matrix[row][i]);
                                            listSeed.Remove(matrix[row][i]);
                                        }
                                        else if (cell.Count == 0 && listSeed.Count == 1)
                                        {
                                            lastpos--;
                                            for (int j = lastpos; j < 9; j++)
                                            {
                                                if (matrix[row][j] != 0)
                                                {
                                                    lstNumber.Add(matrix[row][j]);
                                                    matrix[row][j] = 0;
                                                    listSeed = new List<int>(lstNumber);
                                                }
                                            }
                                            flag = false;
                                        }
                                    }
                                    if (!flag) break;
                                }
                                else
                                {
                                    var cell = ListCanFillCell(lstNumber, row, i);
                                    if (cell.Count > 0)
                                    {
                                        int pos = GetRandomPos(0, cell.Count);
                                        matrix[row][i] = lstNumber[pos];
                                        lstNumber.Remove(matrix[row][i]);
                                    }
                                    else
                                    {
                                        if (listSeed.Count > 0)
                                        {
                                            for (int j = lastpos; j < 9; j++)
                                            {
                                                if (matrix[row][j] != 0)
                                                {
                                                    lstNumber.Add(matrix[row][j]);
                                                    matrix[row][j] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastpos--;
                                            for (int j = lastpos; j < 9; j++)
                                            {
                                                if (matrix[row][j] != 0)
                                                {
                                                    lstNumber.Add(matrix[row][j]);
                                                    matrix[row][j] = 0;
                                                    listSeed = new List<int>(lstNumber);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int pos = GetRandomPos(0, lstFillCell.Count);
                        matrix[row][column] = lstFillCell[pos];
                        lstNumber.Remove(matrix[row][column]);
                    }
                }
            }
            //MessageBox.Show("Done");
            string s = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    s += matrix[i][j].ToString() + " ";
                }
                s += "\n";
            }
            MessageBox.Show($"{s}\n{IsSudoku(matrix).ToString()}");
        }

        //Remove duplicate number in row
        private void RemoveDuplicateNum(int posRow)
        {

        }

        /// <summary>
        /// Check in current row exists 0 in Matrix
        /// </summary>
        /// <param name="posRow">Input current row</param>
        /// <returns>True if has 0 in row, otherwise return false</returns>
        private bool IsFill(int posRow)
        {
            return Matrix[posRow].Any(x => x == 0);
        }

        private List<int> ListCanFillCell(List<int> inputList, int posRow, int posCol)
        {
            List<int> result = new List<int>(inputList);
            //Check value on row remove if exists
            for (int i = posRow; i >= 0; i--)
            {
                if (result.Contains(Matrix[i][posCol]))
                    result.Remove(Matrix[i][posCol]);
            }
            //Check value on box remove if exists
            var box = GetsubMatrix(posCol, posRow);
            foreach (var t in box)
            {
                for (int j = 0; j < box.Count; j++)
                {
                    if (result.Contains(t[j]))
                        result.Remove(t[j]);
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
        private List<List<int>> GetsubMatrix(int posCol, int posRow)
        {
            var result = new List<List<int>>();
            int col = -1, row = -1;
            for (int i = 2; i <= 8; i += 3)
            {
                col = posCol <= i && col.Equals(-1) ? (i + 1) / 3 : col;
                row = posRow <= i && row.Equals(-1) ? (i + 1) / 3 : row;
                if (!col.Equals(-1) && !row.Equals(-1)) break;
            }

            for (int i = (row * 3) - 3; i < (row * 3); i++)
            {
                List<int> tempList = new List<int>();
                for (int j = (col * 3) - 3; j < (col * 3); j++)
                {
                    tempList.Add(Matrix[i][j]);
                }
                result.Add(tempList);
            }
            return result;
        }
    }
}
