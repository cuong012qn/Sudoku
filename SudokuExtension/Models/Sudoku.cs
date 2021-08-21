namespace SudokuExtension.Models
{
    public class Sudoku
    {
        public string Answer { get; set; }
        public string Message { get; set; }
        public string[] Desc { get; set; }

        public string GetQuestion() => Desc[0];
        public string GetAnswer => Desc[1];
    }
}