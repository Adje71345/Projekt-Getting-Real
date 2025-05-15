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
            if (!File.Exists(Filepath))
                return Enumerable.Empty<InventoryItem>();

            try
            {
                return File.ReadLines(Filepath)
                           .Select(line => InventoryItem.FromString(line))
                           .ToList();
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error reading materials from file", ioEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading materials from file", ex);
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

        public void UpdateInventoryItem(InventoryItem inventoryItem)
        {
            var existingItem = GetInventoryItem(inventoryItem.Material.Description, inventoryItem.Storage.StorageName);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {inventoryItem.Material.Description} and location {inventoryItem.Storage.StorageName} does not exist.");

            var allItems = GetAllInventoryItems().ToList();
            allItems.Remove(existingItem);
            allItems.Add(inventoryItem);

            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, false))
                {
                    foreach (var item in allItems)
                    {
                        sw.WriteLine(item.ToString());
                    }
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

        public void DeleteInventoryItem(string materialDescription, string location)
        {
            var existingItem = GetInventoryItem(materialDescription, location);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {location} does not exist.");

            var allItems = GetAllInventoryItems().ToList();
            allItems.Remove(existingItem);

            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, false))
                {
                    foreach (var item in allItems)
                    {
                        sw.WriteLine(item.ToString());
                    }
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

        public void MoveInventoryItem(string materialDescription, string currentLocation, string newLocation)
        {
            var existingItem = GetInventoryItem(materialDescription, currentLocation);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {currentLocation} does not exist.");

            // Opret ny Storage med ny lokation
            var newStorage = new Storage(newLocation);

            // Brug constructor og sæt Id manuelt
            var updatedItem = new InventoryItem(existingItem.Material, existingItem.Amount, newStorage)
            {
                Id = existingItem.Id
            };

            var allItems = GetAllInventoryItems().ToList();
            allItems.Remove(existingItem);
            allItems.Add(updatedItem);

            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, false))
                {
                    foreach (var item in allItems)
                    {
                        sw.WriteLine(item.ToString());
                    }
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


    }
}