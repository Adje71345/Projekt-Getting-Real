using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public class InventoryItem
    {
        public Material Material { get; set; }
        public int Amount { get; set; }
        public Storage Storage { get; set; }

        public InventoryItem(Material material, int amount, Storage storage)
        {
            Material = material;
            Amount = amount;
            Storage = storage;
        }
        public override string ToString()
        {
            return $"{Material.ToString()},{Amount},{Storage.ToString()}";
        }

        public static InventoryItem FromString(string input)
        {
            // Split i alle kommaer
            var parts = input.Split(',');

            // materialString er alt undtagen de sidste to felter
            var materialString = string.Join(",", parts.Take(parts.Length - 2));

            // amount er næstsidste felt
            if (!int.TryParse(parts[parts.Length - 2], out var amount))
                throw new ArgumentException("Invalid amount in InventoryItem string.");

            // storageString er sidste felt
            var storageString = parts.Last();

            // Pak material- og storage-strengene til objekter
            var material = Material.FromString(materialString);
            var storage = Storage.FromString(storageString);

            return new InventoryItem(material, amount, storage);
        }

    }
}
