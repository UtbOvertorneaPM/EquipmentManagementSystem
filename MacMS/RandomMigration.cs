    using System;
    using System.Collections.Generic;
    using System.Linq;


    using EquipmentManagementSystem.Models;
    using EquipmentManagementSystem.Data;


    namespace EquipmentManagementSystem {

        public static class RandomMigration {


            public async static void GetRandomTest(EquipmentHandler equipRepo, OwnerHandler ownerRepo) {

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

                    await equipRepo.Insert(equipment[i], false);
                    if (i % 20 == 0) { equipRepo.Save(); }
                }

                for (int i = 0; i < owners.Count; i++) {

                    await ownerRepo.Insert(owners[i], false);

                    if (i % 20 == 0) { ownerRepo.Save(); }
                }


            }
        }


    }