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
        public IEnumerable<Material> GetMaterialsByCategory(Material.Category category);
        public Material? GetMaterialByDescription(string materialDescription);
        public void AddMaterial(Material material) { }
        public void UpdateMaterial(Material material) { }
        public void DeleteMaterial(string materialDescription) { }
    }
}
