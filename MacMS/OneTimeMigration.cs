using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.Data;
using Newtonsoft.Json;
using System.Threading;
using System.Text;

namespace EquipmentManagementSystem {


    public static class OneTimeMigration {


        public async static void GetJson(EquipmentHandler equipRepo, OwnerHandler ownerRepo) {

            List<Mac> Macs = new List<Mac>();
            List<Owner> Owners = new List<Owner>();
            List<Equipment> Equip = new List<Equipment>();

            string json = File.ReadAllText(@"C:\Users\peter\source\repos\MDMScrape\MDMScrape\bin\Debug\netcoreapp2.1\Macs.json");
            
            Macs = JsonConvert.DeserializeObject<List<Mac>>(json);

            for (int i = 0; i < Macs.Count; i++) {

                Equip.Add(Macs[i]);
                Equip[Equip.Count - 1].EquipType = Equipment.EquipmentType.Mac;
                Owners.Add(Macs[i].Owner);
            }

            var eqpTasks = new List<Task>();
            var ownerTasks = new List<Task>();

            var owner = Owners.Distinct().ToList();

            for (int i = 0; i < Equip.Count; i++) {

             await equipRepo.Insert(Equip[i], false);
                if (i % 20 == 0) { equipRepo.Save(); }
            }

            for (int i = 0; i < owner.Count; i++) {

                await ownerRepo.Insert(owner[i], false);

                if (i % 20 == 0) { ownerRepo.Save(); }
            }

        }
        
    }
}
