using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Models {



    public class EquipmentDDD {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime LastEdited { get; private set; }

        public bool? IDCheck { get; private set; }

        public string Model { get; private set; }

        [Required]
        public string Serial { get; private set; }

        public string Notes { get; private set; }

        public int? OwnerID { get; private set; }

        private Owner _owner;
        public IEnumerable<Owner> Owner => Owner;

        public string OwnerName { get { return _owner.FullName; } }

        public string Location { get; private set; }

        [Required]
        public EquipmentType EquipType { get; private set; }

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


        private EquipmentDDD() { }


        public EquipmentDDD(string serial, EquipmentType type) {

            if (string.IsNullOrEmpty(serial)) {
                throw new ArgumentNullException(nameof(serial));
            }

            Serial = serial;
            EquipType = type;
            LastEdited = DateTime.Now;
        }

        public void AddModel(string model) 
            => Model = model;
        
        public void AddNotes(string notes)
            => Notes = notes;

        public void AddLocation(string location)
            => Location = location;

        public void AddOwner(string firstName, string lastName, string mail, DbContext context) {

            if (_owner != null) {
            
                _owner = new Owner(firstName, lastName);
            }
            else if (context == null) {
                throw new ArgumentNullException(nameof(context),
                    "You must provide a context");
            }
            else if (context.Entry(this).IsKeySet) {

                throw new NotImplementedException();
                context.Add(new Owner(firstName, lastName, ID));
            }
        }


        public void RemoveOwner(Owner owner) {

            owner = null;            
        }
    }
}
