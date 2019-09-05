using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using EquipmentManagementSystem.Data;
using Newtonsoft.Json;

namespace EquipmentManagementSystem.Models {


    public class Equipment : IEqualityComparer {


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime LastEdited { get; set; }


        [NotMapped]
        public bool? IDCheck { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Serial")]
        public string Serial { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public int? OwnerID { get; set; }

        [Display(Name = "Owner")]        
        public Owner Owner { get; set; }

        [NotMapped]
        public string OwnerName { get { return Owner.FullName; } }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "EquipType")]
        public EquipmentType EquipType { get; set; }


        public Equipment() { }


        public new bool Equals(object x, object y) {

            if (x is Equipment && y is Equipment) {

                if (((Equipment)x).ID == ((Equipment)y).ID) {

                    return true;
                }
                else if (((Equipment)x).Serial == ((Equipment)y).Serial) {

                    return true;
                }
            }

            return false;
        }


        public int GetHashCode(object obj) {
            throw new NotImplementedException();
        }

        public string MobileNumber { get; set; }

        public string IP { get; set; }

        public int? Ports { get; set; }

        public string Resolution { get; set; }


        public enum EquipmentType {

            Laptop,
            Chromebook,
            Mac,
            Desktop,
            Projector,
            Tablet,
            Mobile,
            Printer,
            Router,
            Switch,
            Misc
        }


    }
}
