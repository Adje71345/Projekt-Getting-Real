using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public class FileMaterialRepository : IMaterialRepository
    {
        public string Filepath { get; set; } = "materials.txt";
        public FileMaterialRepository(string filepath)
        {
            Filepath = filepath;
        }

        //Henter alle materialer fra filen og konverterer dem til Material-objekter
        public IEnumerable<Material> GetAllMaterials()
        {
            if (!File.Exists(Filepath)) // Tjek, om filen overhovedet findes
                return Enumerable.Empty<Material>();
            try
            {
                // Læs linjerne én ad gangen og konverter dem til Material-objekter
                return File.ReadLines(Filepath)
                           .Select(line => Material.FromString(line))
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

        //Henter et materiale fra filen og konverterer det til et Material-objekt
        public Material? GetMaterialByDescription(string materialDescription)
        {
            return GetAllMaterials()
                .FirstOrDefault(m => m.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase));
        }

        //Tilføjer et materiale til filen
        public void AddMaterial(Material material)
        {
            if (GetMaterialByDescription(material.Description) != null) // Tjekker, om materialet allerede findes
                throw new ArgumentException($"Material with name {material.Description} already exists.");
            try
            {
                using (StreamWriter sw = new StreamWriter(Filepath, true))
                {
                    sw.WriteLine(material.ToString());
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error writing material to file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error writing material to file", ex);
            }
        }

        //Opdaterer et materiale i filen
        public void UpdateMaterial(Material material)
        {
            if (GetMaterialByDescription(material.Description) == null) // Tjekker, om materialet findes
                throw new ArgumentException($"Material with name {material.Description} does not exist.");
            try
            {
                var materials = GetAllMaterials().ToList();
                var index = materials.FindIndex(m => m.Description.Equals(material.Description, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    materials[index] = material;
                    File.WriteAllLines(Filepath, materials.Select(m => m.ToString()));
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error updating material in file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error updating material in file", ex);
            }
        }

        //Sletter et materiale fra filen
        public void DeleteMaterial(string materialDescription)
        {
            if (GetMaterialByDescription(materialDescription) == null) // Tjekker, om materialet findes
                throw new ArgumentException($"Material with name {materialDescription} does not exist.");
            try
            {
                var materials = GetAllMaterials().ToList();
                var index = materials.FindIndex(m => m.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    materials.RemoveAt(index);
                    File.WriteAllLines(Filepath, materials.Select(m => m.ToString()));
                }
            }
            catch (IOException ioEx) // IO-undtagelser
            {
                throw new Exception("IO error deleting material from file", ioEx);
            }
            catch (Exception ex) // Andre undtagelser.
            {
                throw new Exception("Error deleting material from file", ex);
            }
        }
    }
}
