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

        public IEnumerable<Material> GetAllMaterials()
        {
            // Hvis filen ikke findes eller er tom, seed med standard-materialer
            if (!File.Exists(Filepath) || !File.ReadLines(Filepath).Any())
            {
                SeedInitialMaterials();
            }

            try
            {
                return File.ReadLines(Filepath)
                           .Select(line => Material.FromString(line))
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

        private void SeedInitialMaterials()
        {
            // Definér et sæt “standard” materialer
            var defaults = new List<Material>
        {
            new Material(Material.Category.Befæstelse, "Bolte M10x50", 3, Material.Unit.Pakker),
            new Material(Material.Category.Gas, "CO2", 5, Material.Unit.Flasker),
            new Material(Material.Category.Befæstelse, "Møtrikker M8", 3,Material.Unit.Pakker),
            new Material(Material.Category.Stål, "Stålplade 2mm", 1, Material.Unit.Plader)
        };

            // Skriv dem til fil (overskriv eksisterende eller opret ny)
            try
            {
                using var sw = new StreamWriter(Filepath, false);
                foreach (var mat in defaults)
                {
                    sw.WriteLine(mat.ToString());
                }
            }
            catch (IOException ioEx)
            {
                throw new Exception("IO error seeding materials to file", ioEx);
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
