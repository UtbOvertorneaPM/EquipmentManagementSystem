using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using EquipmentManagementSystem.Models;

namespace EquipmentManagementSystem.Data {


    public class SearchOwner {


        public IEnumerable<Owner> Search(string searchString, ManagementContext context) {

            var partialString = searchString.Split(" ");
            var owners = Enumerable.Empty<Owner>().AsQueryable();

            switch (searchString) {

                // Checks DoB pattern
                case var someVal when Regex.IsMatch(searchString, @"^\d{6,11}.?(\d{4})?"):

                    return context.Owners.Where(o => o.SSN.Contains(searchString));

                case var someVal when Regex.IsMatch(searchString, @"[\w.\w]*@{1}[\w.\w]*[\w.\w{2,3}]$"):

                    return context.Owners.Where(o => o.Mail == searchString);

                default:

                    return context.Owners.Where(o => o.FirstName.Contains(searchString) ||
                                                o.LastName.Contains(searchString) ||
                                                o.Address.Contains(searchString) || 
                                                o.Mail.Contains(searchString))
                                                .Distinct();
            }           
        }


    }
}
