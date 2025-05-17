using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GettingRealWPF.Model
{
    public class FileStorageRepository : IStorageRepository
    {
        public string Filepath { get; set; } = "storages.txt";
        public FileStorageRepository(string filepath)
        {
            Filepath = filepath;
        }

        public IEnumerable<Storage> GetAllStorages()
        {
            // Seed, hvis filen ikke findes eller er tom
            if (!File.Exists(Filepath) || !File.ReadLines(Filepath).Any())
            {
                SeedInitialStorages();
            }

            try
            {
                return File.ReadLines(Filepath)
                           .Select(line => Storage.FromString(line))
                           .ToList();
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error reading storages from file", ioEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading storages from file", ex);
            }
        }

        private void SeedInitialStorages()
        {
            var defaults = new List<Storage>
        {
            new Storage("Hovedlager"),
            new Storage("Bil - Mads"),
            new Storage("Bil - Martin"),
            new Storage("Bil - David")
        };

            try
            {
                using var sw = new StreamWriter(Filepath, false);
                foreach (var s in defaults)
                {
                    sw.WriteLine(s.ToString());
                }
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error writing seed storages to file", ioEx);
            }
        }

        //Henter et lager fra filen og konverterer det til et Storage-objekt
        public Storage? GetStorage(string location)
        {
            return GetAllStorages()
                .FirstOrDefault(s => s.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        //Tilføjer et lager til filen
        public void AddStorage(Storage storage)
        {
            if (GetStorage(storage.StorageName) != null) // Tjekker, om lageret allerede findes
                throw new ArgumentException($"Storage with location {storage.StorageName} already exists.");
            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, true))
                {
                    sw.WriteLine(storage.ToString());
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error writing storage to file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error writing storage to file", ex);
            }
        }

        //Opdaterer et lager i filen
        public void UpdateStorage(Storage storage)
        {
            if (GetStorage(storage.StorageName) == null) // Tjekker, om lageret findes
                throw new ArgumentException($"Storage with location {storage.StorageName} does not exist.");
            try
            {
                var storages = GetAllStorages().ToList();
                var index = storages.FindIndex(s => s.StorageName.Equals(storage.StorageName, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    storages[index] = storage;
                    File.WriteAllLines(Filepath, storages.Select(s => s.ToString()));
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error updating storage in file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error updating storage in file", ex);
            }
        }
        public void DeleteStorage(string location)
        {
            if (GetStorage(location) == null) // Tjekker, om storage findes
                throw new ArgumentException($"Storage with location {location} does not exist.");
            try
            {
                var storages = GetAllStorages().ToList();
                var index = storages.FindIndex(s => s.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    storages.RemoveAt(index);
                    File.WriteAllLines(Filepath, storages.Select(s => s.ToString()));
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error deleting storage from file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error deleting storage from file", ex);
            }
        }
    }
}
