using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.DbAccess {

    public static class EquipmentDataFormatting {

        public static IEnumerable<Equipment> Sort<TKey>(IEnumerable<Equipment> query, string sortVariable) {

            switch (sortVariable) {
                case "Date_desc":
                    return query.OrderByDescending(e => e.LastEdited);

                case "Date":
                    return query.OrderBy(e => e.LastEdited);

                case "Owner_desc":
                    return query.OrderByDescending(e => e.OwnerName);

                case "Owner":
                    return query.OrderBy(e => e.OwnerName);

                default:
                    return null;
            }
        }


        public async static Task<IEnumerable<Equipment>> Search(IQueryable<Equipment> data, string searchString) {

            var queryValues = searchString.Split(",");
            var returnData = new List<Equipment>();

            for (int i = 0; i < queryValues.Length; i++) {

                var query = queryValues[i].Split(":");

                if (query.Count() > 1) {

                    switch (query[0]) {

                        case "LastEdited":

                            var dates = query[1]
                                .Replace("/", " ")
                                .Split(' ');

                            for (int j = 0; j < dates.Count(); j++) {

                                returnData.AddRange(await (from x in data
                                                  where x.LastEdited.ToString().Contains(dates[j])
                                                  select x).ToListAsync());
                            }

                            break;

                        case "Model":

                            returnData.AddRange(from x in data
                                              where x.Model.Contains(query[1])
                                              
                                              select x);
                            break;
                        case "Serial":

                            returnData.AddRange(await (from x in data
                                              where x.Serial.Contains(query[1])
                                              select x).ToListAsync());
                            break;

                        case "Owner":

                            returnData.AddRange(await(from x in data
                                              where x.OwnerName.Contains(query[1])
                                              select x).ToListAsync());
                            break;

                        case "EquipType":

                            Enum.TryParse<Equipment.EquipmentType>(query[1], out var eqpVal);
                            returnData.AddRange(await(from x in data
                                              where x.EquipType == eqpVal
                                              select x).ToListAsync());
                            break;

                        default:
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(query[0]) && !string.IsNullOrWhiteSpace(query[0])) {

                    if (query[0] == "Find model/date/owner...") {

                        return await data.ToListAsync();
                    }
                    else {
                        returnData.AddRange(from x in data
                                          where x.OwnerName.Contains(query[0]) ||
                                          x.Model.Contains(query[0]) ||
                                          x.Serial.Contains(query[0]) ||
                                          x.Notes.Contains(query[0]) ||
                                          x.Location.Contains(query[0])
                                          select x);
                    }
                }
            }

            return await Task.Run(() => returnData.Distinct().ToList());
        }
    }
}
