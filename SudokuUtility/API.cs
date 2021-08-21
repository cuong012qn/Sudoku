

namespace SudokuUtility
{
    using SudokuUtility.Models;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Api
    {
        public Api()
        {

        }

        public async Task<Sudoku> GetJson(Level level)
        {
            var url = $"https://sudoku.com/api/getLevel/{level.Name.ToLower()}";
            string result;
            using (var hc = new HttpClient())
            {
                result = await hc.GetStringAsync(url);
            }
            Sudoku su = JsonConvert.DeserializeObject<Sudoku>(result);
            return su;
        }
    }
}
