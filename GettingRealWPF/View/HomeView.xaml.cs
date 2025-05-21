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
using GettingRealWPF.View;
using GettingRealWPF.View.UserControls;
using GettingRealWPF.ViewModel;

namespace GettingRealWPF.View
{

    public partial class HomeView : Window
    {
        private HomeViewModel _viewModel;

        public HomeView()
        {
            this.DataContext = new HomeViewModel();
            InitializeComponent();

            var menuBar = new GettingRealWPF.View.UserControls.MenuBar();
            menuBarPlaceholder.Content = menuBar;
            var menuControl = menuBar;

            _viewModel = DataContext as HomeViewModel;

            // Initial view
            _viewModel.CurrentView = new InventoryListView();

            menuControl.rbLagerListe.IsChecked = true;

            // Alle views er nu UserControls
            menuControl.rbLagerListe.Checked += (s, e) =>
                _viewModel.CurrentView = new InventoryListView();

            menuControl.rbRegisterMaterial.Checked += (s, e) =>
                _viewModel.CurrentView = new RegisterMaterialView();

            menuControl.rbAddMaterial.Checked += (s, e) =>
            _viewModel.CurrentView = new AddMaterialView();

            menuControl.rbConsumeMaterial.Checked += (s, e) =>
                _viewModel.CurrentView = new ConsumeMaterialView();

            menuControl.rbMoveMaterial.Checked += (s, e) =>
                _viewModel.CurrentView = new MoveMaterialView();
        }
    }
}
