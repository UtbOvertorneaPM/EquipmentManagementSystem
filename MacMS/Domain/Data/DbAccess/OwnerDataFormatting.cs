using EquipmentManagementSystem.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.DbAccess {


    public class OwnerDataFormatting {


        public static IEnumerable<Owner> Sort<TKey>(IEnumerable<Owner> query, string sortVariable) {

            switch (sortVariable) {
                case "Date_desc":
                    return query.OrderByDescending(o => o.LastEdited);

                case "Date":
                    return query.OrderBy(o => o.LastEdited);

                case "Owner_desc":
                    return query.OrderByDescending(o => o.FullName);

                case "Owner":
                    return query.OrderBy(o => o.FullName);
                case "Created_desc":
                    return query.OrderByDescending(o => o.Added);
                case "Created":
                    return query.OrderBy(o => o.Added);
                default:
                    return null;
            }
        }


        public async static Task<IEnumerable<Owner>> Search(IQueryable<Owner> data, string searchString) {

            var queryValues = searchString.Split(",");
            var returnData = new List<Owner>();

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

                        case "FullName":

                            returnData.AddRange(from x in data
                                                where x.FullName.Contains(query[1])

                                                select x);
                            break;
                        case "SSN":

                            returnData.AddRange(await (from x in data
                                                       where x.SSN.Contains(query[1])
                                                       select x).ToListAsync());
                            break;

                        case "Mail":

                            returnData.AddRange(await (from x in data
                                                       where x.Mail.Contains(query[1])
                                                       select x).ToListAsync());
                            break;

                        case "Address":

                            returnData.AddRange(await (from x in data
                                                       where x.Address.Contains(query[1])
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
                                          where x.LastEdited.ToString().Contains(query[0]) ||
                                          x.FullName.Contains(query[0]) ||
                                          x.SSN.Contains(query[0]) ||
                                          x.Mail.Contains(query[0]) ||
                                          x.Address.Contains(query[0])
                                          select x);
                    }
                }
            }

            return await Task.Run(() => returnData.Distinct().ToList());
        }


    }
}
