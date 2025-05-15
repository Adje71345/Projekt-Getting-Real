using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GettingRealWPF.ViewModel;


namespace GettingRealWPF.View
{
    public partial class HomeView : Window
    {


        public HomeView()
        {
            // Initialize components and set up data bindings
            InitializeComponent();

            // Initialiser ViewModel og sæt som DataContext
            DataContext = new GettingRealWPF.ViewModel.HomeViewModel();
        }

        // Håndter klik på menu knapper
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

            if (sender is not RadioButton selectedButton)
                return;

            // Håndter navigation baseret på knappens indhold
            string? option = Convert.ToString(selectedButton.Content);

            switch (option)
            {
                case "Lagerliste":
                    // Allerede på lagerliste
                    break;
                case "Registrer materiale":
                    // Gå til RegisterMaterialView
                    break;
                case "Tilføj materiale":
                    // Gå til AddMaterialView
                    break;
                case "Fjern materiale":
                    // Gå til ConsumeMaterialView
                    break;
                case "Rediger materiale":
                    // Gå til EditMaterialView
                    break;
                default:
                    break;
            }
        }

        // Åbner RegisterMaterialView
        private void OpenRegisterMaterialView()
        {
            var registerMaterialView = new RegisterMaterialView();
            registerMaterialView.Show();
            Close(); // Luk nuværende vindue
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Tom implementation eller bind til ViewModel
        }





    }
}

