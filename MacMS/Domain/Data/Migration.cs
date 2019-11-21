using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Http;
using EquipmentManagementSystem.Domain.Business;

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

            try {
                for (int i = 0; i < root.Count; i++) {

                    var macOwner = root[i].Owner;

                    var owner = new Owner {
                        FirstName = macOwner.FirstName,
                        LastName = macOwner.LastName,
                        SSN = macOwner.SSN,
                        Mail = macOwner.Mail,
                        Address = macOwner.Address,
                        TelNr = macOwner.TelNr,
                        LastEdited = dateToday
                    };

                    if (string.IsNullOrEmpty(owner.FirstName) is false) {

                        owners.Add(owner);
                    }

                    var mac = root[i];

                    var equipment = new Equipment {
                        Serial = mac.Serial,
                        Model = mac.Model,
                        LastEdited = mac.Added,
                        OwnerName = owner.FirstName + " " + owner.LastName,
                        Notes = mac.Notes
                    };

                    equipment.LastEdited = dateToday;

                    equip.Add(equipment);
                }

                for (int i = 0; i < equip.Count; i++) {

                    await service.Create<Equipment>(equip[i]);
                }

                for (int i = 0; i < owners.Count; i++) {

                    await service.Create<Owner>(owners[i]);
                }

            }
            catch (Exception) {

                throw;
            }
            

            return;
        }

        public async Task LegacyImportJson(IRequestHandler _service, IFormFile file) {

            var json = GetFileAsJson(file);
            var data = await Task.Run(() => JsonConvert.DeserializeObject<List<LegacyEquipmentModel>>(json));

            if (data is List<LegacyEquipmentModel> test) {

                for (int i = 0; i < test.Count; i++) {

                    try {
                        if ((test[i].Owner is null) is false) {
                            var legacyOwner = test[i].Owner;

                            var owner = new Owner {
                                FirstName = legacyOwner.FirstName,
                                LastName = legacyOwner.LastName,
                                LastEdited = legacyOwner.LastEdited,
                                Added = legacyOwner.Added,
                            };

                            if ((legacyOwner.Mail is null) is false) {
                                owner.Mail = string.IsNullOrEmpty(legacyOwner.Mail.ToString()) ? legacyOwner.Mail.ToString() : null;
                            }
                            if ((legacyOwner.Address is null) is false) {
                                owner.Address = string.IsNullOrEmpty(legacyOwner.Address.ToString()) ? legacyOwner.Address.ToString() : null;
                            }
                            if ((legacyOwner.TelNr is null) is false) {
                                owner.TelNr = string.IsNullOrEmpty(legacyOwner.TelNr.ToString()) ? legacyOwner.TelNr.ToString() : null;
                            }
                            if ((legacyOwner.SSN is null) is false) {
                                owner.SSN = string.IsNullOrEmpty(legacyOwner.SSN.ToString()) ? legacyOwner.SSN.ToString() : null;
                            }

                            var eqp = new Equipment {
                                Model = test[i].Model,
                                Serial = test[i].Serial,
                                Notes = test[i].Notes,
                                Location = test[i].Location,
                                LastEdited = test[i].LastEdited,
                                EquipType = (Equipment.EquipmentType)test[i].EquipType,
                                OwnerName = owner.FullName
                            };


                            await _service.Create<Equipment>(eqp);
                            await _service.Create<Owner>(owner);
                        }
                        else {

                            var eqp = new Equipment {
                                Model = test[i].Model,
                                Serial = test[i].Model,
                                Notes = test[i].Notes,
                                Location = test[i].Location,
                                LastEdited = test[i].LastEdited,
                                EquipType = (Equipment.EquipmentType)test[i].EquipType
                            };

                            await _service.Create<Equipment>(eqp);

                        }

                    }
                    catch (NullReferenceException) {

                        throw;
                    }

                }
            }

            return;
        }


        public async Task<IEnumerable<T>> InsertBackupJson<T>(IFormFile file, bool IsEquipment) where T : class {

            var json = GetFileAsJson(file);
            var data = new List<T>();

            if (IsEquipment) {

                data = await Task.Run(() => JsonConvert.DeserializeObject<List<T>>(json));
                
                if (data is List<Equipment> test) {

                    for (int i = 0; i < data.Count(); i++) {
                        test[i].ID = -1;
                    }
                }
                

            }
            else {

                data = await Task.Run(() => JsonConvert.DeserializeObject<List<T>>(json));

                if (data is List<Owner> test) {

                    for (int i = 0; i < data.Count(); i++) {
                        test[i].ID = -1;
                    }
                }
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
