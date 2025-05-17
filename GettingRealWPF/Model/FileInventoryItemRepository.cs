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
            // Seed, hvis filen ikke findes eller er tom
            if (!File.Exists(Filepath) || !File.ReadLines(Filepath).Any())
            {
                SeedInitialInventoryItems();
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

        private void SeedInitialInventoryItems()
        {
            // Hent de seedede materialer og lagre
            var materialRepo = new FileMaterialRepository("materials.txt");
            var storageRepo = new FileStorageRepository("storages.txt");

            var materials = materialRepo.GetAllMaterials().ToList();
            var storages = storageRepo.GetAllStorages().ToList();

            // Vælg nogle standard-kombinationer (antag at de findes)
            var bolt = materials.First(m => m.Description == "M10 bolt");
            var co2 = materials.First(m => m.Description == "CO2 flaske");
            var hovedlager = storages.First(s => s.StorageName == "Hovedlager");
            var reservedel = storages.First(s => s.StorageName == "Bil - Mads");

            var defaults = new List<InventoryItem>
        {
            new InventoryItem(bolt,  50, hovedlager),
            new InventoryItem(co2,   10, reservedel)
        };

            // Skriv dem til fil (overskriv eller opret ny fil)
            try
            {
                using var sw = new StreamWriter(Filepath, false);
                foreach (var item in defaults)
                    sw.WriteLine(item.ToString());
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error writing seed inventory items to file", ioEx);
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

            // Skriv hele listen tilbage til fil
            try
            {
                using var sw = new StreamWriter(Filepath, false);
                foreach (var item in allItems)
                    sw.WriteLine(item.ToString());
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

            // Opret det opdaterede InventoryItem
            var updatedItem = new InventoryItem(existingItem.Material, existingItem.Amount, newStorage);

            var allItems = GetAllInventoryItems().ToList();
            allItems.Remove(existingItem); // Fjern det gamle objekt baseret på beskrivelse og lokation
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