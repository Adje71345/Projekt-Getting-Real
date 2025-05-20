using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GettingRealWPF.Model;  // Modelklasser som Material, Storage osv. hentes
using GettingRealWPF.Helpers; // RelayCommand-klassen til knap-kommandoer hentes

namespace GettingRealWPF.ViewModel
{
    public class AddMaterialViewModel : INotifyPropertyChanged
    {
        // Repositories til dataadgang for materialer, inventory items og lagre
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        // ObservableCollections til data i comboboxes
        public ObservableCollection<Material.Category> Categories { get; }
            = new ObservableCollection<Material.Category>(); // Materialekategorier
        public ObservableCollection<Material> Materials { get; }
            = new ObservableCollection<Material>(); // Materialeliste
        public ObservableCollection<Storage> Storages { get; }
            = new ObservableCollection<Storage>(); // Lagerplaceringer

        // Kommando for “Tilføj”-knappen
        public ICommand AddCommand { get; }

        // Valgt kategori i UI
        private Material.Category? _selectedCategory;
        public Material.Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (!Nullable.Equals(_selectedCategory, value))
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));

                    // Materialer filtreres efter valgt kategori
                    FilterMaterialsByCategory();
                }
            }
        }

        // Valgt materiale i UI
        private Material _selectedMaterial;
        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                if (_selectedMaterial != value)
                {
                    _selectedMaterial = value;
                    OnPropertyChanged(nameof(SelectedMaterial));

                    // Lagerbeholdning opdateres ved valg af materiale
                    UpdateSelectedInventoryItem();
                }
            }
        }

        // Valgt lagerplacering i UI
        private Storage _selectedStorage;
        public Storage SelectedStorage
        {
            get => _selectedStorage;
            set
            {
                if (_selectedStorage != value)
                {
                    _selectedStorage = value;
                    OnPropertyChanged(nameof(SelectedStorage));

                    // Lagerbeholdning opdateres ved valg af lager
                    UpdateSelectedInventoryItem();
                }
            }
        }

        // Den aktuelle lagerbeholdning for valgt materiale og lager
        private InventoryItem _selectedInventoryItem;
        public InventoryItem SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set
            {
                _selectedInventoryItem = value;
                OnPropertyChanged(nameof(SelectedInventoryItem));

                // Aktuel mængde opdateres
                CurrentAmount = _selectedInventoryItem?.Amount ?? 0;

                // Beregner ny beholdning baseret på tilføjet antal
                RecalculateUpdatedAmount();
            }
        }

        // Aktuel mængde på lager
        private int _currentAmount;
        public int CurrentAmount
        {
            get => _currentAmount;
            set
            {
                _currentAmount = value;
                OnPropertyChanged(nameof(CurrentAmount));

                // Ny beholdning beregnes ved ændring i mængde
                RecalculateUpdatedAmount();
            }
        }

        // Mængde der skal tilføjes, indtastet af bruger
        private int _amountToAdd;
        public int AmountToAdd
        {
            get => _amountToAdd;
            set
            {
                _amountToAdd = value;
                OnPropertyChanged(nameof(AmountToAdd));

                // Ny beholdning beregnes ved ændring i tilføjet antal
                RecalculateUpdatedAmount();
            }
        }

        // Ny beregnet beholdning efter tilføjelse
        private int _updatedAmount;
        public int UpdatedAmount
        {
            get => _updatedAmount;
            set
            {
                _updatedAmount = value;
                OnPropertyChanged(nameof(UpdatedAmount)); // UI opdateres automatisk
            }
        }

        // Statusbesked til UI, fx fejl eller bekræftelse
        private string _statusMessage = ""; // Starter som tom tekst
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage)); // UI opdateres
            }
        }

        // Constructor initialiserer repositories og henter data
        public AddMaterialViewModel(IMaterialRepository materialRepository,
                                    IInventoryItemRepository inventoryItemRepository,
                                    IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            // Data til comboboxes hentes
            LoadInitialData();

            // Kommando til knappen "Tilføj" initialiseres
            AddCommand = new RelayCommand(AddMaterial);
        }

        // Henter data til dropdowns
        private void LoadInitialData()
        {
            var allMaterials = _materialRepository.GetAllMaterials();
            foreach (var m in allMaterials)
            {
                if (!Categories.Contains(m.MaterialCategory))
                    Categories.Add(m.MaterialCategory);

                Materials.Add(m);
            }

            foreach (var s in _storageRepository.GetAllStorages())
            {
                Storages.Add(s);
            }
        }

        // Filtrerer materialer efter valgt kategori
        private void FilterMaterialsByCategory()
        {
            Materials.Clear();

            var filteredMaterials = _materialRepository.GetAllMaterials()
                .Where(m => m.MaterialCategory == SelectedCategory);

            foreach (var m in filteredMaterials)
            {
                Materials.Add(m);
            }
        }

        // Opdaterer lagerbeholdning baseret på valgt materiale og lager
        private void UpdateSelectedInventoryItem()
        {
            if (SelectedMaterial != null && SelectedStorage != null)
            {
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedMaterial.Description,
                    SelectedStorage.StorageName);
            }
            else
            {
                SelectedInventoryItem = null;
            }
        }

        // Beregner ny beholdning
        private void RecalculateUpdatedAmount()
        {
            UpdatedAmount = CurrentAmount + AmountToAdd;
        }

        // Tilføjer materiale til lageret ved knaptryk
        private void AddMaterial()
        {
            if (SelectedInventoryItem == null || AmountToAdd <= 0)
            {
                StatusMessage = "Vælg materiale og lager samt et gyldigt antal.";
                return;
            }

            try
            {
                // Kalder repository til at øge lagerbeholdningen
                _inventoryItemRepository.IncreaseAmount(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName,
                    AmountToAdd);

                // Opdaterer lagerbeholdning efter ændring
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName);

                StatusMessage = "Materiale tilføjet og beholdning opdateret.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Der opstod en fejl: {ex.Message}";
            }
        }

        // Event for at opdatere UI ved property ændringer
        public event PropertyChangedEventHandler PropertyChanged;

        // Metode som kaldes for at fortælle UI at en property er ændret
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

