using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public class MockInventoryItemRepository : IInventoryItemRepository
    {
        private List<InventoryItem> _items;

        public MockInventoryItemRepository()
        {
            // Test data
            _items = CreateMockData();
        }

        // IInventoryItemRepository implementation
        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            return _items;
        }

        public InventoryItem? GetInventoryItem(string materialDescription, string location)
        {
            return _items.FirstOrDefault(i =>
            i.Material.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase) &&
            i.Storage.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        //Implementere IInventoryItemRepository metoder-  vises ikke i LagerListeView
        public void AddInventoryItem(InventoryItem inventoryItem) { }
        public void UpdateInventoryItem(InventoryItem inventoryItem) { }
        public void DeleteInventoryItem(string materialDescription, string location) { }
        public void IncreaseAmount(string materialDescription, string location, int amount) { }
        public void DecreaseAmount(string materialDescription, string location, int amount) { }
        public void MoveInventoryItem(string materialDescription, string currentLocation, string newLocation, int moveAmount) { }


        // Hjælpemetode til at oprette mock data
        private List<InventoryItem> CreateMockData()
        {
            var elektrode = new Material(Material.Category.Svejsning, "6010 elektroder 2.5 mm", 3, Material.Unit.Pakker);
            var stålplade = new Material(Material.Category.Stål, "Stålplade 2mm", 2, Material.Unit.Plader);
            var vjern = new Material(Material.Category.Stål, "Vinkeljern 30x30x3mm", 3, Material.Unit.Længder);
            var bolt = new Material(Material.Category.Befæstelse, "Bolte M10x50mm", 5, Material.Unit.Pakker);
            var møtrik = new Material(Material.Category.Befæstelse, "Møtrik M10", 5, Material.Unit.Pakker);
            var skiver = new Material(Material.Category.Slibning, "Skrubskriver 125 mm", 5, Material.Unit.Pakker);
            var oxy = new Material(Material.Category.Gas, "Oxygen", 5, Material.Unit.Flasker);
            var acetylen = new Material(Material.Category.Gas, "Acetylen", 5, Material.Unit.Flasker);
            var svhandsker = new Material(Material.Category.Sikkerhed, "Svejsehandsker", 5, Material.Unit.Pakker);
            var svbriller = new Material(Material.Category.Sikkerhed, "Svejsebriller", 5, Material.Unit.Pakker);
            var svmaske = new Material(Material.Category.Sikkerhed, "Svejsemasker", 5, Material.Unit.Pakker);
            var svmaske2 = new Material(Material.Category.Sikkerhed, "Svejsemasker 2", 5, Material.Unit.Pakker);
            var svmaske3 = new Material(Material.Category.Sikkerhed, "Svejsemasker 3", 5, Material.Unit.Pakker);
            var svmaske4 = new Material(Material.Category.Sikkerhed, "Svejsemasker 4", 5, Material.Unit.Pakker);
            var svmaske5 = new Material(Material.Category.Sikkerhed, "Svejsemasker 5", 5, Material.Unit.Pakker);
            var svmaske6 = new Material(Material.Category.Sikkerhed, "Svejsemasker 6", 5, Material.Unit.Pakker);
            var svmaske7 = new Material(Material.Category.Sikkerhed, "Svejsemasker 7", 5, Material.Unit.Pakker);
            var svmaske8 = new Material(Material.Category.Sikkerhed, "Svejsemasker 8", 5, Material.Unit.Pakker);
            var svmaske9 = new Material(Material.Category.Sikkerhed, "Svejsemasker 9", 5, Material.Unit.Pakker);
            var svmaske10 = new Material(Material.Category.Sikkerhed, "Svejsemasker 10", 5, Material.Unit.Pakker);

            // Opret lagerlokationer
            var lager1 = new Storage("Hovedlager");
            var lager2 = new Storage("Firmabil (D)");
            var lager3 = new Storage("Firmabil (M)");
            var lager4 = new Storage("Firmabil (C)");

            // Opret inventory items
            return new List<InventoryItem>
                {
                    new InventoryItem(elektrode, 5, lager1),
                    new InventoryItem(stålplade, 2, lager1),
                    new InventoryItem(vjern, 3, lager1),
                    new InventoryItem(bolt, 5, lager1),
                    new InventoryItem(møtrik, 5, lager1),
                    new InventoryItem(skiver, 5, lager1),
                    new InventoryItem(oxy, 5, lager1),
                    new InventoryItem(acetylen, 5, lager1),
                    new InventoryItem(svhandsker, 5, lager1)                        ,
                    new InventoryItem(elektrode, 5, lager2),
                    new InventoryItem(stålplade, 2, lager2),
                    new InventoryItem(vjern, 3, lager2),
                    new InventoryItem(bolt, 5, lager2),
                    new InventoryItem(møtrik, 5, lager2),
                    new InventoryItem(skiver, 5, lager2),
                    new InventoryItem(oxy, 5, lager2),
                    new InventoryItem(acetylen, 5, lager2),
                    new InventoryItem(svhandsker, 5, lager2),
                    new InventoryItem(elektrode, 5, lager3),
                    new InventoryItem(stålplade, 2, lager3),
                    new InventoryItem(vjern, 3, lager3),
                    new InventoryItem(bolt, 5, lager3),
                    new InventoryItem(møtrik, 5, lager3),
                    new InventoryItem(skiver, 5, lager3),
                    new InventoryItem(møtrik, 5, lager2),
                    new InventoryItem(skiver, 5, lager2),
                    new InventoryItem(oxy, 5, lager2),
                    new InventoryItem(acetylen, 5, lager2),
                    new InventoryItem(svhandsker, 5, lager2),
                    new InventoryItem(elektrode, 5, lager3),
                    new InventoryItem(stålplade, 2, lager3),
                    new InventoryItem(vjern, 3, lager3)
            };
        }
    }
}





