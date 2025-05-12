using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GettingRealWPF.Model
{
    public class Storage
    {
        public string StorageName { get; set; }

        public Storage(string storageName)
        {
            StorageName = storageName;
        }
        public override string ToString()
        {
            return $"{StorageName}";
        }
        public static Storage FromString(string input)
        {
            return new Storage(input);
        }
    }
}
