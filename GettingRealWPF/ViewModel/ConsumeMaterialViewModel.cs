using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using GettingRealWPF.Model;
using Material = GettingRealWPF.Model.Material;

namespace GettingRealWPF.ViewModel
{
    public class ConsumeMaterialViewModel : INotifyPropertyChanged
    {
        //Repositories for material, inventoryitem og storage
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        //Konstruktor, der intitialiserer repositories
        public ConsumeMaterialViewModel() : this(new FileMaterialRepository("materials.txt"), new FileInventoryItemRepository("inventoryitems.txt"), new FileStorageRepository("storages.txt"))
        {
        }

        //constructor for repositories
        public ConsumeMaterialViewModel(IMaterialRepository materialRepository, IInventoryItemRepository inventoryItemRepository, IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;
            Categories = new ObservableCollection<string>();
            Materials = new ObservableCollection<Material>();
            Storages = new ObservableCollection<Storage>();
            // Load initial data
            LoadInitialData();
        }

        // 1. Lister til comboboxes
        public ObservableCollection<string> Categories { get; }
        public ObservableCollection<Material> Materials { get; }
        public ObservableCollection<Storage> Storages { get; }

        // 2. Valgte elementer til comboboxes
        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    // Valgfrit: filter din Materials-list baseret på kategori
                    // fx UpdateMaterialsBasedOnCategory();
                    UpdateSelectedInventoryItem();
                }
            }
        }

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
                    UpdateSelectedInventoryItem();
                }
            }
        }

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
                    UpdateSelectedInventoryItem();
                }
            }
        }

        // Den inventory item der svarer til de tre valg – opdateres automatisk
        private InventoryItem _selectedInventoryItem;
        public InventoryItem SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set
            {
                if (_selectedInventoryItem != value)
                {
                    _selectedInventoryItem = value;
                    OnPropertyChanged(nameof(SelectedInventoryItem));
                    if (_selectedInventoryItem != null)
                    {
                        CurrentAmount = _selectedInventoryItem.Amount;
                        AmountToConsume = 0; // nulstil fjerningsmængden
                        RecalculateUpdatedAmount();
                        OnPropertyChanged(nameof(SelectedUnitDisplay));
                    }
                }
            }
        }

        // Nåværende beholdning (fra inventory item)
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
                    RecalculateUpdatedAmount();
                }
            }
        }

        // Antal der skal fjernes – brugeren indtaster dette
        private int _amountToConsume;
        public int AmountToConsume
        {
            get => _amountToConsume;
            set
            {
                if (_amountToConsume != value)
                {
                    _amountToConsume = value;
                    OnPropertyChanged(nameof(AmountToConsume));
                    RecalculateUpdatedAmount();
                }
            }
        }

        // Udregnet ny beholdning
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

        // Viser unit for det valgte material (read-only)
        public string SelectedUnitDisplay => SelectedInventoryItem?.Material.MaterialUnit.ToString() ?? string.Empty;

        // Metode til at loade initial data fra repositories
        private void LoadInitialData()
        {
            // Henter alle materialer og fyld Categories og Materials
            var allMaterials = _materialRepository.GetAllMaterials();
            foreach (var m in allMaterials)
            {
                if (!Categories.Contains(m.Category))
                    Categories.Add(m.Category);
                Materials.Add(m);
            }

            // Hent alle lager (placeringer)
            var allStorages = _storageRepository.GetAllStorages();
            foreach (var s in allStorages)
            {
                Storages.Add(s);
            }
        }

        // Når alle tre valg er sat, opdateres SelectedInventoryItem vha. repository
        private void UpdateSelectedInventoryItem()
        {
            // Antag, at material og storage udgør et unikt identifikator for inventory item
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedMaterial != null && SelectedStorage != null)
            {
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedMaterial.Description, SelectedStorage.StorageName);
            }
            else
            {
                SelectedInventoryItem = null;
            }
        }

        // Udregn den nye beholdning
        private void RecalculateUpdatedAmount()
        {
            UpdatedAmount = CurrentAmount - AmountToConsume;
        }

        //Gemmer ændringerne i repository'et
        public void ConsumeMaterial()
        {
            if (SelectedInventoryItem == null)
                return;

            // Valider input – sikrer at AmountToConsume ikke er negativ og ikke overstiger CurrentAmount
            if (AmountToConsume < 0)
                AmountToConsume = 0;
            if (AmountToConsume > CurrentAmount)
                AmountToConsume = CurrentAmount;
            try
            {
                // Kalder repository'et, så den vedvarende lagerbeholdning opdateres
                _inventoryItemRepository.DecreaseAmount(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName,
                    AmountToConsume);

                // Hent det opdaterede inventory item fra repository'et
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName);

                // Denne linje sætter CurrentAmount ud fra den opdaterede item og
                // RecalculateUpdatedAmount vil opsætte UpdatedAmount efter dine regler
                CurrentAmount = SelectedInventoryItem?.Amount ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved forbrug af materiale.", ex);
            }
        }

        //OBS!!!!! Mangler RelayCommand til at gemme med consumematerial-metode


        //Eventhandler for property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
