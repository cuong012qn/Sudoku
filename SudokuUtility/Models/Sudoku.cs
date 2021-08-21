using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuUtility.Models
{
    public class Sudoku
    {
        public string Answer { get; set; }
        public string Message { get; set; }
        public string[] Desc { get; set; }

        public List<List<int>> GetQuestion => GetMatrixFromString(Desc[0]);
        public List<List<int>> GetAnswer => GetMatrixFromString(Desc[1]);

        private List<List<int>> GetMatrixFromString(string text)
        {
            List<List<int>> result = new List<List<int>>();
            List<int> num = new List<int>();
            foreach (var t in text)
            {
                num.Add((t - '0'));
                if (num.Count.Equals(9))
                {
                    List<int> templist = new List<int>(num);
                    result.Add(templist);
                    num.Clear();
                }
            }
            return result;
        }
    }
}
