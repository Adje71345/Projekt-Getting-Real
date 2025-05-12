using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public class FileInventoryItemRepository : IInventoryItemRepository
    {
        public string Filepath { get; set; } = "inventoryitems.txt";
        public FileInventoryItemRepository(string filepath)
        {
            Filepath = filepath;
        }

        //Henter alle InventoryItems fra filen og returnerer dem som en liste
        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            if (!File.Exists(Filepath)) // Tjek, om filen overhovedet findes
                return Enumerable.Empty<InventoryItem>();
            try
            {
                // Læs linjerne én ad gangen og konverter dem til InventoryItem-objekter
                return File.ReadLines(Filepath)
                           .Select(line => InventoryItem.FromString(line))
                           .ToList();
            }
            catch (IOException ioEx) // Andre IO-problemer
            {
                throw new Exception("IO error reading materials from file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error reading materials from file", ex);
            }
        }

        //Henter et InventoryItem fra filen og konverterer det til et InventoryItem-objekt
        public InventoryItem? GetInventoryItem(string materialDescription, string location)
        {
            return GetAllInventoryItems()
                .FirstOrDefault(i => i.Material.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase) &&
                                     i.Storage.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        //Tilføjer et InventoryItem til filen
        public void AddInventoryItem(InventoryItem inventoryItem)
        {
            if (GetInventoryItem(inventoryItem.Material.Description, inventoryItem.Storage.StorageName) != null) // Tjekker, om InventoryItem allerede findes
                throw new ArgumentException($"InventoryItem with material {inventoryItem.Material.Description} and location {inventoryItem.Storage.StorageName} already exists.");
            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, true))
                {
                    sw.WriteLine(inventoryItem.ToString());
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error writing materials to file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error writing materials to file", ex);
            }
        }

        //Opdaterer et InventoryItem i filen
        public void UpdateInventoryItem(InventoryItem inventoryItem)
        {
            // Find det eksisterende InventoryItem
            var existingItem = GetInventoryItem(inventoryItem.Material.Description, inventoryItem.Storage.StorageName);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {inventoryItem.Material.Description} and location {inventoryItem.Storage.StorageName} does not exist.");
            // Opdaterer det eksisterende InventoryItem
            var allItems = GetAllInventoryItems().ToList();
            allItems.Remove(existingItem);
            allItems.Add(inventoryItem);
            // Skriver de opdaterede InventoryItems tilbage til filen
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
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error writing materials to file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error writing materials to file", ex);
            }
        }

        //Sletter et InventoryItem fra filen
        public void DeleteInventoryItem(string materialDescription, string location)
        {
            // Find det eksisterende InventoryItem
            var existingItem = GetInventoryItem(materialDescription, location);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {location} does not exist.");
            // Sletter det eksisterende InventoryItem
            var allItems = GetAllInventoryItems().ToList();
            allItems.Remove(existingItem);
            // Skriver de andre InventoryItems tilbage til filen
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
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error writing materials to file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error writing materials to file", ex);
            }
        }

        //Tilføjer et antal til mængden af et InventoryItem i filen
        public void IncreaseAmount(string materialDescription, string location, int amount)
        {
            // Find det eksisterende InventoryItem
            var existingItem = GetInventoryItem(materialDescription, location);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {location} does not exist.");
            // Opdaterer mængden
            existingItem.Amount += amount;
            // Opdaterer det eksisterende InventoryItem
            UpdateInventoryItem(existingItem);
        }

        //Reducerer mængden af et InventoryItem i filen
        public void DecreaseAmount(string materialDescription, string location, int amount)
        {
            // Find det eksisterende InventoryItem
            var existingItem = GetInventoryItem(materialDescription, location);
            if (existingItem == null)
                throw new ArgumentException($"InventoryItem with material {materialDescription} and location {location} does not exist.");
            int newAmount=Math.Max(0, existingItem.Amount - amount); // Sikrer at mængden ikke bliver negativ
            // Opdaterer mængden
            existingItem.Amount -= amount;
            // Opdaterer det eksisterende InventoryItem
            UpdateInventoryItem(existingItem);
        }
    }
}
