using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuUtility.Models
{
    public class HighScoreModel
    {
        public LevelConstants Level { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan CompleteTime { get; set; }

        public override string ToString()
        {
            return $"{Level} - Completion time: {CompleteTime.TotalSeconds} - Create time: {DateTime.ToShortDateString()}";
        }
    }
}
