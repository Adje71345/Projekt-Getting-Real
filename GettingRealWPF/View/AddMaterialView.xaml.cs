using System.Windows.Controls;
using GettingRealWPF.ViewModel;
using GettingRealWPF.Model;

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
        }
    }
}
