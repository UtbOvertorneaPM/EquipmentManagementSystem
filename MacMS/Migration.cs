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
using Microsoft.AspNetCore.Http;

namespace EquipmentManagementSystem {


    public class DataMigrations {


        public void InsertMacServiceJson(EquipmentHandler equipRepo, OwnerHandler ownerRepo, IFormFile file) {

            List<Mac> macs = new List<Mac>();
            List<Owner> owners = new List<Owner>();
            List<Equipment> equip = new List<Equipment>();

            var dateToday = DateTime.Now;

            string json = GetFileAsJson(file);

            macs = JsonConvert.DeserializeObject<List<Mac>>(json);

            for (int i = 0; i < macs.Count; i++) {

                equip.Add(macs[i]);
                equip[equip.Count - 1].EquipType = Equipment.EquipmentType.Mac;
                owners.Add(macs[i].Owner);
            }

            var eqpTasks = new List<Task>();
            var ownerTasks = new List<Task>();

            var owner = owners.Distinct().ToList();

            for (int i = 0; i < owner.Count; i++) {

                owner[i].LastEdited = dateToday;
                ownerRepo.Insert(owner[i], false);

                if (i % 100 == 0) { ownerRepo.Save(); }
            }

            ownerRepo.Save();

            for (int i = 0; i < equip.Count; i++) {

                equipRepo.Insert(equip[i], false);
            }

            equipRepo.Save();


        }


        public void InsertBackupJson(IFormFile file, bool IsEquipment, EquipmentHandler equipRepo, OwnerHandler ownerRepo) {

            string json = GetFileAsJson(file);

            if (IsEquipment) {

                var equip = JsonConvert.DeserializeObject<List<Equipment>>(json);

                for (int i = 0; i < equip.Count; i++) {

                    equipRepo.Insert(equip[i], false);
                    if (i % 20 == 0) { equipRepo.Save(); }
                }

                equipRepo.Save();
            }
            else {

                List<Owner> owners = new List<Owner>();

                for (int i = 0; i < owners.Count; i++) {

                    ownerRepo.Insert(owners[i], false);

                    if (i % 20 == 0) { ownerRepo.Save(); }
                }

                ownerRepo.Save();
            }
        }


        public void InsertRandomData(EquipmentHandler equipRepo, OwnerHandler ownerRepo) {

            var usersToAdd = 500;
            var equipmentToAdd = 500;

            const string chars = "abcdefghijklmnopqrstuvxyzw";
            const string numbers = "123456789";

            var random = new Random();

            var owners = new List<Owner>();
            var equipment = new List<Equipment>();

            for (int i = 0; i < usersToAdd; i++) {

                var owner = new Owner();

                owner.FirstName = new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                owner.LastName = new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                owner.LastEdited = DateTime.Now;

                owners.Add(owner);
            }

            for (int i = 0; i < equipmentToAdd; i++) {

                var eqp = new Equipment();

                eqp.Serial = new string(Enumerable.Repeat(numbers, 15)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                eqp.Model = new string(new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray()) +
                    new string(Enumerable.Repeat(numbers, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray()));

                eqp.LastEdited = DateTime.Now;
                eqp.EquipType = Equipment.EquipmentType.Laptop;

                equipment.Add(eqp);
            }

            for (int i = 0; i < equipment.Count; i++) {

                equipRepo.Insert(equipment[i], false);
                if (i % 20 == 0) { equipRepo.Save(); }
            }

            equipRepo.Save();

            for (int i = 0; i < owners.Count; i++) {

                ownerRepo.Insert(owners[i], false);

                if (i % 20 == 0) { ownerRepo.Save(); }
            }

            ownerRepo.Save();
        }


        private string GetFileAsJson(IFormFile file) {

            using (var stream = new StreamReader(file.OpenReadStream())) {

                return stream.ReadToEnd();
            }

        }
        
    }
}
