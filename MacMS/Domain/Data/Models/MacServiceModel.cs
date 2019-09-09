using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.Models {

    public class MacServiceModel {

        public class Rootobject {
            public Owner Owner { get; set; }
            public string Serial { get; set; }
            public string Model { get; set; }
            public string Location { get; set; }
            public string Contract { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string Notes { get; set; }
            public string OrderNr { get; set; }
            public string InvoiceNr { get; set; }
            public string Retailer { get; set; }
            public DateTime Added { get; set; }
            public object Logs { get; set; }
            public string CustomerRef { get; set; }
            public string AddedBy { get; set; }
        }

        public class Owner {
            public string Serial { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string SSN { get; set; }
            public string Address { get; set; }
            public string TelNr { get; set; }
            public string Mail { get; set; }
        }

    }
}
