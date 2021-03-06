﻿using Newtonsoft.Json;
using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections;

namespace EquipmentManagementSystem.Domain.Data.Models {


    public class Owner : IEqualityComparer, IEquatable<Owner> {


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime LastEdited { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "TelNr")]
        public string TelNr { get; set; }

        //[Required]
        [Display(Name = "Mail")]
        public string Mail { get; set; }




        [Display(Name = "FullName")]
        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }

        [Display(Name = "Added")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Added { get; set; }



        public Owner() { }

        public Owner(string firstName, string lastName) {

            FirstName = firstName;
            LastName = lastName;
            LastEdited = DateTime.Now;
        }

        public Owner(string firstName, string lastName, int eqpId) : this(firstName, lastName) {

            //Equipment.Add(id);
            //throw new NotImplementedException();

            //AddEquipment(id);
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
