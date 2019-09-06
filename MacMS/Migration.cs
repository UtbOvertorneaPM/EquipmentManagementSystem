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


        public async Task<IEnumerable<Equipment>> InsertMacServiceJson(IFormFile file) {

            List<Mac> macs = new List<Mac>();
            List<Equipment> equip = new List<Equipment>();
            var owner = new List<Owner>();
            var dateToday = DateTime.Now;

            string json = GetFileAsJson(file);

            if (file.FileName.Contains("vit")) {

                equip = await Task.Run(() => JsonConvert.DeserializeObject<List<Equipment>>(json));

                for (int i = 0; i < equip.Count; i++) {

                    equip[i].EquipType = Equipment.EquipmentType.Mac;
                }
            }
            else {
                macs = JsonConvert.DeserializeObject<List<Mac>>(json);

                for (int i = 0; i < macs.Count; i++) {

                    equip.Add(macs[i]);
                    equip[equip.Count - 1].EquipType = Equipment.EquipmentType.Mac;
                }

            }


            return equip;
        }


        public async Task<IEnumerable<T>> InsertBackupJson<T>(IFormFile file, bool IsEquipment) where T : class{

            var json = GetFileAsJson(file);
            var data = Enumerable.Empty<T>();

            if (IsEquipment) {

                data = await Task.Run(() => JsonConvert.DeserializeObject<List<T>>(json));
            }
            else {

                data = await Task.Run(() => JsonConvert.DeserializeObject<List<T>>(json));
            }

            return data;
        }

        
        public List<Equipment> InsertRandomData() {

            var usersToAdd = 500;
            var equipmentToAdd = 500;

            const string chars = "abcdefghijklmnopqrstuvxyzw";
            const string numbers = "123456789";

            var random = new Random();

            var owners = new List<Owner>();
            var equipment = new List<Equipment>();

            for (int i = 0; i < usersToAdd; i++) {

                var owner = new Owner {
                    FirstName = new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray()),

                    LastName = new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray()),
                    Added = DateTime.Now,
                    LastEdited = DateTime.Now
                };

                owners.Add(owner);
            }

            for (int i = 0; i < equipmentToAdd; i++) {

                var eqp = new Equipment {
                    Serial = new string(Enumerable.Repeat(numbers, 15)
                    .Select(s => s[random.Next(s.Length)]).ToArray()),

                    Model = new string(new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray()) +
                    new string(Enumerable.Repeat(numbers, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray())),

                    LastEdited = DateTime.Now,
                    EquipType = Equipment.EquipmentType.Laptop,
                    Owner = owners[i]
                };

                equipment.Add(eqp);
            }

            return equipment;
            
        }

        


        private string GetFileAsJson(IFormFile file) {

            using (var stream = new StreamReader(file.OpenReadStream())) {

                return stream.ReadToEnd();
            }

        }
        
    }
}
