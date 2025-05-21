using System.Windows;
using System.Windows.Controls;
using GettingRealWPF.Model;
using GettingRealWPF.ViewModel;

namespace GettingRealWPF.View
{
    /// <summary>
    /// Interaction logic for AddMaterialView.xaml
    /// </summary>
    public partial class AddMaterialView : UserControl
    {
        public AddMaterialView()
        {
            InitializeComponent();
            // Sæt DataContext til ViewModel med korrekte repositories
            DataContext = new AddMaterialViewModel(
                new FileMaterialRepository("materials.txt"),
                new FileInventoryItemRepository("inventoryitems.txt"),
                new FileStorageRepository("storages.txt"));
        } // <-- VIGTIGT: Manglende afsluttende parentes og semikolon her

        // CancelButton_Click metoden skal være UDEN FOR konstruktøren
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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
