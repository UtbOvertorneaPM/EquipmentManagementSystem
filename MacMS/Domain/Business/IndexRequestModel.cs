using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Business {


    public class IndexRequestModel {

        public string SortVariable { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Type { get; set; }
    }
}
