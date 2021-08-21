
namespace SudokuUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public class ControlColor
    {
        private static BrushConverter _Converter = new BrushConverter();

        public static Brush BrushMarkTextbox = (Brush)_Converter.ConvertFromString("#D9D3D2");

        public static SolidColorBrush BrushWrongTextbox = Brushes.PaleVioletRed;
    }
}
