namespace UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Algorithm
    {
        private List<List<int>> _matrix;
        public List<List<int>> Matrix { get => _matrix; set => _matrix = value; }
        private readonly List<int> Number
            = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public Algorithm()
        {

        }

        public Algorithm(List<List<int>> Matrix)
        {
            this.Matrix = Matrix;
        }

        public List<List<int>> SolveSudoku()
        {
            List<List<int>> result = new List<List<int>>();
            if (Matrix.Equals(null)) return null;
            int row = -1;
            int col = -1;
            bool isEmpty = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Matrix[i][j].Equals(0))
                    {
                        row = i; col = j;
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                    break;
            }
            if (isEmpty)
                return result;

            for (int num = 1; num <= 9; num++)
            {
                List<int> Lstrow = Matrix[row];
                List<int> Lstcolumn = GetListColumn(Matrix, col);
                if (!isAvaiRow(Lstrow, num) &&
                    !isAvaiColumn(Lstcolumn, num))
                {

                }
                //if (isSafe(, row, col, num))
                //{
                //    board[row, col] = num;
                //    if (solveSudoku(board, n))
                //    {
                //        // print(board, n); 
                //        return true;
                //    }
                //    else
                //    {
                //        board[row, col] = 0; // replace it 
                //    }
                //}
            }
            return result;
        }


        /// <summary>
        /// Matrix 9x9 to 3x3
        /// </summary>
        /// <param name="Matrix">Matrix 9x9</param>
        /// <param name="PosCol">Position Column</param>
        /// <param name="PosRow">Position Row</param>
        /// <returns>Matrix 3x3</returns>
        public static List<List<int>> GetBoxMatrix(List<List<int>> Matrix,
                                                    int PosCol, int PosRow)
        {
            List<List<int>> result = new List<List<int>>();
            int col = -1, row = -1;
            for (int i = 2; i <= 8; i += 3)
            {
                col = PosCol <= i && col.Equals(-1) ? (i + 1) / 3 : col;
                row = PosRow <= i && row.Equals(-1) ? (i + 1) / 3 : row;
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

        private List<int> GetListRow(List<List<int>> Matrix, int Pos)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                result.Add(Matrix[Pos][i]);
            }
            return result;
        }

        private List<int> GetListColumn(List<List<int>> Matrix, int Pos)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                result.Add(Matrix[i][Pos]);
            }
            return result;
        }

        private bool isAvaiRow(List<int> row, int value)
        {
            return row.Contains(value);
        }

        private bool isAvaiColumn(List<int> column, int value)
        {
            return column.Contains(value);
        }

        private bool isAvaiMatrix(List<List<int>> matrix, int value)
        {
            return matrix.Any(i => i.Contains(value));
        }
    }
}
