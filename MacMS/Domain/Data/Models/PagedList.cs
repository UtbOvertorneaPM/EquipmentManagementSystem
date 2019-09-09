using EquipmentManagementSystem.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EquipmentManagementSystem.Domain.Data.Models {

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

        public void Initialize(IEnumerable<T> list, IndexRequestModel request, int count) {

            Page = request.Page;
            TotalEntries = count;
            TotalPages = (int)Math.Ceiling(count / (double)request.PageSize);

            PageEntries = Page * request.PageSize + 1;
            PageEnd = (PageEntries + list.Count() - 1);

            if (list.Count() > 0) {

                this.AddRange(list);
            }
        }
        
    }
}
