using MaterialDesignThemes.Wpf;
using Sodoku.Views;
using SudokuUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Level = SudokuUtility.Models.Level;
using Sudoku = SudokuUtility.Models.Sudoku;
using SudokuUtility.Interfaces;
using SudokuUtility.Models;
using System.Linq;
using System.Text;

namespace Sodoku.ViewModels
{
    class NewWindowViewModel : BaseViewModel
    {
        #region PrivateProperties
        private Level _selectedLevel;
        private object _selectedOption;
        private ObservableCollection<Level> _listLevel;
        private bool _isEnableBtnStart;
        private bool _isEnableBtnResume;
        private string _timerCountDown;
        private readonly Stopwatch _stopwatch;
        private Task TaskRunTimer;
        private List<List<int>> _SudokuResult = new List<List<int>>();
        private readonly IHighScore _highScore;
        #endregion

        #region Command
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand StartButtonCommand { get; set; }
        public ICommand CheckButtonCommand { get; set; }
        public ICommand PauseButtonCommand { get; set; }
        public ICommand HighScoreCommand { get; set; }
        #endregion

        #region PublicProperties

        public ObservableCollection<Level> ListLevel
        {
            get => _listLevel;
            set
            {
                _listLevel = value;
                OnPropertyChanged();
            }
        }
        public Level SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                OnPropertyChanged();
            }
        }
        public object SelectedOption
        {
            get => _selectedOption;
            set
            {
                _selectedOption = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnableBtnStart
        {
            get => _isEnableBtnStart;
            set
            {
                _isEnableBtnStart = value;
                OnPropertyChanged();
            }
        }

        public string TimerCountDown
        {
            get => _timerCountDown;
            set
            {
                _timerCountDown = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnableBtnResume
        {
            get => _isEnableBtnResume;
            set
            {
                _isEnableBtnResume = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public NewWindowViewModel()
        {
            _highScore = new HighScoreService();
            _stopwatch = new Stopwatch();
            ListLevel = new ObservableCollection<Level>();
            ListLevel.Add(new Level() { Name = "Easy" });
            ListLevel.Add(new Level() { Name = "Medium" });
            ListLevel.Add(new Level() { Name = "Hard" });
            ListLevel.Add(new Level() { Name = "Expert" });

            StartButtonCommand = new RelayCommand<StackPanel>((p) => true, async (p) =>
            {
                if (SelectedLevel != null && SelectedOption != null)
                {
                    if (((ComboBoxItem)SelectedOption).Content.Equals("Using API"))
                    {
                        if (_SudokuResult.Count != 0) _SudokuResult.Clear();
                        Api api = new Api();
                        Sudoku su = await api.GetJson(SelectedLevel);
                        if (su != null)
                        {
                            _SudokuResult = su.GetAnswer;
                            WindowSudoku.SetSudoku(su.GetQuestion, p);
                            if (_stopwatch.IsRunning)
                            {
                                _stopwatch.Reset();
                                TimerCountDown = string.Empty;
                                while (TaskRunTimer.Status == TaskStatus.Running)
                                {
                                    TaskRunTimer.Wait();
                                    TaskRunTimer.Dispose();
                                }
                                TaskRunTimer = null;
                            }
                            _stopwatch.Start();
                        }
                    }
                    else
                    {
                        SudokuTable table = new SudokuTable();
                        bool IsDone = true;
                        while (IsDone)
                        {
                            try
                            {
                                table.Solve();
                                IsDone = false;
                            }
                            catch
                            {

                            }
                            if (IsDone)
                            {
                                table = null;
                                table = new SudokuTable();
                                table.Solve();
                            }
                        }
                        if (_SudokuResult.Count != 0) _SudokuResult.Clear();
                        SudokuGenerate gen = new SudokuGenerate(SelectedLevel, table.ToString());
                        WindowSudoku.SetSudoku(gen.Question, p);
                        _SudokuResult = new List<List<int>>(gen.Answer);
                        if (_stopwatch.IsRunning)
                        {
                            
                            TimerCountDown = string.Empty;
                            while (TaskRunTimer.Status == TaskStatus.Running)
                            {
                                TaskRunTimer.Wait();
                                TaskRunTimer.Dispose();
                            }
                            TaskRunTimer = null;
                        }
                        _stopwatch.Reset();
                        _stopwatch.Start();
                    }
                    StartTimer();
                    IsEnableBtnResume = true;
                    //MessageBox.Show(((p.Children[0] as StackPanel).Children[0] as TextBox).Background.ToString());

                    DialogHost.CloseDialogCommand.Execute(null, p);
                }
            });

            LoadedWindowCommand = new RelayCommand<object[]>((p) => true, (p) =>
            {
                //Show loading on DialogHost
                Task task = Task.Run(() =>
                {
                    if (Application.Current.Dispatcher != null)
                        Application.Current.Dispatcher.Invoke((Action)(async () =>
                        {
                            UCLoadingView loading = new UCLoadingView();
                            await DialogHost.Show(loading, "Sudoku");
                        }));
                });

                //Enable button start
                IsEnableBtnStart = true;
                //Disable button Resume 
                IsEnableBtnResume = false;
                //Convert Object[] (p) to BindingControl
                BindingControl bc = new BindingControl()
                { GridSudoku = p[0] as StackPanel, GridButton = p[1] as ItemsControl };
                WindowSudoku.DrawButton(bc.GridButton);
                WindowSudoku.SetEvent(bc.GridSudoku);
            });

            CheckButtonCommand = new RelayCommand<StackPanel>((p) => true, async (p) =>
            {
                IsEnableBtnStart = false;
                int countEmpty = 0;
                Task task = Task.Run(() =>
                {
                    if (Application.Current.Dispatcher != null)
                        Application.Current.Dispatcher.Invoke((Action)(async () =>
                       {
                           #region oldMethod
                           //foreach (StackPanel stackChild in p.Children)
                           //{
                           //    foreach (UIElement tb in stackChild.Children)
                           //    {
                           //        var textbox = tb as TextBox;
                           //        if (string.IsNullOrEmpty(textbox.Text) && !textbox.IsReadOnly)
                           //        {
                           //            textbox.Background = Brushes.PaleVioletRed;
                           //            countEmpty++;
                           //            await Task.Delay(50);
                           //        }
                           //    }
                           //}
                           #endregion

                           Task<bool> taskRun = WindowSudoku.IsValidSudoku(p, _SudokuResult);
                           bool result = await taskRun;

                           IsEnableBtnStart = true;
                           WindowSudoku.GridSudoku = p;
                           if (result)
                           {
                               if (countEmpty.Equals(0))
                               {
                                   _stopwatch.Stop();
                                   TimeSpan ts = _stopwatch.Elapsed;
                                   MessageBox.Show($"Hoàn thành trong {ts.Minutes}:{ts.Seconds}");
                                   _highScore.WriteScore(new HighScoreModel
                                   {
                                       DateTime = DateTime.Now,
                                       Level = SelectedLevel.GetLevelConstants(),
                                       CompleteTime = ts

                                   });
                                   while (TaskRunTimer.Status == TaskStatus.Running)
                                   {
                                       TaskRunTimer.Wait();
                                       TaskRunTimer.Dispose();
                                   }
                                   TaskRunTimer = null;
                               }
                           }
                       }));
                });
                await task;

                if (TaskRunTimer == null)
                {
                    StartButtonCommand.Execute(p);
                }
            });

            PauseButtonCommand = new RelayCommand<object[]>(p => true, (p) =>
            {
                Button ButtonPause = (Button)p[0];
                StackPanel spGridSudoku = (StackPanel)p[1];
                if (_stopwatch.IsRunning)
                {
                    Task task = Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(async () =>
                        {
                            UCLoadingView loading = new UCLoadingView();
                            await DialogHost.Show(loading, "Sudoku");
                        }));
                    });
                    var child = (StackPanel)ButtonPause.Content;
                    ((PackIcon)child.Children[0]).Kind = PackIconKind.Play;
                    ((TextBlock)child.Children[1]).Text = "Resume";
                    _stopwatch.Stop();
                    while (TaskRunTimer.Status == TaskStatus.Running)
                    {
                        TaskRunTimer.Wait();
                        TaskRunTimer.Dispose();
                    }
                    TaskRunTimer = null;
                }
                else if (!string.IsNullOrEmpty(TimerCountDown))
                {
                    var child = (StackPanel)ButtonPause.Content;
                    ((MaterialDesignThemes.Wpf.PackIcon)child.Children[0]).Kind = PackIconKind.Pause;
                    ((TextBlock)child.Children[1]).Text = "Pause";
                    DialogHost.CloseDialogCommand.Execute(null, spGridSudoku);
                    _stopwatch.Start();
                    StartTimer();
                }
            });

            HighScoreCommand = new RelayCommand<object>(p => true, (p) =>
            {
                if (SelectedLevel != null)
                {
                    var highScore = _highScore.ReadScore();

                    if (highScore != null)
                    {
                        var data = highScore.Where(x => x.Level == SelectedLevel.GetLevelConstants()).ToList();
                        if (data == null)
                        {
                            MessageBox.Show("No record!", "Message");
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < data.Count; ++i)
                            {
                                sb.AppendLine(data[i].ToString());
                            }
                            MessageBox.Show(sb.ToString(), "Message");  
                        }
                    }
                    else
                    {
                        MessageBox.Show("No record!", "Message");
                    }
                }
                else
                {
                    MessageBox.Show("Please chooose level first!", "Message");
                }
            });
        }

        private List<List<int>> GetSudokuFromFile(string address)
        {
            List<List<int>> sudoku = new List<List<int>>();
            string stringFromFile = File.ReadAllText(address);
            if (string.IsNullOrEmpty(stringFromFile))
                return null;

            string[] rows = stringFromFile.Split('\n');
            foreach (string row in rows)
            {
                if (!string.IsNullOrEmpty(row))
                {
                    List<int> listLine = new List<int>();
                    string[] line = row.Split(' ');
                    foreach (string s in line)
                        if (!string.IsNullOrEmpty(s))
                            listLine.Add(Convert.ToInt32(s));
                    if (listLine.Count > 0)
                        sudoku.Add(listLine);
                }
            }

            return sudoku;
        }

        private void StartTimer()
        {
            if (TaskRunTimer == null)
            {
                TaskRunTimer = Task.Run(() =>
                {
                    while (_stopwatch.IsRunning)
                    {
                        TimeSpan ts = _stopwatch.Elapsed;
                        TimerCountDown = $"{ts.Minutes}:{ts.Seconds}";
                    }
                });
            }
        }
    }

    class BindingControl
    {
        public StackPanel GridSudoku { get; set; }
        public ItemsControl GridButton { get; set; }
    }

}
