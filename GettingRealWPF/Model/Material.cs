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
        public string Category { get; set; }
        public string Description { get; set; }
        public int MinimumAmount { get; set; }
        public Unit MaterialUnit { get; set; }

        public enum Unit
        {
            Kg,
            Ruller,
            Flasker,
            Plader,
            Længde,
            Pakker,
        }

        public Material(string category, string description, int minimumAmount, Unit materialUnit)
        {
            Category = category;
            Description = description;
            MinimumAmount = minimumAmount;
            MaterialUnit = materialUnit;
        }

        public override string ToString()
        {
            return $"{Category},{Description},{MinimumAmount},{MaterialUnit}";
        }

        public static Material FromString(string input)
        {
            string[] parts = input.Split(',');
            if (parts.Length != 4)
            {
                throw new ArgumentException("Invalid input format");
            }
            string category = parts[0];
            string description = parts[1];
            int minimumAmount = int.Parse(parts[2]);
            Unit materialUnit = (Unit)Enum.Parse(typeof(Unit), parts[3]);
            return new Material(category, description, minimumAmount, materialUnit);
        }
    }
}
