using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GettingRealWPF.Model
{
    public class Material
    {
        public Category MaterialCategory { get; set; }
        public string Description { get; set; }
        public int MinimumAmount { get; set; }
        public Unit MaterialUnit { get; set; }

        public enum Category
        {
            Svejsning,
            Stål,
            Befæstelse,
            Slibning,
            Gas,
            Sikkerhed,
        }

        public enum Unit
        {
            Par,
            Stk,
            Flasker,
            Pakker,
            Længder,
            Plader,
            Ruller,
        }

        public Material(Category materialCategory, string description, int minimumAmount, Unit materialUnit)
        {
            MaterialCategory = materialCategory;
            Description = description;
            MinimumAmount = minimumAmount;
            MaterialUnit = materialUnit;
        }

        public override string ToString()
        {
            return $"{MaterialCategory},{Description},{MinimumAmount},{MaterialUnit}";
        }

        public static Material FromString(string input)
        {
            string[] parts = input.Split(',');
            if (parts.Length != 4)
            {
                throw new ArgumentException("Invalid input format");
            }
            Category materialCategory = (Category)Enum.Parse(typeof(Category), parts[0]);
            string description = parts[1];
            int minimumAmount = int.Parse(parts[2]);
            Unit materialUnit = (Unit)Enum.Parse(typeof(Unit), parts[3]);
            return new Material(materialCategory, description, minimumAmount, materialUnit);
        }
    }
}
