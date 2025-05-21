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

namespace GettingRealWPF.View
{
    /// <summary>
    /// Interaction logic for RegisterMaterialView.xaml
    /// </summary>
    public partial class RegisterMaterialView : Window
    {
        private readonly IMaterialRepository _materialRepository;
        public RegisterMaterialView()
        <Button Content = "Gem" Command="{Binding RegisterMaterialCommand}" />
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void SaveMaterial()
        {
            // Convert string values to enums as needed
            if (!Enum.TryParse<Category>(SelectedCategory, out var category))
            {
                VerificationMessage = "Invalid category selected.";
                return;
            }
            if (!Enum.TryParse<Unit>(SelectedUnit, out var unit))
            {
                VerificationMessage = "Invalid unit selected.";
                return;
            }

            var material = new Material(category, Description, MinimumAmount, unit)
            {
                // Set other properties if needed
            };

            _materialRepository.AddMaterial(material);
            VerificationMessage = "Materialet er nu oprettet.";
        }

        public RegisterMaterialViewModel(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
            RegisterMaterialCommand = new RelayCommand(SaveMaterial);
        }


    }
    public class MaterialRepository : IMaterialRepository
{
    private readonly List<Material> _materials = new();

    public IEnumerable<Material> GetAllMaterials() => _materials;

    public Material? GetMaterialByDescription(string materialDescription) =>
        _materials.FirstOrDefault(m => m.Description == materialDescription);

    public void AddMaterial(Material material)
    {
        _materials.Add(material);
    }

    public void UpdateMaterial(Material material)
    {
        var existing = GetMaterialByDescription(material.Description);
        if (existing != null)
        {
            // Update properties as needed
        }
    }

    public void DeleteMaterial(string materialDescription)
    {
        var material = GetMaterialByDescription(materialDescription);
        if (material != null)
            _materials.Remove(material);
    }
}
public interface IMaterialRepository
{
    IEnumerable<Material> GetAllMaterials();
    Material? GetMaterialByDescription(string materialDescription);
    void AddMaterial(Material material);
    void UpdateMaterial(Material material);
    void DeleteMaterial(string materialDescription);
}
}
