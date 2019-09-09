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
using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data;
using System.Diagnostics;
using EquipmentManagementSystem.Domain.Data.Models;

namespace EquipmentManagementSystem.Domain.Data {


    public class DataMigrations {


        public async Task ImportMacServiceJson(IRequestHandler service, IFormFile file) {

            List<Equipment> equip = new List<Equipment>();
            var owners = new List<Owner>();
            var dateToday = DateTime.Now;

            string json = GetFileAsJson(file);

            var root = await Task.Run(() => JsonConvert.DeserializeObject<List<MacServiceModel.Rootobject>>(json));


            for (int i = 0; i < root.Count; i++) {

                var macOwner = root[i].Owner;
                var owner = new Owner();

                owner.FirstName = macOwner.FirstName;
                owner.LastName = macOwner.LastName;
                owner.SSN = macOwner.SSN;
                owner.Mail = macOwner.Mail;
                owner.Address = macOwner.Address;
                owner.TelNr = macOwner.TelNr;

                if (!string.IsNullOrEmpty(owner.FirstName)) {

                    owners.Add(owner);
                }

                var equipment = new Equipment();

                var mac = root[i];
                equipment.Serial = mac.Serial;
                equipment.Model = mac.Model;
                equipment.LastEdited = mac.Added;
                equipment.OwnerName = owner.FirstName + " " + owner.LastName;
                equipment.Notes = mac.Notes;

                equip.Add(equipment);
            }

            for (int i = 0; i < equip.Count; i++) {

                await service.Create<Equipment>(equip[i]);
            }

            for (int i = 0; i < owners.Count; i++) {

                await service.Create<Owner>(owners[i]);
            }


            return;
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


        public async Task InsertRandomData(IRequestHandler service) {

            if (Debugger.IsAttached) {

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
                    };

                    equipment.Add(eqp);
                }

                for (int i = 0; i < equipment.Count; i++) {

                    await service.Create<Equipment>(equipment[i]);
                }

                for (int i = 0; i < owners.Count; i++) {

                    await service.Create<Owner>(owners[i]);
                }

                return;
            }

            throw new Exception("This feature is only available in debug");
          
        }

        


        private string GetFileAsJson(IFormFile file) {

            using (var stream = new StreamReader(file.OpenReadStream())) {

                return stream.ReadToEnd();
            }

        }
        
    }
}
