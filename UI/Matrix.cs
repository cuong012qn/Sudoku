namespace UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public class Matrix
    {
        private static TextBox tmpTextBox = new TextBox();
        public static List<List<Number>> MatrixSudoku = new List<List<Number>>();
        public static StackPanel PanelSudoku = new StackPanel();

        public static List<List<Number>> GetMatrixFromFile(string address)
        {
            List<List<Number>> result = new List<List<Number>>();
            try
            {
                string[] inputFromFile = File.ReadAllText(address).Split('\n');
                foreach (string s in inputFromFile)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        List<Number> Lines = new List<Number>();
                        string[] vs = s.Split(' ');
                        foreach (string v in vs)
                        {
                            if (!string.IsNullOrEmpty(v))
                            {
                                int temp = Convert.ToInt32(v);
                                if (temp.Equals(0))
                                    Lines.Add(new Number() { Value = temp, CanEdit = true });
                                else Lines.Add(new Number() { Value = temp, CanEdit = false });
                            }
                        }

                        result.Add(Lines);
                    }
                }
            }
            catch
            {
                return null;
            }
            return result;
        }

        public List<List<int>> MergeMatrix(List<List<int>> Matrix, List<List<int>> inputMatrix, int BlockCol)
        {
            List<List<int>> result = Matrix;
            int row = 0;
            for (int i = (BlockCol * 3) - 3; i < (BlockCol * 3); i++)
            {
                for (int j = 0; j < 3; j++)
                    Matrix[i].Add(inputMatrix[row][j]);
                row++;
            }
            return result;
        }

        public static bool SetMatrix(StackPanel sp, List<List<Number>> inputMatrix)
        {
            int colBlock = 1, rowBlock = 1;
            try
            {
                foreach (StackPanel mainStack in sp.Children)
                {
                    foreach (Border bd in mainStack.Children)
                    {
                        int posRow = (rowBlock * 3) - 3; //0
                        int posCol = (colBlock * 3) - 3; //0
                        foreach (StackPanel stackChild in ((StackPanel)bd.Child).Children)
                        {
                            foreach (UIElement item in stackChild.Children)
                            {
                                if (item is TextBox)
                                {
                                    (item as TextBox).Background = Brushes.White;
                                    #region Set event Textbox
                                    (item as TextBox).MaxLength = 1;
                                    (item as TextBox).PreviewTextInput += TextBox_PreviewTextInput;
                                    (item as TextBox).MouseDoubleClick += TextBox_MouseDoubleClick;
                                    (item as TextBox).GotFocus += TextBox_GotFocus;
                                    (item as TextBox).LostFocus += TextBox_LostFocus;
                                    #endregion
                                    if (!inputMatrix[posRow][posCol].Value.Equals(0))
                                    {
                                        (item as TextBox).Background = Brushes.CornflowerBlue;
                                        (item as TextBox).Text =
                                            inputMatrix[posRow][posCol].Value.ToString();
                                    }
                                    else
                                        (item as TextBox).Text = null;
                                }
                                posCol++;
                            }
                            posRow++;
                            posCol = (colBlock * 3) - 3;
                        }
                        colBlock++;
                    }
                    colBlock = 1; rowBlock++;
                }
            }
            catch { return false; }
            return true;
        }

        public static List<List<Number>> GetMatrix(StackPanel sp)
        {
            List<List<Number>> result = new List<List<Number>>();
            foreach (StackPanel row in sp.Children)
            {
                foreach (Border block in row.Children)
                {
                    List<Number> tempList = new List<Number>();
                    List<List<Number>> r = GetMiniMatrix(block.Child as StackPanel);
                    foreach (List<Number> item in r)
                    {
                        item.ForEach(x => tempList.Add(x));
                    }
                    result.Add(tempList);
                }
            }

            MatrixSudoku = result;
            return result;
        }

        public static void SetEventButton(StackPanel sp)
        {
            foreach (StackPanel s in sp.Children)
            {
                foreach (UIElement btn in s.Children)
                {
                    if (btn is Button)
                        (btn as Button).Click += Button_Click;
                }
            }
        }

        private static List<List<Number>> GetMiniMatrix(StackPanel sp)
        {
            List<List<Number>> result = new List<List<Number>>();
            foreach (StackPanel item in sp.Children)
            {
                List<Number> temp = new List<Number>();
                foreach (UIElement UI in item.Children)
                {
                    if (UI is TextBox)
                    {
                        if ((UI as TextBox).Background.Equals(Brushes.CornflowerBlue))
                            temp.Add(new Number() { Value = Convert.ToInt32((UI as TextBox).Text), CanEdit = false });
                        else if (string.IsNullOrEmpty((UI as TextBox).Text))
                            temp.Add(new Number() { Value = 0, CanEdit = true });
                        else temp.Add(new Number() { Value = Convert.ToInt32((UI as TextBox).Text), CanEdit = true });
                    }
                }
                result.Add(temp);
            }
            return result;
        }


        #region Event
        private static void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!((TextBox)sender).Background.Equals(Brushes.CornflowerBlue))
                ((TextBox)sender).Text = string.Empty;
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!((TextBox)sender).Background.Equals(Brushes.CornflowerBlue))
                ((TextBox)sender).Background = Brushes.White;
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!((TextBox)sender).Background.Equals(Brushes.CornflowerBlue))
                ((TextBox)sender).Background = Brushes.Red;
            tmpTextBox = (TextBox)sender;
            GetMatrix(PanelSudoku);
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tmpTextBox.Background != null)
                if (!tmpTextBox.Background.Equals(Brushes.CornflowerBlue))
                    tmpTextBox.Text = ((Button)sender).Content.ToString();
        }
        #endregion
    }
}
