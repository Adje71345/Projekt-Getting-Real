using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GettingRealWPF.Model;
using System.Windows.Input;

namespace GettingRealWPF.ViewModel
{
    public class MoveInventoryItemViewModel : INotifyPropertyChanged
    {
        //Repositories for material, inventoryitem og storage
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        //Constructors
        public MoveInventoryItemViewModel() : this(new FileMaterialRepository("materials.txt"), new FileInventoryItemRepository("inventoryitems.txt"), new FileStorageRepository("storages.txt"))
        {
        }

        public MoveInventoryItemViewModel(IMaterialRepository materialRepository, IInventoryItemRepository inventoryItemRepository, IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            //Initialiser RelayCommand til at gemme
            MoveMaterialCommand = new RelayCommand(MoveMaterial, CanMoveMaterial);

            LoadInitialData();
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
        public ObservableCollection<Storage> AvailableToLocations { get; private set; } 
            = new ObservableCollection<Storage>();

        //Valgte elementer i comboboxes
        private Material.Category? _selectedCategory;
        public Material.Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    FilterMaterialsByCategory();
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
                    OnPropertyChanged(nameof(SelectedUnitDisplay));
                    OnPropertyChanged(nameof(CurrentAmount));
                    UpdateSelectedInventoryItem();
                }
            }
        }

        private Storage _selectedFromLocation;
        public Storage SelectedFromLocation
        {
            get => _selectedFromLocation;
            set
            {
                if (_selectedFromLocation != value)
                {
                    _selectedFromLocation = value;
                    OnPropertyChanged(nameof(SelectedFromLocation));
                    OnPropertyChanged(nameof(CurrentAmount));
                    UpdateAvailableToLocations();
                }
            }
        }


        private Storage _selectedToLocation;
        public Storage SelectedToLocation
        {
            get => _selectedToLocation;
            set
            {
                if (_selectedToLocation != value)
                {
                    _selectedToLocation = value;
                    OnPropertyChanged(nameof(SelectedToLocation));
                }
            }
        }


        private InventoryItem _selectedInventoryItem;
        public InventoryItem SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            private set
            {
                _selectedInventoryItem = value;
                OnPropertyChanged(nameof(SelectedInventoryItem));
            }
        }

        private int _moveAmount;
        public int MoveAmount
        {
            get => _moveAmount;
            set
            {
                if (_moveAmount != value)
                {
                    _moveAmount = value;
                    OnPropertyChanged(nameof(MoveAmount));
                }
            }
        }

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

        //Computed Properties til at vise den valgte enhed og Currentamount i UI'en
        public string SelectedUnitDisplay => SelectedMaterial?.MaterialUnit.ToString() ?? string.Empty;

        public int CurrentAmount
        {
            get
            {
                if (SelectedMaterial != null && SelectedFromLocation != null)
                {
                    var inventoryItem = _inventoryItemRepository.GetInventoryItem(
                        SelectedMaterial.Description, SelectedFromLocation.StorageName);

                    return inventoryItem?.Amount ?? 0; // Hvis null, vis 0
                }
                return 0;
            }
        }


        //RelayCommand til at flytte materialer
        public ICommand MoveMaterialCommand { get; }

        private bool CanMoveMaterial()
        {
            return SelectedInventoryItem != null &&
                   MoveAmount > 0 &&
                   SelectedMaterial != null &&
                   SelectedFromLocation != null &&
                   SelectedToLocation != null;
        }

        private void MoveMaterial()
        {
            try
            {
                // Hent det inventory-item ud fra valgt materiale og "fra"-location
                var inventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedMaterial.Description,
                    SelectedFromLocation.StorageName);

                if (inventoryItem == null)
                {
                    VerificationMessage = $"Fejl: InventoryItem med materiale '{SelectedMaterial.Description}' på '{SelectedFromLocation.StorageName}' blev ikke fundet.";
                    return; // Stopper funktionen uden at crashe
                }

                if (MoveAmount > inventoryItem.Amount)
                {
                    VerificationMessage = "Fejl: Flyttemængden overstiger den tilgængelige mængde.";
                    return; // Stopper funktionen uden at forsøge at flytte
                }

                // Flyt materialet fra "fra"-location til "til"-location
                _inventoryItemRepository.MoveInventoryItem(
                  SelectedMaterial.Description,
                  SelectedFromLocation.StorageName,
                  SelectedToLocation.StorageName,
                  MoveAmount);

                VerificationMessage = $"{MoveAmount} {SelectedUnitDisplay} {SelectedMaterial.Description} er flyttet fra '{SelectedFromLocation.StorageName}' til '{SelectedToLocation.StorageName}'.";
                RefreshInventoryItems();
                ClearFields();
            }
            catch (Exception ex)
            {
                VerificationMessage = $"Fejl ved flytning af materiale: {ex.Message}";
            }
        }

        //Metoder
        private void RefreshInventoryItems()
        {
            InventoryItems.Clear();
            foreach (var item in _inventoryItemRepository.GetAllInventoryItems())
            {
                InventoryItems.Add(item);
            }
        }

        private void LoadInitialData()
        {
            foreach (var m in _materialRepository.GetAllMaterials()) Materials.Add(m);
            foreach (var s in _storageRepository.GetAllStorages()) Storages.Add(s);
            foreach (var i in _inventoryItemRepository.GetAllInventoryItems()) InventoryItems.Add(i);

            Categories.Clear();
            foreach (var cat in InventoryItems.Select(i => i.Material.MaterialCategory).Distinct()) Categories.Add(cat);
        }

        private void FilterMaterialsByCategory()
        {
            Materials.Clear();

            if (SelectedCategory == null) return;

            var filteredMaterials = _materialRepository.GetMaterialsByCategory((Material.Category)SelectedCategory);

            foreach (var m in filteredMaterials)
                Materials.Add(m);
        }

        private void UpdateSelectedInventoryItem()
        {
            if (SelectedMaterial != null && SelectedFromLocation != null)
            {
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedMaterial.Description,
                    SelectedFromLocation.StorageName);
            }
            else
            {
                SelectedInventoryItem = null;
            }
        }

        private void UpdateAvailableToLocations()
        {
            AvailableToLocations.Clear();

            foreach (var storage in Storages)
            {
                if (storage != SelectedFromLocation)
                    AvailableToLocations.Add(storage);
            }
        }
        public void ClearFields()
        {
            SelectedCategory = null;
            SelectedMaterial = null;
            SelectedFromLocation = null;
            SelectedToLocation = null;
            MoveAmount = 0;
            SelectedInventoryItem = null;
        }


        //Eventhandler implementering
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
