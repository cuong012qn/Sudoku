using SudokuUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuUtility.Interfaces
{
    public interface IHighScore
    {
        void WriteScore(HighScoreModel highScore);

        List<HighScoreModel> ReadScore();

        string CreateOrEditFile();
    }
}
