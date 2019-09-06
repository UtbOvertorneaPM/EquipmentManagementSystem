
namespace EquipmentManagementSystem.Business.Data {

    public class Rootobject {
        public Credentials Credentials { get; set; }
    }

    public class Credentials {
        public string User { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string DbName { get; set; }
        public string Domain { get; set; }
        public string DebugDomain { get; set; }
    }

}



