using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GettingRealWPF.Model
{
    public class FileInventoryItemRepository : IInventoryItemRepository
    {
        public string Filepath { get; set; } = "inventoryitems.txt";

        public FileInventoryItemRepository(string filepath)
        {
            Filepath = filepath;
        }

        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            // Hvis filen ikke findes eller er tom, brug mock-data
            if (!File.Exists(Filepath) || !File.ReadLines(Filepath).Any())
            {
                var mockRepo = new MockInventoryItemRepository();
                var mockItems = mockRepo.GetAllInventoryItems().ToList();

                // Skriv mock-items til filen, så den bliver initialiseret
                SaveInventoryItemsToFile(mockItems);
            }

            try
            {
                return File.ReadLines(Filepath)
                           .Select(line => InventoryItem.FromString(line))
                           .ToList();
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error reading inventory items from file", ioEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading inventory items from file", ex);
            }
        }

        public InventoryItem? GetInventoryItem(string materialDescription, string location)
        {
            return GetAllInventoryItems()
                .FirstOrDefault(i => i.Material.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase) &&
                                     i.Storage.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        public void AddInventoryItem(InventoryItem inventoryItem)
        {
            if (GetInventoryItem(inventoryItem.Material.Description, inventoryItem.Storage.StorageName) != null)
                throw new ArgumentException($"InventoryItem with material {inventoryItem.Material.Description} and location {inventoryItem.Storage.StorageName} already exists.");

            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, true))
                {
                    sw.WriteLine(inventoryItem.ToString());
                }
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error writing materials to file", ioEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing materials to file", ex);
            }
        }

        private void SaveInventoryItemsToFile(IEnumerable<InventoryItem> items)
        {
            try
            {
                using var sw = new StreamWriter(Filepath, false);
                foreach (var item in items)
                {
                    sw.WriteLine(item.ToString());
                }
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error writing materials to file", ioEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing materials to file", ex);
            }
        }


        public void UpdateInventoryItem(InventoryItem inventoryItem)
        {
            // Hent alle items som en liste
            var allItems = GetAllInventoryItems().ToList();

            // Find index på det, der matcher dit materiale & lager
            int idx = allItems.FindIndex(i =>
                i.Material.Description.Equals(inventoryItem.Material.Description, StringComparison.OrdinalIgnoreCase) &&
                i.Storage.StorageName.Equals(inventoryItem.Storage.StorageName, StringComparison.OrdinalIgnoreCase));

            if (idx < 0)
                throw new ArgumentException($"InventoryItem with material {inventoryItem.Material.Description} " +
                                            $"and location {inventoryItem.Storage.StorageName} does not exist.");

            // Overskriv den gamle
            allItems[idx] = inventoryItem;

            SaveInventoryItemsToFile(allItems);
        }

        public void DeleteInventoryItem(string materialDescription, string location)
        {
            var allItems = GetAllInventoryItems().ToList();

            allItems.RemoveAll(item =>
                item.Material.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase) &&
                item.Storage.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));

            SaveInventoryItemsToFile(allItems);
        }

        public void IncreaseAmount(string materialDescription, string location, int amount)
        {
            var existingItem = GetInventoryItem(materialDescription, location);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {location} does not exist.");

            existingItem.Amount += amount;
            UpdateInventoryItem(existingItem);
        }

        public void DecreaseAmount(string materialDescription, string location, int amount)
        {
            var existingItem = GetInventoryItem(materialDescription, location);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {location} does not exist.");

            existingItem.Amount = Math.Max(0, existingItem.Amount - amount);
            UpdateInventoryItem(existingItem);
        }

        private void MoveFullInventoryItem(string materialDescription, string currentLocation, string newLocation)
        {
            var existingItem = GetInventoryItem(materialDescription, currentLocation);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {currentLocation} does not exist.");

            // Opret ny Storage og det opdaterede inventory-item
            var newStorage = new Storage(newLocation);
            var updatedItem = new InventoryItem(existingItem.Material, existingItem.Amount, newStorage);

            var allItems = GetAllInventoryItems().ToList();
            allItems.RemoveAll(item =>
                item.Material.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase) &&
                item.Storage.StorageName.Equals(currentLocation, StringComparison.OrdinalIgnoreCase));
            allItems.Add(updatedItem);

            SaveInventoryItemsToFile(allItems);
        }

        public void MoveInventoryItem(string materialDescription, string currentLocation, string newLocation, int moveAmount)
        {
            var existingItem = GetInventoryItem(materialDescription, currentLocation);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {currentLocation} does not exist.");

            // Hele flytning
            if (moveAmount == existingItem.Amount)
            {
                MoveFullInventoryItem(materialDescription, currentLocation, newLocation);
                DeleteInventoryItem(materialDescription, currentLocation); // Fjern den gamle post korrekt
                return;
            }

            //Dele flytning
            DecreaseAmount(materialDescription, currentLocation, moveAmount);
            var targetItem = GetInventoryItem(materialDescription, newLocation);

            if (targetItem != null)
            {
                IncreaseAmount(materialDescription, newLocation, moveAmount);
            }
            else
            {
                // Hvis der ikke findes et item, opret en ny post
                var newStorage = new Storage(newLocation);
                var movedItem = new InventoryItem(existingItem.Material, moveAmount, newStorage);

                var allItems = GetAllInventoryItems().ToList();
                allItems.Add(movedItem);
                SaveInventoryItemsToFile(allItems);
            }
        }
    }
}