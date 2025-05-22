using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GettingRealWPF.Model;    // Model-klasser som Material, Storage, InventoryItem

namespace GettingRealWPF.ViewModel
{
    public class AddInventoryItemViewModel : INotifyPropertyChanged
    {
        // Repositories til dataadgang for materialer, lagerbeholdning og lagre
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        // ObservableCollections til dropdowns i UI (kategori, materiale, lagerplacering)
        public ObservableCollection<Material.Category> Categories { get; } = new ObservableCollection<Material.Category>();
        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();
        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();

        // Kommando til "Tilføj" knappen i UI
        public ICommand AddInventoryItemCommand { get; }

        // Valgt kategori i UI (kan være null)
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

                    // Når kategori ændres, filtreres materialer
                    FilterMaterialsByCategory();
                }
            }
        }

        // Valgt materiale i UI (kan være null)
        private Material? _selectedMaterial;
        public Material? SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                if (_selectedMaterial != value)
                {
                    _selectedMaterial = value;
                    OnPropertyChanged(nameof(SelectedMaterial));
                    OnPropertyChanged(nameof(SelectedUnitDisplay));

                    // Når materiale ændres, opdater lagerbeholdning
                    UpdateSelectedInventoryItem();
                }
            }
        }

        // Valgt lagerplacering i UI (kan være null)
        private Storage? _selectedStorage;
        public Storage? SelectedStorage
        {
            get => _selectedStorage;
            set
            {
                if (_selectedStorage != value)
                {
                    _selectedStorage = value;
                    OnPropertyChanged(nameof(SelectedStorage));

                    // Når lager ændres, opdater lagerbeholdning
                    UpdateSelectedInventoryItem();
                }
            }
        }

        // Lagerbeholdningspost der matcher valgt materiale og lager (kan være null)
        private InventoryItem? _selectedInventoryItem;
        public InventoryItem? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set
            {
                if (_selectedInventoryItem != value)
                {
                    _selectedInventoryItem = value;
                    OnPropertyChanged(nameof(SelectedInventoryItem));

                    // Opdater den aktuelle mængde på lager
                    CurrentAmount = _selectedInventoryItem?.Amount ?? 0;

                    // Beregn opdateret beholdning baseret på tilføjelse
                    RecalculateUpdatedAmount();
                }
            }
        }

        // Aktuel mængde af valgt materiale på valgt lager
        private int _currentAmount;
        public int CurrentAmount
        {
            get => _currentAmount;
            set
            {
                if (_currentAmount != value)
                {
                    _currentAmount = value;
                    OnPropertyChanged(nameof(CurrentAmount));

                    // Når den aktuelle mængde ændres, opdater den nye beholdning
                    RecalculateUpdatedAmount();
                }
            }
        }

        // Antal materialer, som brugeren vil tilføje
        private int _amountToAdd;
        public int AmountToAdd
        {
            get => _amountToAdd;
            set
            {
                if (_amountToAdd != value)
                {
                    _amountToAdd = value;
                    OnPropertyChanged(nameof(AmountToAdd));
                    RecalculateUpdatedAmount();
                }
            }
        }
        // Tekstfelt til at indtaste antal materialer, der skal tilføjes
        private string _amountToAddText;
        public string AmountToAddText
        {
            get => _amountToAddText;
            set
            {
                if (_amountToAddText != value)
                {
                    _amountToAddText = value;
                    OnPropertyChanged(nameof(AmountToAddText));

                    // Når feltet er tomt, tolkes det som 0.
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        AmountToAdd = 0;
                    }
                    else if (int.TryParse(value, out int parsed))
                    {
                        AmountToAdd = parsed;
                    }
                    else
                    {
                        AmountToAdd = 0;
                    }
                }
            }
        }

        // Computed Property til at vise den valgte enhed
        public string SelectedUnitDisplay => SelectedMaterial?.MaterialUnit.ToString() ?? string.Empty;

        // Beregnet ny beholdning efter tilføjelse
        private int _updatedAmount;
        public int UpdatedAmount
        {
            get => _updatedAmount;
            set
            {
                if (_updatedAmount != value)
                {
                    _updatedAmount = value;
                    OnPropertyChanged(nameof(UpdatedAmount));
                }
            }
        }

        // Statusbesked til UI, fx fejl eller succesmeddelelse
        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        // Konstruktor med dependency injection af repositories
        public AddInventoryItemViewModel(IMaterialRepository materialRepository,
                                    IInventoryItemRepository inventoryItemRepository,
                                    IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            // Hent data til dropdowns ved opstart
            LoadInitialData();

            // Initialiser kommandoen til "Tilføj" knappen
            AddInventoryItemCommand = new RelayCommand(AddInventoryItem, CanAddInventoryItem);
        }

        // Hent data til dropdowns (kategori, materiale, lager)
        private void LoadInitialData()
        {
            Categories.Clear();
            Materials.Clear();
            Storages.Clear();

            // Tilføj alle kategorier fra enum
            foreach (Material.Category category in Enum.GetValues(typeof(Material.Category)))
            {
                Categories.Add(category);
            }

            

            // Tilføj lagerplaceringer
            foreach (var s in _storageRepository.GetAllStorages())
            {
                Storages.Add(s);
            }
        }

        // Filtrerer materialer baseret på valgt kategori
        private void FilterMaterialsByCategory()
        {
            Materials.Clear();

            if (SelectedCategory == null)
                return;

            var allMaterialsInCategory = _materialRepository.GetMaterialsByCategory((Material.Category)SelectedCategory);

            foreach (var material in allMaterialsInCategory)
            {
                Materials.Add(material);
            }
        }

        // Opdaterer SelectedInventoryItem baseret på valgt materiale og lager
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

        // Beregn den nye beholdning ud fra aktuel mængde og tilføjet antal
        private void RecalculateUpdatedAmount()
        {
            UpdatedAmount = CurrentAmount + AmountToAdd;
        }

        // Metode der kaldes når "Tilføj" knappen trykkes
        private void AddInventoryItem()
        {
            // Tjek at både SelectedMaterial og SelectedStorage er valgt
            if (SelectedMaterial == null || SelectedStorage == null)
            {
                StatusMessage = "Vælg materiale og lager.";
                return;
            }

            // Tjek at antal der skal tilføjes er gyldigt (> 0)
            if (AmountToAdd <= 0)
            {
                StatusMessage = "Indtast et gyldigt antal at tilføje.";
                return;
            }

            try
            {
                var newItem = new InventoryItem(SelectedMaterial, AmountToAdd, SelectedStorage);
                _inventoryItemRepository.AddInventoryItem(newItem);

                // Hent og opdater SelectedInventoryItem efter ændringen
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedMaterial.Description,
                    SelectedStorage.StorageName);

                StatusMessage = "Materiale tilføjet og beholdning opdateret.";
                ClearFields();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fejl: {ex.Message}";
            }
        }

        private bool CanAddInventoryItem()
        {
            return SelectedMaterial != null && SelectedStorage != null && AmountToAdd > 0;
        }

        // Event fra INotifyPropertyChanged, der opdaterer UI ved ændringer
        public event PropertyChangedEventHandler? PropertyChanged;

        // Hjælpe-metode til at notificere UI om property-ændringer
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public void ClearFields()
        {
            
            AmountToAddText = null;
            CurrentAmount = 0;
            UpdatedAmount = 0;

            
            SelectedMaterial = null;
            SelectedStorage = null;
            SelectedInventoryItem = null;

           
            SelectedCategory = null;
        }
    }    
}
