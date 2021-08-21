namespace UI
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class MainWindow
    {
        private ItemsControl _itemsControl;

        public ItemsControl ItemsControl { get => _itemsControl; set => _itemsControl = value; }

        public MainWindow()
        {

        }

        public MainWindow(ItemsControl p)
        {
            this.ItemsControl = p;
        }

        public static void DrawListTextBox(ItemsControl items)
        {
            StackPanel rows = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            for (int i = 0; i < 9; i++)
            {
                StackPanel columns = new StackPanel() { Orientation = Orientation.Horizontal };
                for (int j = 0; j < 9; j++)
                {
                    columns.Children.Add(new TextBox()
                    {
                        Style = (Style)Application.Current.Resources["MaterialDesignOutlinedTextFieldTextBox"],
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 20,
                        Height = 100,
                        Width = 100,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(5),
                        MaxLength = 1
                    });
                }
                rows.Children.Add(columns);
            }
            items.Items.Add(rows);
        }

        public static void AddEvent_TextBox(ItemsControl items)
        {
            StackPanel matrix = (items.Items[0] as StackPanel);
            for (int i = 0; i < 9; i++)
            {
                StackPanel columnStack = matrix.Children[i] as StackPanel;
                foreach (UIElement column in columnStack.Children)
                {
                    if (column is TextBox)
                        (column as TextBox).PreviewTextInput += Column_PreviewTextInput;
                }
            }
        }

        private static void Column_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Set value in textbox from matrix
        /// </summary>
        /// <param name="p">Itemscontrol</param>
        /// <param name="input">matrix</param>
        public static void SetMatrix(ItemsControl p, List<List<int>> input)
        {
            if (Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke((Action) (() =>
                {
                    int countRows = 0;
                    foreach (StackPanel row in ((StackPanel) p.Items[0]).Children)
                    {
                        int countColumns = 0;
                        foreach (UIElement column in row.Children)
                        {
                            if (column is TextBox)
                            {
                                (column as TextBox).Text = input[countRows][countColumns].ToString();
                                //(column as TextBox).Width = (column as TextBox).ActualHeight;
                                //(column as TextBox).TextAlignment = TextAlignment.Center;
                                countColumns++;
                            }
                        }

                        countRows++;
                    }
                }));
        }

        /// <summary>
        /// Get value in textbox to matrix
        /// </summary>
        /// <param name="p">ItemsControl</param>
        /// <returns>Matrix in list textbox</returns>
        public static List<List<int>> GetMatrix(ItemsControl p)
        {
            List<List<int>> matrixResult = new List<List<int>>();

            //ItemsControl to list
            StackPanel sp = (p.Items[0] as StackPanel);
            foreach (StackPanel chil in sp.Children)
            {
                List<int> templist = new List<int>();
                foreach (UIElement uiElem in chil.Children)
                {
                    if (uiElem is TextBox)
                        if (!string.IsNullOrEmpty((uiElem as TextBox).Text))
                            templist.Add(Convert.ToInt32((uiElem as TextBox).Text));
                        else templist.Add(0);
                }
                matrixResult.Add(templist);
            }
            return matrixResult;
        }
    }
}
