using System;
using System.Collections.Generic;
using System.Linq;

namespace EquipmentManagementSystem.Models {

    public class PagedList<T> : List<T> {

        public int Page { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalEntries { get; private set; }
        public int PageEntries { get; private set; }
        public int PageEnd { get; private set; }

        /// <summary>
        /// Handles paging of data
        /// </summary>
        public PagedList() {


        }

        public void Initialize(IEnumerable<T> list, int count, int page, int pageSize) {

            Page = page;
            TotalEntries = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            PageEntries = page * pageSize + 1;
            PageEnd = (PageEntries + list.Count() - 1);

            if (list.Count() > 0) {

                this.AddRange(list);
            }
        }
        
    }
}
