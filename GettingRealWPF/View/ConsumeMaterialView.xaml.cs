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
using System.Windows.Shapes;
using GettingRealWPF.ViewModel;

namespace GettingRealWPF.View
{
    /// <summary>
    /// Interaction logic for ConsumeMaterialView.xaml
    /// </summary>
    public partial class ConsumeMaterialView : UserControl
    {
        public ConsumeMaterialView()
        {
            InitializeComponent();
            DataContext = new ViewModel.ConsumeMaterialViewModel();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Hvis dette tidligere var et vindue, der lukkede sig selv:
            // Naviger tilbage til InventoryListView
            var window = Window.GetWindow(this);
            if (window is HomeView homeView)
            {
                var viewModel = homeView.DataContext as HomeViewModel;
                if (viewModel != null)
                {
                    viewModel.CurrentView = new InventoryListView();
                }
            }
        }
    }
}
