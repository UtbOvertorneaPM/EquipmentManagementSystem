using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using EquipmentManagementSystem.Data;
using System.Collections;

namespace EquipmentManagementSystem.Models {


    [JsonObject]
    public class Owner : Entity, IEqualityComparer, IEquatable<Owner> {

        [JsonProperty]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [JsonProperty]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [JsonProperty]
        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [JsonProperty]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [JsonProperty]
        [Display(Name = "TelNr")]
        public string TelNr { get; set; }

        [JsonProperty]
        [Display(Name = "Mail")]
        public string Mail { get; set; }

        private List<Equipment> _Equipment;

        [JsonIgnore]
        [Display(Name = "Equipment")]
        public List<Equipment> Equipment {
            get {

                return _Equipment ?? new List<Equipment>();
            }
            set {
                _Equipment = new List<Equipment>();
                _Equipment = value;
            }
        }

        [Display(Name = "FullName")]
        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }

        [Display(Name = "Added")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Added { get; set; }
 

        public new bool Equals(object x, object y) {

            if (x is Owner && y is Owner) {

                if (((Owner)x).ID == ((Owner)y).ID) {

                    return true;
                }
                else if (((Owner)x).FullName == ((Owner)y).FullName) {

                    return true;
                }
            }

            return false;
        }

        public bool Equals(Owner other) {

            if (other.ID == this.ID) {

                return true;
            }
            else if (other.FullName == this.FullName) {

                return true;
            }

            return false;
        }

        public int GetHashCode(object obj) {
            throw new NotImplementedException();
        }
    }
}
