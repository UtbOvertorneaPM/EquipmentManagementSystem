using System;
using System.Collections.Generic;
using System.Linq;

namespace EquipmentManagementSystem.Models {

    public class PagedList<T> : List<T> {

        public int Page { get; private set; }
        public int TotalPages { get; private set; }

        /// <summary>
        /// Handles paging of data
        /// </summary>
        public PagedList() {


        }

        public void Initialize(IEnumerable<T> list, int count, int page, int pageSize) {

            Page = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (list.Count() > 0) {

                this.AddRange(list);
            }
        }
        
    }
}
