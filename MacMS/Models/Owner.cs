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
        public string FirstName { get; set; }

        [JsonProperty]
        public string LastName { get; set; }

        [JsonProperty]
        public string SSN { get; set; }

        [JsonProperty]
        public string Address { get; set; }

        [JsonProperty]
        public string TelNr { get; set; }

        [JsonProperty]
        public string Mail { get; set; }

        [JsonIgnore]
        public ICollection<Equipment> Equipment { get; set; }


        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }
 

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
