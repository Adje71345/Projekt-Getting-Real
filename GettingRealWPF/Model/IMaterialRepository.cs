using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public interface IMaterialRepository
    {
        public IEnumerable<Material> GetAllMaterials();
        public void AddInventoryItem(InventoryItem inventoryItem) { }
        public void UpdateInventoryItem(InventoryItem inventoryItem) { }
        public void DeleteInventoryItem(string materialDescription, string location) { }
        public void IncreaseAmount(string materialDescription, string location, int amount) { }
        public void DecreaseAmount(string materialDescription, string location, int amount) { }
        public void MoveInventoryItem(string materialDescription, string currentLocation, string newLocation) { }
    }
}
