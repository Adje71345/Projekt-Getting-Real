using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public class InventoryItem
    {
        public Material Material { get; private set; }
        public int Amount { get; set; }
        public Storage Storage { get; private set; }

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
            string[] parts = input.Split(',');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid input format for InventoryItem.");
            Material material = Material.FromString(parts[0]);
            int amount = int.Parse(parts[1]);
            Storage storage = Storage.FromString(parts[2]);
            return new InventoryItem(material, amount, storage);
        }
    }
}
