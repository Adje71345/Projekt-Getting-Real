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

        //Constructors
        public RegisterMaterialViewModel() : this(new FileMaterialRepository("materials.txt"), new FileInventoryItemRepository("inventoryitems.txt"), new FileStorageRepository("storages.txt"))
        {
        }

        public ConsumeMaterialViewModel(IMaterialRepository materialRepository, IInventoryItemRepository inventoryItemRepository, IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            // Load initial data
            LoadInitialData();

            //Initialiser RelayCommand til at gemme
            ConsumeMaterialCommand = new RelayCommand(ConsumeMaterial, CanConsumeMaterial);
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

                    int clampedAmount = GetClampedAmountToConsume(AmountToConsume);
                    if (_amountToConsume != clampedAmount)
                    {
                        _amountToConsume = clampedAmount;
                        OnPropertyChanged(nameof(AmountToConsume));
                    }
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
                int clampedValue = GetClampedAmountToConsume(value);
                if (_amountToConsume != clampedValue)
                {
                    _amountToConsume = clampedValue;
                    OnPropertyChanged(nameof(AmountToConsume));
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

        //Computed Properties til at vise den valgte enhed i UI'en
        public string SelectedUnitDisplay => SelectedMaterial?.MaterialUnit.ToString() ?? string.Empty;

        //Commands
        public ICommand ConsumeMaterialCommand { get; }
        private bool CanConsumeMaterial()
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
        }

        private void FilterMaterialsByCategory()
        {
            Materials.Clear();

            var filtered = InventoryItems
                .Where(item => item.Material.MaterialCategory == SelectedCategory)
                .Select(item => item.Material)
                .GroupBy(m => m.Description)
                .Select(g => g.First());

            foreach (var m in filtered)
                Materials.Add(m);
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

        private int GetClampedAmountToConsume(int value)
        {
            return Math.Max(0, Math.Min(value, CurrentAmount));
        }

        public void ConsumeMaterial()
        {
            if (SelectedInventoryItem == null)
                return;

            int clampedAmount = GetClampedAmountToConsume(AmountToConsume);

            try
            {
                _inventoryItemRepository.DecreaseAmount(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName,
                    AmountToConsume);

                // Hent det opdaterede inventory item
                SelectedInventoryItem = _inventoryItemRepository.GetInventoryItem(
                    SelectedInventoryItem.Material.Description,
                    SelectedInventoryItem.Storage.StorageName);

                // Opdater den aktuelle mængde
                CurrentAmount = SelectedInventoryItem?.Amount ?? 0;

                VerificationMessage = $"{AmountToConsume} {SelectedUnitDisplay} {SelectedMaterial.Description} fjernet. Ny beholdning: {CurrentAmount} {SelectedUnitDisplay}.";

                // Nulstil AmountToConsume og opdater den beregnede nye mængde
                AmountToConsume = 0;
                AmountToConsumeText = string.Empty;
                RecalculateUpdatedAmount();
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved forbrug af materiale.", ex);
            }
        }

        //Eventhandler implementering
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
