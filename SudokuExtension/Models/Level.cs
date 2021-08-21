using System.IO.Pipes;

namespace SudokuExtension.Models
{
    public class Level
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Index {Index}\nName {Name}";
        }

        public enum LevelConstants
        {
            Easy,
            Medium,
            Hard,
            Expert
        }
    }
}
