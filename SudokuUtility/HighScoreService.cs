using SudokuUtility.Interfaces;
using SudokuUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SudokuUtility
{
    public class HighScoreService : IHighScore
    {
        public string CreateOrEditFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "score.txt");

            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                File.Create(path);
                return path;
            }
        }

        public List<HighScoreModel> ReadScore()
        {
            string path = CreateOrEditFile();

            var data = File.ReadAllText(path);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HighScoreModel>>(data);
        }

        public void WriteScore(HighScoreModel highScore)
        {
            var currScore = ReadScore();

            if (currScore == null)
                currScore = new List<HighScoreModel>();

            currScore.Add(highScore);

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(currScore);

            File.WriteAllText(CreateOrEditFile(), data);
        }
    }
}
