using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace Sodoku.ViewModels
{
    class UCLoadingViewModel : BaseViewModel
    {
        public ICommand CloseButtonCommand { get; set; }

        public UCLoadingViewModel()
        {
            CloseButtonCommand = new RelayCommand<object>(p => true, (p) =>
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
            });
        }
    }
}
