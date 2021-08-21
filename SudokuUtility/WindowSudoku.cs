namespace SudokuUtility
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using System.Windows.Media;
    using MaterialDesignThemes.Wpf;
    using System.Threading.Tasks;

    public class WindowSudoku : IDisposable
    {
        private static TextBox tmpTextBox = new TextBox();
        public static StackPanel GridSudoku { get; set; }
        public static bool isChecking { get; set; } = false;
        private static int _PosRowGotFocus = -1;
        private static int _PosColumnGotFocus = -1;

        public static bool SetSudoku(List<List<int>> inputSudoku, StackPanel targetPanel)
        {
            if (inputSudoku.Count > 9 || inputSudoku[0].Count > 9) return false;
            try
            {
                int i = 0;
                foreach (StackPanel grid in targetPanel.Children)
                {
                    int j = 0;
                    foreach (UIElement tb in grid.Children)
                    {
                        if (tb != null && tb is TextBox)
                        {
                            //((TextBox)tb).SelectionBrush = null;
                            //((TextBox)tb).Background = Brushes.Transparent;
                            if (!inputSudoku[i][j].Equals(0))
                            {
                                //#FFB9B9B9
                                var textbox = (tb as TextBox);
                                textbox.Text = inputSudoku[i][j].ToString();
                                textbox.FontWeight = FontWeights.Bold;
                                textbox.Foreground = Brushes.Black;
                                textbox.IsReadOnly = true;
                            }
                            else
                            {
                                var textbox = tb as TextBox;
                                textbox.Text = string.Empty;
                                textbox.FontWeight = FontWeights.Normal;
                                textbox.Foreground = Brushes.Blue;
                                textbox.IsReadOnly = false;
                            }
                        }
                        j++;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public static List<List<int>> GetSudoku(StackPanel grids)
        {
            List<List<int>> sudoku = new List<List<int>>();
            if (grids != null)
            {
                foreach (StackPanel grid in grids.Children)
                {
                    List<int> row = new List<int>();
                    foreach (UIElement tb in grid.Children)
                    {
                        if (tb != null && tb is TextBox)
                            row.Add(Convert.ToInt32((tb as TextBox).Text));
                    }
                    sudoku.Add(row);
                }
            }
            return sudoku;
        }

        public static bool SetEvent(StackPanel Grid)
        {
            if (Grid.Children.Count != 9) return false;
            if (((StackPanel)Grid.Children[0]).Children.Count != 9) return false;

            try
            {
                foreach (StackPanel grid in Grid.Children)
                {
                    foreach (UIElement tb in grid.Children)
                    {
                        var textbox = (TextBox)tb;
                        textbox.Background = Brushes.Transparent;
                        textbox.GotFocus += TextBox_GotFocus;
                        textbox.LostFocus += TextBox_LostFocus;
                        textbox.PreviewTextInput += TextBox_PreviewTextInput;
                        textbox.MouseDoubleClick += TextBox_MouseDoubleClick;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #region Event
        private static void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Text = (!textbox.IsReadOnly) ? string.Empty : textbox.Text;
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (sender as TextBox);
            tb.Text = tmpTextBox.Text;

            //Textbox lostfocus remove color
            MarkBoxTextbox(tb, true);
            MarkColumnTextbox(tb, true);
            MarkRowTextbox(tb, true);

            //Set color text
            if (tb.IsReadOnly) tb.Foreground = Brushes.Black;
            else tb.Foreground = Brushes.Blue;
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (sender as TextBox);

            if (isChecking)
                RemoveBackgroundTextbox(tb);

            tmpTextBox = tb;

            MarkRowTextbox(tb, false);
            MarkColumnTextbox(tb, false);
            MarkBoxTextbox(tb, false);

            tb.Background = Brushes.DimGray;
            tb.Foreground = Brushes.White;


            //MessageBox.Show($"Row {PosRowGotFocus} Column {PosColumnGotFocus}");
            //MessageBox.Show(GetParent((TextBox)sender).Name);
        }

        public static void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tmpTextBox.Background != null && !tmpTextBox.IsReadOnly)
                if (tmpTextBox.Background.Equals(Brushes.Transparent))
                    tmpTextBox.Text = ((Button)sender).Content.ToString();
        }

        #endregion

        #region MarkTextbox

        private static void MarkRowTextbox(TextBox tb, bool unMark = true)
        {
            //Mark Brushes.DeepSkyBlue
            //Unmark Brushes.Transparent
            var brushes = unMark ? Brushes.Transparent : ControlColor.BrushMarkTextbox;
            StackPanel rows = tb.Parent as StackPanel;
            foreach (TextBox textbox in rows.Children)
            {
                textbox.Background = brushes;
            }
        }

        private static void MarkColumnTextbox(TextBox tb, bool unMark = true)
        {
            var brushes = unMark ? Brushes.Transparent : ControlColor.BrushMarkTextbox;
            StackPanel parent = GetParent(tb) as StackPanel;
            StackPanel row = (tb.Parent) as StackPanel;
            //Find textbox isFocused
            if (!unMark)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (row.Children[i].IsFocused)
                    {
                        _PosColumnGotFocus = i;
                        break;
                    }
                }
            }
            //Mark column
            for (int i = 0; i < 9; i++)
            {
                StackPanel rows = parent.Children[i] as StackPanel;
                for (int j = 0; j < 9; j++)
                {
                    if (((TextBox)rows.Children[j]).Background.Equals(ControlColor.BrushMarkTextbox))
                    {
                        _PosRowGotFocus = i;
                        break;
                    }
                }
                TextBox tbChild = rows.Children[_PosColumnGotFocus] as TextBox;
                tbChild.Background = brushes;
            }
        }

        private static void MarkBoxTextbox(TextBox tb, bool unMark = true)
        {
            if (_PosColumnGotFocus < 0 || _PosRowGotFocus < 0) return;
            else
            {
                var brushes = unMark ? Brushes.Transparent : ControlColor.BrushMarkTextbox;
                StackPanel spSudoku = GetParent(tb) as StackPanel;
                Tuple<int, int> getPosBox = GetPosBox(_PosRowGotFocus, _PosColumnGotFocus);
                int col = getPosBox.Item2;
                int row = getPosBox.Item1;

                for (int i = 0; i < 9; i++)
                {
                    StackPanel rows = spSudoku.Children[i] as StackPanel;
                    for (int j = 0; j < 9; j++)
                    {
                        var textbox = rows.Children[j] as TextBox;
                        if (textbox.IsReadOnly && textbox.Text.Equals(tb.Text))
                        {
                            textbox.Background = brushes;
                        }
                    }
                }

                for (int i = (row * 3) - 3; i < (row * 3); i++)
                {
                    StackPanel rows = spSudoku.Children[i] as StackPanel;
                    for (int j = (col * 3) - 3; j < (col * 3); j++)
                    {
                        var textbox = rows.Children[j] as TextBox;
                        textbox.Background = brushes;
                    }
                }
            }
        }

        private static void RemoveBackgroundTextbox(TextBox tb)
        {
            StackPanel spSudoku = GetParent(tb) as StackPanel;
            foreach (StackPanel rows in spSudoku.Children)
            {
                foreach (TextBox tbCol in rows.Children)
                    tbCol.Background = Brushes.Transparent;
            }
            isChecking = false;
        }

        private static Tuple<int, int> GetPosBox(int PosRow, int PosColumn)
        {
            int col = -1, row = -1;
            for (int i = 2; i <= 8; i += 3)
            {
                col = PosColumn <= i && col.Equals(-1) ? (i + 1) / 3 : col;
                row = PosRow <= i && row.Equals(-1) ? (i + 1) / 3 : row;
                if (!col.Equals(-1) && !row.Equals(-1)) break;
            }
            return Tuple.Create(row, col);
        }

        private static FrameworkElement GetParent(TextBox tb)
        {
            FrameworkElement element = tb;
            while (!(element.Parent is DialogHost))
            {
                element = element.Parent as FrameworkElement;
            }
            return element;
        }

        #endregion

        public static async Task<bool> IsValidSudoku(StackPanel sp, List<List<int>> sudokuResult)
        {
            if (sp.Children.Count != 9) return false;
            if (((StackPanel)sp.Children[0]).Children.Count != 9) return false;

            isChecking = true;
            int countEmptyCell = 0, countWrongCell = 0;
            int posRow = 0, posCol = 0;
            foreach (StackPanel spchil in sp.Children)
            {
                foreach (UIElement txb in spchil.Children)
                {
                    var textbox = (TextBox)txb;
                    if (!textbox.Background.Equals(Brushes.DarkGray) &&
                        !textbox.IsReadOnly)
                    {
                        if (string.IsNullOrEmpty(textbox.Text))
                        {
                            textbox.Background = ControlColor.BrushWrongTextbox;
                            countEmptyCell++;
                            await Task.Delay(20);
                        }
                        else if (!Convert.ToInt32(textbox.Text).Equals(sudokuResult[posRow][posCol]))
                        {
                            textbox.Background = ControlColor.BrushWrongTextbox;
                            countWrongCell++;
                            await Task.Delay(20);
                        }
                    }
                    posCol++;
                }
                posCol = 0;
                posRow++;
            }

            return (countEmptyCell.Equals(0) && countWrongCell.Equals(0));
        }

        public static void DrawButton(ItemsControl ic)
        {
            int count = 1;
            for (int i = 0; i < 3; i++)
            {
                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                for (int j = 0; j < 3; j++)
                {
                    var button = new Button
                    {
                        Content = count.ToString()
                    };
                    button.Click += Button_Click;
                    sp.Children.Add(button);
                    count++;
                }
                ic.Items.Add(sp);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
