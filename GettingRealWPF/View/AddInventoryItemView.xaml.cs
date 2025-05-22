using System.Windows;
using System.Windows.Controls;
using GettingRealWPF.Model;
using GettingRealWPF.ViewModel;

namespace GettingRealWPF.View
{
    /// <summary>
    /// Interaction logic for AddMaterialView.xaml
    /// </summary>
    public partial class AddInventoryItemView : UserControl
    {
        public AddInventoryItemView()
        {
            InitializeComponent();
            // Sæt DataContext til ViewModel med korrekte repositories
            DataContext = new AddInventoryItemViewModel(
                new FileMaterialRepository("materials.txt"),
                new FileInventoryItemRepository("inventoryitems.txt"),
                new FileStorageRepository("storages.txt"));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AddInventoryItemViewModel;
            viewModel?.ClearFields();
        }

    }
}
