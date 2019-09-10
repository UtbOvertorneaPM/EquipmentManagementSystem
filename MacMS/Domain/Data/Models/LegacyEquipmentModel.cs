
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.Models {


    public class LegacyEquipmentModel {

        public object IDCheck { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public string Notes { get; set; }
        public int OwnerID { get; set; }
        public LegacyOwner Owner { get; set; }
        public string Location { get; set; }
        public int EquipType { get; set; }
        public object MobileNumber { get; set; }
        public object IP { get; set; }
        public object Ports { get; set; }
        public object Resolution { get; set; }
        public int ID { get; set; }
        public DateTime LastEdited { get; set; }
    }

    public class LegacyOwner {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public object SSN { get; set; }
        public object Address { get; set; }
        public object TelNr { get; set; }
        public object Mail { get; set; }
        public string FullName { get; set; }
        public DateTime Added { get; set; }
        public int ID { get; set; }
        public DateTime LastEdited { get; set; }
    }
}
