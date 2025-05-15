using GettingRealWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;


namespace GettingRealWPF.ViewModel
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        // Repositories til at hente data
        private readonly IMaterialRepository _materialRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IStorageRepository _storageRepository;

        // Samling af inventory items til visning i DataGrid
        private ObservableCollection<InventoryItem> _inventoryItems;
        public ObservableCollection<InventoryItem> InventoryItems
        {
            get => _inventoryItems;
            set
            {
                _inventoryItems = value;
                OnPropertyChanged(nameof(InventoryItems));
            }
        }

        // CollectionView til filtrering og sortering
        private ICollectionView _inventoryItemsView;
        public ICollectionView InventoryItemsView => _inventoryItemsView;

        // Søgetekst
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterItems();
            }
        }

        // Filtrering efter kategori
        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                FilterItems();
            }
        }

        // Filtrering efter placering
        private string _selectedLocation;
        public string SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged(nameof(SelectedLocation));
                FilterItems();
            }
        }

        // Lister til ComboBoxes
        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Locations { get; } = new ObservableCollection<string>();

        // Konstruktør
        public HomeViewModel() : this(
            new FileMaterialRepository("materials.txt"),
            new FileInventoryItemRepository("inventoryitems.txt"),
            new FileStorageRepository("storages.txt"))
        {
        }

        // Konstruktør med dependency injection (til testing)
        public HomeViewModel(IMaterialRepository materialRepository,
                            IInventoryItemRepository inventoryItemRepository,
                            IStorageRepository storageRepository)
        {
            _materialRepository = materialRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _storageRepository = storageRepository;

            // Initialiser data
            LoadData();
        }

        // Indlæs data fra repositories
        private void LoadData()
        {
            // Hent inventory items
            var items = _inventoryItemRepository.GetAllInventoryItems();
            InventoryItems = new ObservableCollection<InventoryItem>(items);

            // Opret CollectionView til filtrering og sortering
            _inventoryItemsView = CollectionViewSource.GetDefaultView(InventoryItems);

            // Indlæs kategorier og placeringer til filtrering
            LoadCategories();
            LoadLocations();
        }

        // Indlæs kategorier til filtrering
        private void LoadCategories()
        {
            Categories.Clear();
            Categories.Add("Alle kategorier"); // Standard valgmulighed

            // Tilføj unikke kategorier fra materialer
            var categories = _materialRepository.GetAllMaterials()
                .Select(m => m.Category)
                .Distinct()
                .OrderBy(c => c);

            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            // Vælg standard
            SelectedCategory = "Alle kategorier";
        }

        // Indlæs placeringer til filtrering
        private void LoadLocations()
        {
            Locations.Clear();
            Locations.Add("Alle placeringer"); // Standard valgmulighed

            // Tilføj unikke placeringer fra storage
            var locations = _storageRepository.GetAllStorages()
                .Select(s => s.StorageName)
                .Distinct()
                .OrderBy(l => l);

            foreach (var location in locations)
            {
                Locations.Add(location);
            }

            // Vælg standard
            SelectedLocation = "Alle placeringer";
        }

        // Filtrer items baseret på søgning og filtreringer
        private void FilterItems()
        {
            if (_inventoryItemsView == null)
                return;

            _inventoryItemsView.Filter = item =>
            {
                var inventoryItem = (InventoryItem)item;

                // Filtrer efter søgeord
                bool matchesSearch = string.IsNullOrEmpty(SearchText) ||
                    inventoryItem.Material.Description.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    inventoryItem.Material.Category.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;

                // Filtrer efter kategori
                bool matchesCategory = SelectedCategory == "Alle kategorier" ||
                    inventoryItem.Material.Category == SelectedCategory;

                // Filtrer efter placering
                bool matchesLocation = SelectedLocation == "Alle placeringer" ||
                    inventoryItem.Storage.StorageName == SelectedLocation;

                return matchesSearch && matchesCategory && matchesLocation;
            };
        }

        // PropertyChanged event handler
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}