using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingRealWPF.Model
{
    public interface IStorageRepository
    {
        public IEnumerable<Storage> GetAllStorages();
        public Storage? GetStorage(string location);
        public void AddStorage(Storage storage);
        public void UpdateStorage(Storage storage);
        public void DeleteStorage(string location);
    }
}
