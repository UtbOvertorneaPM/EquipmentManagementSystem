using Newtonsoft.Json;
using System.Collections.Generic;

namespace EquipmentManagementSystem {

    public class Rootobject {
        public Credentials Credentials { get; set; }
    }

    public class Credentials {
        public string User { get; set; }
        public string Password { get; set; }
    }

}



