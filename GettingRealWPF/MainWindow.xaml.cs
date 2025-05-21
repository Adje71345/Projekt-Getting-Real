using System.Windows;
using GettingRealWPF.View;
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

            // Abonner på event for vellykket login
            viewModel.LoginSuccessful += ViewModel_LoginSuccessful;
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            viewModel.Password = PasswordBox.Password;
            viewModel.LoginCommand.Execute(null);
        }

        // Når login lykkes, åbn HomeView og lukMainWidow
        private void ViewModel_LoginSuccessful(object sender, System.EventArgs e)
        {
            HomeView homeView = new HomeView();
            homeView.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
