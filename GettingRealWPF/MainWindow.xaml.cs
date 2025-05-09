using System.Windows;
using GettingRealWPF.ViewModels;

namespace GettingRealWPF
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            viewModel.Password = PasswordBox.Password;
            viewModel.LoginCommand.Execute(null);
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
