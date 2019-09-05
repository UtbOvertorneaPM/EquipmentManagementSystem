using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Business {

    public class DataFormatting {

        public async Task<IEnumerable<Equipment>> Sort<TKey>(IQueryable<Equipment> query, string sortVariable) {

            switch (sortVariable) {
                case "Date_desc":
                    return await query.OrderByDescending(e => e.LastEdited).ToListAsync();
                case "Date":
                    return await query.OrderBy(e => e.LastEdited).ToListAsync();

                case "Owner_desc":
                    return await query.OrderByDescending(e => e.Owner.LastName).ToListAsync();
                case "Owner":
                    return await query.OrderBy(e => e.Owner.LastName).ToListAsync();
                default:
                    return null;
            }
        }


        public IQueryable<T> Search(IQueryable<T> data, string searchString) {

            var queryValues = searchString.Split(",");
            var returnData = Enumerable.Empty<T>().AsQueryable();

            for (int i = 0; i < queryValues.Length; i++) {

                var query = queryValues[i].Split(":");

                if (query.Length > 1) {

                    switch (query[0]) {

                        case "LastEdited":

                            var dates = query[1]
                                .Replace("/", " ")
                                .Split(' ');

                            for (int j = 0; j < dates.Count(); j++) {

                                returnData.Concat(from x in data
                                                  where x.LastEdited.ToString().Contains(dates[j])
                                                  && x.EquipType == Equipment.EquipmentType.Chromebook
                                                  select x);
                            }

                            break;

                        case "Model":

                            returnData.Concat(from x in data
                                              where x.Model.Contains(query[0])
                                              && x.EquipType == Equipment.EquipmentType.Chromebook
                                              select x);
                            break;
                        case "Serial":

                            returnData.Concat(from x in data
                                              where x.Serial.Contains(query[0])
                                              && x.EquipType == Equipment.EquipmentType.Chromebook
                                              select x);
                            break;

                        case "Owner":

                            returnData.Concat(from x in data
                                              where x.Owner.FullName.Contains(query[0])
                                              && x.EquipType == Equipment.EquipmentType.Chromebook
                                              select x);
                            break;

                        case "EquipType":

                            Enum.TryParse<Equipment.EquipmentType>(query[1], out var eqpVal);
                            returnData.Concat(from x in data
                                              where x.EquipType == eqpVal
                                              select x);
                            break;

                        default:
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(query[0]) && !string.IsNullOrWhiteSpace(query[0])) {

                    if (query[0] == "Find model/date/owner...") {

                        return data;
                    }
                    else {
                        returnData.Concat(from x in data
                                          where x.Owner.FullName.Contains(query[0]) ||
                                          x.Model.Contains(query[0]) ||
                                          x.Serial.Contains(query[0]) ||
                                          x.Notes.Contains(query[0]) ||
                                          x.Location.Contains(query[0])
                                          select x);

                        returnData = returnData.Where(x => x.EquipType == Equipment.EquipmentType.Chromebook);
                    }
                }
            }

            return returnData;
        }
    }
}
