using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace GettingRealWPF.Model
{
    public class MockInventoryItemRepository : IInventoryItemRepository
    {
        private List<InventoryItem> _items;

        public MockInventoryItemRepository()
        {
            // Test data
            _items = CreateMockData();
        }

        // IInventoryItemRepository implementation
        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            return _items;
        }

        public InventoryItem? GetInventoryItem(string materialDescription, string location)
        {
            return _items.FirstOrDefault(i =>
            i.Material.Description.Equals(materialDescription, StringComparison.OrdinalIgnoreCase) &&
            i.Storage.StorageName.Equals(location, StringComparison.OrdinalIgnoreCase));
        }

        //Implementere IInventoryItemRepository metoder-  vises ikke i LagerListeView
        public void AddInventoryItem(InventoryItem inventoryItem) { }
        public void UpdateInventoryItem(InventoryItem inventoryItem) { }
        public void DeleteInventoryItem(string materialDescription, string location) { }
        public void IncreaseAmount(string materialDescription, string location, int amount) { }
        public void DecreaseAmount(string materialDescription, string location, int amount) { }
        public void MoveInventoryItem(string materialDescription, string currentLocation, string newLocation, int moveAmount) { }

       

        // Hjælpemetode til at oprette mock data
        private List<InventoryItem> CreateMockData()
        {
            var elektrode = new Material(Material.Category.Svejsning, "6010 elektroder 2.5mm", 0, Material.Unit.Pakker);
            var elektrode1 = new Material(Material.Category.Svejsning, "6011 elektroder 3.2mm", 0, Material.Unit.Pakker);
            var elektrode2 = new Material(Material.Category.Svejsning, "7018 elektroder 2.5mm", 0, Material.Unit.Pakker);
            var elektrode3 = new Material(Material.Category.Svejsning, "7024 elektroder 4.0mm", 0, Material.Unit.Pakker);
            var mig = new Material(Material.Category.Svejsning, "MIG tråd 1.00 mmStål", 0, Material.Unit.Ruller);
            var mig1 = new Material(Material.Category.Svejsning, "MIG tråd 1.2 mmStål", 0, Material.Unit.Ruller);
            var mig2 = new Material(Material.Category.Svejsning, "MIG tråd 0.8 mmRustfri", 0, Material.Unit.Ruller);
            var mig3 = new Material(Material.Category.Svejsning, "MIG tråd 1.0 mmAluminium", 0, Material.Unit.Ruller);
            var rør = new Material(Material.Category.Svejsning, "Rør tråd 1.2mm", 0, Material.Unit.Ruller);
            var tig = new Material(Material.Category.Svejsning, "TIG tråd 1.6mm Stål", 0, Material.Unit.Pakker);
            var tig1 = new Material(Material.Category.Svejsning, "TIG tråd 2.4mm Rustfri", 0, Material.Unit.Pakker);
            var tig2 = new Material(Material.Category.Svejsning, "TIG tråd 3.2mm Aluminium", 0, Material.Unit.Pakker);
            var tråd = new Material(Material.Category.Svejsning, "Massiv tråd", 0, Material.Unit.Ruller);

            var stål = new Material(Material.Category.Stål, "Stålplade 2mm", 0, Material.Unit.Plader);
            var stål1 = new Material(Material.Category.Stål, "Stålplade 4mm", 0, Material.Unit.Plader);
            var stål2 = new Material(Material.Category.Stål, "Stålplade 6mm", 0, Material.Unit.Plader);
            var stål3 = new Material(Material.Category.Stål, " Rustfri stålplade 2mm", 0, Material.Unit.Plader);
            var vjern = new Material(Material.Category.Stål, "Vinkeljern 30x30x3mm", 0, Material.Unit.Længder);
            var vjern1 = new Material(Material.Category.Stål, "Vinkeljern 40x40x4mm", 3, Material.Unit.Længder);
            var fjern = new Material(Material.Category.Stål, "Fladjern 30x5 mm", 0, Material.Unit.Længder);
            var fprofil = new Material(Material.Category.Stål, "Firkant profil 40x40mm", 0, Material.Unit.Længder);
            var rør1 = new Material(Material.Category.Stål, "Rustfri rør ø50x5mm", 0, Material.Unit.Længder);
            var alu = new Material(Material.Category.Stål, "Aluminiumsplade 2 mm", 0, Material.Unit.Plader);
            var aprofil = new Material(Material.Category.Stål, "Aluminiumsprofil 40x40x4mm", 0, Material.Unit.Længder);

            var bolt = new Material(Material.Category.Befæstelse, "Bolte M8x50", 0, Material.Unit.Pakker);
            var bolt1 = new Material(Material.Category.Befæstelse, "Bolte M10x50", 0, Material.Unit.Pakker);
            var bolt2 = new Material(Material.Category.Befæstelse, "Bolte M12x50", 0, Material.Unit.Pakker);
            var bolt3 = new Material(Material.Category.Befæstelse, "Bolte M16x50", 0, Material.Unit.Pakker);
            var bolt4 = new Material(Material.Category.Befæstelse, "Bolte M24x50", 0, Material.Unit.Pakker);
            var møtrik = new Material(Material.Category.Befæstelse, "Møtrikker M8", 0, Material.Unit.Pakker);
            var møtrik1 = new Material(Material.Category.Befæstelse, "Møtrikker M10", 0, Material.Unit.Pakker);
            var møtrik2 = new Material(Material.Category.Befæstelse, "Møtrikker M12", 0, Material.Unit.Pakker);
            var møtrik3 = new Material(Material.Category.Befæstelse, "Møtrikker M16", 0, Material.Unit.Pakker);
            var møtrik4 = new Material(Material.Category.Befæstelse, "Møtrikker M24", 0, Material.Unit.Pakker);
            var skive = new Material(Material.Category.Befæstelse, "Skiver M8", 0, Material.Unit.Pakker);
            var skive1 = new Material(Material.Category.Befæstelse, "Skiver M10", 0, Material.Unit.Pakker);
            var skive2 = new Material(Material.Category.Befæstelse, "Skiver M12", 0, Material.Unit.Pakker);
            var skive3 = new Material(Material.Category.Befæstelse, "Skiver M16", 0, Material.Unit.Pakker);
            var skive4 = new Material(Material.Category.Befæstelse, "Skiver M24", 0, Material.Unit.Pakker);
            var skrue = new Material(Material.Category.Befæstelse, "Skruer M8x50", 0, Material.Unit.Pakker);
            var bnitter = new Material(Material.Category.Befæstelse, "Blindnitter Alu/Rustfri ø4x24mm", 0, Material.Unit.Pakker);
            var bnitter1 = new Material(Material.Category.Befæstelse, "Blindnitter Kobber/Alu ø3.2x12mm", 0, Material.Unit.Pakker);
            var bnitter2 = new Material(Material.Category.Befæstelse, "Blindnitter Alu/Kobber ø4x36mm", 0, Material.Unit.Pakker);

            var sskive = new Material(Material.Category.Befæstelse, "Skrubskiver 125mm", 0, Material.Unit.Stk);
            var sskive1 = new Material(Material.Category.Befæstelse, "Skrubskiver 150mm", 0, Material.Unit.Stk);
            var sskive2 = new Material(Material.Category.Befæstelse, "Skæreskiver 125mm", 0, Material.Unit.Stk);
            var sskive3 = new Material(Material.Category.Befæstelse, "Skæreskiver 150mm", 0, Material.Unit.Stk);
            var børste = new Material(Material.Category.Slibning, "Stålbørste til vinkelsliber", 0, Material.Unit.Stk);
            var pskive = new Material(Material.Category.Slibning, "Lamelpudseskiver 125mm", 0, Material.Unit.Stk);

            var gas = new Material(Material.Category.Gas, "Argon", 3, Material.Unit.Flasker);
            var gas1 = new Material(Material.Category.Gas, "CO2", 3, Material.Unit.Flasker);
            var gas2 = new Material(Material.Category.Gas, "Acetylen", 3, Material.Unit.Flasker);
            var gas3 = new Material(Material.Category.Gas, "Oxygen", 3, Material.Unit.Flasker);
            var gas4 = new Material(Material.Category.Gas, "Propan", 3, Material.Unit.Flasker);

            var handske = new Material(Material.Category.Sikkerhed, "Svejsehandsker", 0, Material.Unit.Par);
            var briller = new Material(Material.Category.Sikkerhed, "Svejsebriller", 0, Material.Unit.Stk);
            var filter = new Material(Material.Category.Sikkerhed, "Filtre til åndedrætsværn", 0, Material.Unit.Stk);
            var glas = new Material(Material.Category.Sikkerhed, "Svejseglas til hjelme", 0, Material.Unit.Stk);
            var handske1 = new Material(Material.Category.Sikkerhed, "Arbejdshandsker", 0, Material.Unit.Par);

            // Opret lagerlokationer
            var lager1 = new Storage("Hovedlager");
            var lager2 = new Storage("Bil - David");
            var lager3 = new Storage("Bil - Martin");
            var lager4 = new Storage("Bil - Mads");

            // Opret inventory items
            return new List<InventoryItem>
                {
                    new InventoryItem(elektrode, 12, lager1),
                    new InventoryItem(elektrode1, 7, lager1),
                    new InventoryItem(elektrode2, 9, lager1),
                    new InventoryItem(elektrode3, 5, lager1),
                    new InventoryItem(mig, 3, lager1),
                    new InventoryItem(mig1, 2, lager1),
                    new InventoryItem(mig2, 4, lager1),
                    new InventoryItem(mig3, 1, lager1),
                    new InventoryItem(rør, 6, lager1),
                    new InventoryItem(tig, 8, lager1),
                    new InventoryItem(tig1, 10, lager1),
                    new InventoryItem(tig2, 11, lager1),
                    new InventoryItem(tråd, 13, lager1),
                    new InventoryItem(stål, 7, lager1),
                    new InventoryItem(stål1, 4, lager1),
                    new InventoryItem(stål2, 3, lager1),
                    new InventoryItem(stål3, 5, lager1),
                    new InventoryItem(vjern, 2, lager1),
                    new InventoryItem(vjern1, 1, lager1),
                    new InventoryItem(fjern, 8, lager1),
                    new InventoryItem(fprofil, 6, lager1),
                    new InventoryItem(rør1, 9, lager1),
                    new InventoryItem(alu, 12, lager1),
                    new InventoryItem (aprofil, 7, lager1),
                    new InventoryItem (bolt, 10, lager1),
                    new InventoryItem (bolt1, 5, lager1),
                    new InventoryItem (bolt2, 3, lager1),
                    new InventoryItem (bolt3, 2, lager1),
                    new InventoryItem (bolt4, 1, lager1),
                    new InventoryItem (møtrik, 8, lager1),
                    new InventoryItem (møtrik1, 6, lager1),
                    new InventoryItem (møtrik2, 4, lager1),
                    new InventoryItem (møtrik3, 2, lager1),
                    new InventoryItem (møtrik4, 1, lager1),
                    new InventoryItem (skive, 8, lager1),
                    new InventoryItem (skive1, 6, lager1),
                    new InventoryItem (skive2, 4, lager1),
                    new InventoryItem (skive3, 2, lager1),
                    new InventoryItem (skive4, 1, lager1),
                    new InventoryItem (skrue, 8, lager1),
                    new InventoryItem (bnitter, 6, lager1),
                    new InventoryItem (bnitter1, 4, lager1),
                    new InventoryItem (bnitter2, 2, lager1),
                    new InventoryItem (sskive, 8, lager1),
                    new InventoryItem (sskive1, 6, lager1),
                    new InventoryItem (sskive2, 4, lager1),
                    new InventoryItem (sskive3, 2, lager1),
                    new InventoryItem (børste, 1, lager1),
                    new InventoryItem (pskive, 8, lager1),
                    new InventoryItem (gas, 6, lager1),
                    new InventoryItem (gas1, 4, lager1),
                    new InventoryItem (gas2, 6, lager1),
                    new InventoryItem (gas3, 4, lager1),
                    new InventoryItem (gas4, 8, lager1),
                    new InventoryItem (handske, 6, lager1),
                    new InventoryItem (briller, 4, lager1),
                    new InventoryItem (filter, 6, lager1),
                    new InventoryItem (glas, 4, lager1),
                    new InventoryItem (handske1, 8, lager1),

                    new InventoryItem(elektrode, 12, lager2),
                    new InventoryItem(elektrode1, 7, lager2),
                    new InventoryItem(møtrik4, 1, lager2),
                    new InventoryItem(handske1, 1, lager2),
                    new InventoryItem (sskive3, 1, lager2),
                    new InventoryItem (børste, 1, lager2),
                    new InventoryItem (pskive, 1, lager2),
                    new InventoryItem (gas, 1, lager2),
                    new InventoryItem (gas1, 1, lager2),
                    new InventoryItem (sskive1, 1, lager2),
                    new InventoryItem (tig, 1, lager2),
                    new InventoryItem (mig, 1, lager2),
                    new InventoryItem (mig1, 1, lager2),
                    new InventoryItem (mig2, 1, lager2),
                    new InventoryItem (mig3, 1, lager2),
                    new InventoryItem (rør, 1, lager2),
                    new InventoryItem (tig1, 1, lager2),


                    new InventoryItem (mig1, 1, lager3),
                    new InventoryItem (mig2, 1, lager3),
                    new InventoryItem (mig3, 1, lager3),
                    new InventoryItem (rør, 1, lager3),
                    new InventoryItem (tig1, 1, lager3),
                    new InventoryItem (elektrode, 1, lager3),
                    new InventoryItem (elektrode1, 1, lager3),
                    new InventoryItem (elektrode2, 1, lager3),
                    new InventoryItem (elektrode3, 1, lager3),
                    new InventoryItem (sskive, 1, lager3),
                    new InventoryItem (sskive2, 1, lager3),
                    new InventoryItem (sskive3, 1, lager3),
                    new InventoryItem (møtrik3 , 1, lager3),
                    new InventoryItem (handske1, 1, lager3),
                    new InventoryItem (sskive1, 1, lager3),
                    new InventoryItem (børste, 1, lager3),
                    new InventoryItem (pskive, 1, lager3),

                    new InventoryItem (mig, 1, lager4),
                    new InventoryItem (mig1, 1, lager4),
                    new InventoryItem (elektrode2 , 1, lager4),
                    new InventoryItem (elektrode3, 1, lager4),
                    new InventoryItem (tig, 1, lager4),
                    new InventoryItem (handske1, 1, lager4),
                    new InventoryItem (sskive, 1, lager4),
                    new InventoryItem (briller , 1, lager4),
                    new InventoryItem (filter, 1, lager4),
                    new InventoryItem (glas, 1, lager4),
                    new InventoryItem (sskive1, 1, lager4),
                    new InventoryItem (sskive2, 1, lager4),
                    new InventoryItem (sskive3, 1, lager4),
                    new InventoryItem (møtrik4, 1, lager4),



            };
        }
    }
}





