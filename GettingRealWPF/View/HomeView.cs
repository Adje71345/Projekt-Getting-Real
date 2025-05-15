using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GettingRealWPF.ViewModel;
using GettingRealWPF.Model;


namespace GettingRealWPF.View
{
    public partial class HomeView : Window
    {
        // Constructor
        public HomeView()
        {
            // Initialize components and set up data bindings
            InitializeComponent();
            DataContext= new HomeViewModel();
        }

    }
}
