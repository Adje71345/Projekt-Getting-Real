using System.Collections.Generic;
using System.Linq;

public class Storage
{
    public List<Material> Materials { get; private set; }
    public string Location { get; set; }

    public Storage(string location)
    {
        Location = location;
        Materials = new List<Material>();
    }

    // Tilføjer et materiale
    public bool RegisterMaterial(Material material)
    {
        if (material == null) return false;

        Materials.Add(material);
        return true;
    }

    // Sletter et materiale ud fra beskrivelse - matcher materiale beskrivelsen med din beskrivelse
    public bool DeleteMaterial(string description)
    {
        var material = Materials.FirstOrDefault(m => m.Description == description);
        if (material == null) return false;

        Materials.Remove(material);
        return true;
    }

    // Redigerer et materiales egenskaber - matcher materiale beskrivelsen med din beskrivelse
    public bool EditMaterial(string description, string newCategory, string newDescription, int newAmount)
    {
        var material = Materials.FirstOrDefault(m => m.Description == description);
        if (material == null) return false;

        material.Category = newCategory;
        material.Description = newDescription;
        material.Amount = newAmount;
        return true;
    }

    // Trækker mængde fra et materiale
    public bool RemoveAmount(string description, int amountToRemove)
    {
        var material = Materials.FirstOrDefault(m => m.Description == description);
        if (material == null || amountToRemove <= 0 || material.Amount < amountToRemove) return false;

        material.Amount -= amountToRemove;
        return true;
    }

    // Returnerer hele lagerets indhold som tekst
    public string GetDescription()
    {
        if (Materials.Count == 0) return "No materials registered.";
        return string.Join("\n", Materials.Select(m => m.ToString()));
    }
}