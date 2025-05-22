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
    public partial class ConsumeInventoryItemView : UserControl
    {
        public ConsumeInventoryItemView()
        {
            InitializeComponent();
            DataContext = new ViewModel.ConsumeInventoryItemViewModel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ConsumeInventoryItemViewModel;
            viewModel?.ClearFields();
        }

    }
}
