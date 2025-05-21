using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GettingRealWPF.Model;

namespace GettingRealWPF.ViewModel
{
    public class RegisterMaterialViewModel : INotifyPropertyChanged
    {
        //Repositories for material, inventoryitem og storage
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        //Constructors
        public RegisterMaterialViewModel() : this(new FileMaterialRepository("materials.txt"), new FileInventoryItemRepository("inventoryitems.txt"), new FileStorageRepository("storages.txt"))
        {
        }

        public RegisterMaterialViewModel(IMaterialRepository materialRepository, IInventoryItemRepository inventoryItemRepository, IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            // Load initial data
            LoadInitialData();

            //Initialiser RelayCommand til at gemme
            RegisterMaterialCommand = new RelayCommand(RegisterMaterial, CanRegisterMaterial);
        }

        //ObservableCollections til at holde styr på valgte elementer
        public ObservableCollection<Material.Category> Categories { get; }
            = new ObservableCollection<Material.Category>();
        public ObservableCollection<Material> Materials { get; }
            = new ObservableCollection<Material>();
        public ObservableCollection<Storage> Storages { get; }
            = new ObservableCollection<Storage>();
        public ObservableCollection<InventoryItem> InventoryItems { get; }
            = new ObservableCollection<InventoryItem>();

        private void LoadInitialData()
        {
            foreach (var m in _materialRepository.GetAllMaterials()) Materials.Add(m);
            foreach (var s in _storageRepository.GetAllStorages()) Storages.Add(s);
            foreach (var i in _inventoryItemRepository.GetAllInventoryItems()) InventoryItems.Add(i);

            Categories.Clear();
            foreach (var cat in InventoryItems.Select(i => i.Material.MaterialCategory).Distinct()) Categories.Add(cat);
        }

       
        private string _selectedCategory;
        private string _selectedDescription;
        private int _selectedQuantity;
        private string _selectedUnit;
        private Storage _selectedStorage;
        private int _selectedMinimumAmount;

        // Kategori
        public string SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); }
        }

        // Beskrivelse
        public string SelectedDescription
        {
            get => _selectedDescription;
            set { _selectedDescription = value; OnPropertyChanged(nameof(SelectedDescription)); }
        }

        // Antal
        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set { _selectedQuantity = value; OnPropertyChanged(nameof(SelectedQuantity)); }
        }


        //Enhed
        public string SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(nameof(SelectedUnit)); }
        }

        // Placering
        public Storage SelectedStorage
        {
            get => _selectedStorage;
            set { _selectedStorage = value; OnPropertyChanged(nameof(SelectedStorage)); }
        }

        // Minimumsbeholdning
        public int SelectedMinimumAmount
        {
            get => _selectedMinimumAmount;
            set { _selectedMinimumAmount = value; OnPropertyChanged(nameof(SelectedMinimumAmount)); }
        }


        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ICommand RegisterMaterialCommand { get; }
        private bool CanRegisterMaterial()
        {
            return !string.IsNullOrWhiteSpace(SelectedCategory) &&
                   !string.IsNullOrWhiteSpace(SelectedDescription) &&
                   SelectedQuantity > 0 &&
                   !string.IsNullOrWhiteSpace(SelectedUnit) &&
                   SelectedStorage != null &&
                   SelectedMinimumAmount >= 0;
        }

        public void RegisterMaterial()
        {
            try
            {
                // Opret et nyt materiale
                var newMaterial = new Material(
                    (Material.Category)Enum.Parse(typeof(Material.Category), SelectedCategory),
                    SelectedDescription,
                    SelectedQuantity,
                    (Material.Unit)Enum.Parse(typeof(Material.Unit), SelectedUnit)
                );
                // Opret et nyt inventory item
                var newInventoryItem = new InventoryItem(newMaterial, SelectedQuantity, SelectedStorage);
                // Gem materialet og inventory item i deres respektive repositories
                _materialRepository.AddMaterial(newMaterial);
                _inventoryItemRepository.AddInventoryItem(newInventoryItem);
                VerificationMessage = "Materiale registreret!";
                ClearFields();
            }
            catch (Exception ex)
            {
                VerificationMessage = $"Fejl ved registrering: {ex.Message}";
            }
        }

        private void ClearFields()
        {
            SelectedCategory = null;
            SelectedDescription = null;
            SelectedQuantity = 0;
            SelectedUnit = null;
            SelectedStorage = null;
            SelectedMinimumAmount = 0;
        }

        //Verification besked
        private string _verificationMessage;

        public string VerificationMessage
        {
            get => _verificationMessage;
            set
            {
                if (_verificationMessage != value)
                {
                    _verificationMessage = value;
                    OnPropertyChanged(nameof(VerificationMessage));
                }
            }
        }
    }
}
