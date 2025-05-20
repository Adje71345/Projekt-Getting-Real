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
using GettingRealWPF.ViewModel;

namespace GettingRealWPF.View
{
    public partial class InventoryListView : UserControl
    {
        private InventoryListViewModel _viewModel;

        public InventoryListView()
        {
            InitializeComponent();
            _viewModel = new InventoryListViewModel();
            this.DataContext = _viewModel;

            // Populer ComboBoxes
            searchBar.cmbCategory.ItemsSource = _viewModel.Categories;
            searchBar.cmbLocation.ItemsSource = _viewModel.Locations;

            // Default valg
            searchBar.cmbCategory.SelectedIndex = 0;
            searchBar.cmbLocation.SelectedIndex = 0;

            // Bind søgning
            searchBar.txtInput.TextChanged += (s, e) =>
            {
                _viewModel.SearchText = searchBar.txtInput.Text;
                UpdateComboBoxTexts();
            };

            // Bind kategori filter
            searchBar.cmbCategory.SelectionChanged += (s, e) =>
            {
                if (searchBar.cmbCategory.SelectedItem != null)
                {
                    _viewModel.SelectedCategory =
                        searchBar.cmbCategory.SelectedItem.ToString();
                    UpdateComboBoxTexts();
                }
            };

            // Bind lokation filter
            searchBar.cmbLocation.SelectionChanged += (s, e) =>
            {
                if (searchBar.cmbLocation.SelectedItem != null)
                {
                    _viewModel.SelectedLocation =
                        searchBar.cmbLocation.SelectedItem.ToString();
                    UpdateComboBoxTexts();
                }
            };

            // Initial tekst
            UpdateComboBoxTexts();
        }

        private void UpdateComboBoxTexts()
        {
            searchBar.UpdateComboBoxText(
                searchText: _viewModel.SearchText,
                selectedCategory: _viewModel.SelectedCategory,
                selectedLocation: _viewModel.SelectedLocation
            );
        }
    }
}
