using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using GettingRealWPF.Model;
using Material = GettingRealWPF.Model.Material;

namespace GettingRealWPF.ViewModel
{
    public class ConsumeInventoryItemViewModel : INotifyPropertyChanged
    {
        //Repositories for material, inventoryitem og storage
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        //Constructors
        public ConsumeInventoryItemViewModel() : this(new FileMaterialRepository("materials.txt"), new FileInventoryItemRepository("inventoryitems.txt"), new FileStorageRepository("storages.txt"))
        {
        }

        public ConsumeInventoryItemViewModel(IMaterialRepository materialRepository, IInventoryItemRepository inventoryItemRepository, IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            // Load initial data
            LoadInitialData();

            //Initialiser RelayCommand til at gemme
            ConsumeInventoryItemCommand = new RelayCommand(ConsumeInventoryItem, CanConsumeInventoryItem);
        }

        //ObservableCollections til at holde styr på valgte elementer
        public ObservableCollection<Material.Category> Categories { get; } = new ObservableCollection<Material.Category>();
        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();
        public ObservableCollection<Storage> Storages { get; } = new ObservableCollection<Storage>();
        public ObservableCollection<InventoryItem> InventoryItems { get; } = new ObservableCollection<InventoryItem>();
        private ICollectionView _availableMaterialsView;
        public ICollectionView AvailableMaterialsView
        {
            get => _availableMaterialsView;
        }

        //Valgte elementer i UI
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
                    _availableMaterialsView.Refresh();
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

        public string SelectedUnitDisplay => SelectedMaterial?.MaterialUnit.ToString() ?? string.Empty;


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


        //Amount Properties
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
                    CommandManager.InvalidateRequerySuggested();
                    RecalculateUpdatedAmount();
                }
            }
        }

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
                    CommandManager.InvalidateRequerySuggested();
                }
                RecalculateUpdatedAmount();
            }
        }

        private string _amountToConsumeText;
        public string AmountToConsumeText
        {
            get => _amountToConsumeText;
            set
            {
                if (_amountToConsumeText != value)
                {
                    _amountToConsumeText = value;
                    OnPropertyChanged(nameof(AmountToConsumeText));

                    // Hvis feltet er tomt, tolkes det som 0.
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        AmountToConsume = 0;
                    }
                    else if (int.TryParse(value, out int parsed))
                    {
                        AmountToConsume = parsed;
                    }
                    else
                    {
                        AmountToConsume = 0;
                    }
                }
            }
        }

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


        //Commands
        public ICommand ConsumeInventoryItemCommand { get; }
        private bool CanConsumeInventoryItem()
        {
            return SelectedInventoryItem != null && AmountToConsume > 0 && AmountToConsume <= CurrentAmount;
        }

        //Metoder
        private void LoadInitialData()
        {
            foreach (var m in _materialRepository.GetAllMaterials()) Materials.Add(m);
            foreach (var s in _storageRepository.GetAllStorages()) Storages.Add(s);
            foreach (var i in _inventoryItemRepository.GetAllInventoryItems()) InventoryItems.Add(i);

            Categories.Clear();
            foreach (var cat in InventoryItems.Select(i => i.Material.MaterialCategory).Distinct()) Categories.Add(cat);
            InitializeMaterialsView();
        }

        private void InitializeMaterialsView()
        {
            _availableMaterialsView = CollectionViewSource.GetDefaultView(Materials);
            _availableMaterialsView.Filter = obj =>
            {
                if (obj is Material material)
                {
                    // Hvis en kategori er valgt, så sikre, at materialet tilhører denne kategori
                    if (SelectedCategory != null && material.MaterialCategory != SelectedCategory)
                        return false;

                    // Saml mængden for dette materiale på tværs af alle lagre.
                    var totalAmount = _inventoryItemRepository
                                        .GetAllInventoryItems()
                                        .Where(i => i.Material.Description.Equals(material.Description, StringComparison.OrdinalIgnoreCase))
                                        .Sum(i => i.Amount);

                    // Vis materialet, hvis den samlede mængde er større end 0.
                    return totalAmount > 0;
                }
                return false;
            };
        }

        private void UpdateSelectedInventoryItem()
        {
            SelectedInventoryItem = SelectedCategory != null && SelectedMaterial != null && SelectedStorage != null
                ? _inventoryItemRepository.GetInventoryItem(SelectedMaterial.Description, SelectedStorage.StorageName)
                : null;

            CurrentAmount = SelectedInventoryItem?.Amount ?? 0;
            AmountToConsume = 0;
            RecalculateUpdatedAmount();
            OnPropertyChanged(nameof(SelectedUnitDisplay));
        }

        private void RecalculateUpdatedAmount()
        {
            UpdatedAmount = CurrentAmount - AmountToConsume;
        }

        public void ConsumeInventoryItem()
        {
            if (SelectedInventoryItem == null)
                return;

            try
            {
                _inventoryItemRepository.DecreaseAmount(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName,
                    AmountToConsume);

                // Hent det opdaterede inventory item
                var updatedItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName);

                if (updatedItem == null)
                {
                    // Hvis item er blevet slettet, opdater CurrentAmount og SelectedInventoryItem
                    CurrentAmount = 0;
                    SelectedInventoryItem = null;
                }
                else
                {
                    SelectedInventoryItem = updatedItem;
                    CurrentAmount = updatedItem.Amount;
                }

                VerificationMessage = $"{AmountToConsume} {SelectedUnitDisplay} {SelectedMaterial.Description} fjernet. Ny beholdning: {CurrentAmount} {SelectedUnitDisplay}.";

                // Nulstil forbruget
                AmountToConsume = 0;
                AmountToConsumeText = string.Empty;
                RecalculateUpdatedAmount();
                ClearFields();
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved forbrug af materiale.", ex);
            }
        }

        public void ClearFields()
        {
            // Nulstil inputs
            AmountToConsume = 0;
            AmountToConsumeText = string.Empty;

            // Nulstil references
            SelectedMaterial = null;
            SelectedStorage = null;

            // Nulstil kategori til sidst (dette kan påvirke materials-listen)
            SelectedCategory = null;

            // Nulstil statusbeskeder
            VerificationMessage = string.Empty;
        }

        //Eventhandler implementering
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
