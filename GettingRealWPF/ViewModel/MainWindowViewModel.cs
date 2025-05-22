using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GettingRealWPF.Model;
using GettingRealWPF.Models;

namespace GettingRealWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly FileUserRepository userRepo;

        private string _username;
        private string _password;
        private string _loginMessage;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public string LoginMessage
        {
            get => _loginMessage;
            set { _loginMessage = value; OnPropertyChanged(nameof(LoginMessage)); }
        }

        public ICommand LoginCommand { get; }

        public MainWindowViewModel()
        {
            userRepo = new FileUserRepository("users.txt");
            LoginCommand = new RelayCommand(Login);
        }

        public void Login()
        {
            var users = userRepo.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Name == Username && u.Password == Password);

            if (user != null)
            {
                LoginMessage = "Login succesfuldt";
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                LoginMessage = "Forkert brugernavn eller adgangskode";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Event for vellykket login
        public event EventHandler LoginSuccessful;
    }
}
