using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GettingRealWPF.View
{
    /// <summary>
    /// Interaction logic for MoveMaterialView.xaml
    /// </summary>
    public partial class MoveMaterialView : Window
    {
        public MoveMaterialView()
        {
            InitializeComponent();
            DataContext = new ViewModel.MoveMaterialViewModel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationService = NavigationService.GetNavigationService(this);
            if (navigationService != null)
            {
                navigationService.Navigate(new HomeView());
            }
            else
            {
                this.Close();
            }
        }
    }
}
