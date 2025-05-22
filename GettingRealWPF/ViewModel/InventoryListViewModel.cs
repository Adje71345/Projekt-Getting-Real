using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GettingRealWPF.Model;
using System.IO;

namespace GettingRealWPF.ViewModel
{
    public class InventoryListViewModel : INotifyPropertyChanged
    {
        private IInventoryItemRepository _inventoryItemRepository;
        private List<InventoryItem> _allItems;
        private ObservableCollection<InventoryItem> _filteredItems;
        private string _searchText = "";
        private string _selectedCategory = "Alle kategorier";
        private string _selectedLocation = "Alle lokationer";

        public InventoryListViewModel()
        {
            // Slet filen hvis den eksisterer
            if (File.Exists("inventoryitems.txt"))
                File.Delete("inventoryitems.txt");

            _inventoryItemRepository = new FileInventoryItemRepository("inventoryitems.txt");
            LoadData();
        }

        private void LoadData()
        {
            _allItems = _inventoryItemRepository.GetAllInventoryItems().ToList();
            _filteredItems = new ObservableCollection<InventoryItem>(_allItems);

            // Opret kategorier
            Categories = new ObservableCollection<string>
            {
                "Alle kategorier"
            };

            // Tilføj enum-værdier konverteret til strings
            var categoryStrings = _allItems
                .Select(i => i.Material.MaterialCategory.ToString())  // ÆNDRET FRA Category til MaterialCategory
                .Distinct()
                .OrderBy(c => c);

            Categories = new ObservableCollection<string>(
                Categories.Concat(categoryStrings)
            );

            // Opret lokationer
            Locations = new ObservableCollection<string>
        {
            "Alle lokationer"
        };
            Locations = new ObservableCollection<string>(
                Locations.Concat(_allItems.Select(i => i.Storage.StorageName).Distinct().OrderBy(l => l))
            );

            ApplyFilters();
        }

        public void ApplyFilters()
        {
            var filtered = _allItems.AsEnumerable();

            // Søgning
            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(i =>
                    i.Material.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Kategori filter
            if (SelectedCategory != "Alle kategorier")
            {
                filtered = filtered.Where(i =>
                    i.Material.MaterialCategory.ToString() == SelectedCategory  // ÆNDRET FRA Category til MaterialCategory
                );
            }

            // Lokation filter
            if (SelectedLocation != "Alle lokationer")
            {
                filtered = filtered.Where(i => i.Storage.StorageName == SelectedLocation);
            }

            // Opdater filtreret collection
            _filteredItems.Clear();
            foreach (var item in filtered)
            {
                _filteredItems.Add(item);
            }

            OnPropertyChanged(nameof(FilteredItems));
        }

        // Properties med PropertyChanged
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters();
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                ApplyFilters();
            }
        }

        public string SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged(nameof(SelectedLocation));
                ApplyFilters();
            }
        }

        // Øvrige properties
        public ObservableCollection<InventoryItem> FilteredItems => _filteredItems;
        public ObservableCollection<string> Categories { get; private set; }
        public ObservableCollection<string> Locations { get; private set; }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
