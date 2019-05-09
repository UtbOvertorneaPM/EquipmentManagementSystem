using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace EquipmentManagementSystem.Models {


    [JsonObject]
    public class Mac : Equipment {


        [JsonProperty]
        public string Contract { get; set; }

        [JsonProperty]
        public DateTime From { get; set; }

        [JsonProperty]
        public DateTime To { get; set; }

        [JsonProperty]
        public string Notes { get { return base.Notes; } set { base.Notes = value; } }

        [JsonProperty]
        public string OrderNr { get; set; }

        [JsonProperty]
        public string InvoiceNr { get; set; }

        [JsonProperty]
        public string Retailer { get; set; }

        [JsonProperty]
        public DateTime Added { get { return base.LastEdited; } set { base.LastEdited = value; } }

        [JsonProperty]
        public string CustomerRef { get; set; }

        [JsonProperty]
        public string AddedBy { get; set; }
    }


}

