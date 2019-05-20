using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using System.IO;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;


namespace EquipmentManagementSystem.Controller {


    //[Authorize("Administrators")]
    //public class OwnerController : Microsoft.AspNetCore.Mvc.Controller {
    public class OwnerController : BaseController { 
            
        OwnerHandler repo;
        //private readonly Localizer Localizer;


        public OwnerController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            repo = new OwnerHandler(ctx);
            //Localizer = new Localizer(factory);
        }

        /*
        public override void OnActionExecuting(ActionExecutingContext context) {

            base.OnActionExecuting(context);
            var cookie = context.HttpContext.Request.Cookies;

            if (!context.ActionArguments.ContainsKey("culture") && !(cookie[".AspNetCore.Culture"] is null)) {

                var culture = cookie[".AspNetCore.Culture"].Substring(2, 5);
                SetLanguage(culture);
            }
            else if (context.ActionArguments.ContainsKey("culture")) {

                SetLanguage(context.ActionArguments["culture"].ToString());
            }
            else {

                SetLanguage("en-GB");
            }
        }


        private void SetLanguage(string culture) => ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;


        // GET: Owner
        public IActionResult Index(string sortVariable, string searchString, string culture, int page = 0) {

            var count = 0;
            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;

            // Searchstring priority
            if (!(string.IsNullOrEmpty(searchString)) && ViewData.ContainsKey("SearchString")) {

                ViewData["SearchString"] = searchString;
            }
            else if (string.IsNullOrEmpty(searchString) && ViewData.ContainsKey("SearchString")) {

                searchString = ViewData["SearchString"].ToString();
            }
            else {

                ViewData["SearchString"] = searchString;
            }

            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            SetLanguage(culture);

            return HandleIndexRequest(sortVariable, searchString, culture, page);
        }
        */

        /// <summary>
        /// Handles request to Index action
        /// </summary>
        /// <param name="sortVariable"></param>
        /// <param name="searchString"></param>
        /// <param name="culture"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected override IActionResult HandleIndexRequest(string sortVariable, string searchString, string culture, int page) {

            var data = Enumerable.Empty<Owner>();
            var pageSize = repo.PageSize;

            var pagedList = new PagedList<Owner>();

            // Search then sort
            if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(sortVariable)) {

                data = repo.Sort(repo.Search(searchString), sortVariable);
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), data.Count(), page, pageSize);
            }
            // Search
            else if (!string.IsNullOrEmpty(searchString)) {

                data = repo.Search(searchString);
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), data.Count(), page, pageSize);
            }
            // Sort
            else if (!string.IsNullOrEmpty(sortVariable)) {

                data = repo.Sort(repo.GetAll(), sortVariable);
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), repo.Count<Equipment>(), page, pageSize);

            }
            // Index request without modifiers
            else {

                data = repo.Sort(repo.GetAll(), "Date_desc");
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), repo.Count<Equipment>(), page, pageSize);
            }

            return View(pagedList);
        }


        // GET: Owner/Create
        public IActionResult Create() {

            return View();
        }


        public IActionResult CreateModal() {

            var owner = new Owner();
            return PartialView("_OwnerModalPartial", owner);
        }


        [HttpPost]
        public async Task<IActionResult> PostModal(Owner owner) {

            try {

                owner.Added = DateTime.Now;
                if (ModelState.IsValid && await repo.Insert<Owner>(owner)) {

                    return Json(true);
                }

                return Json(false);
            }
            catch (Exception) {

                return null;
            }

        }


        // POST: Owner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Owner owner) {

            try {
                owner.LastEdited = DateTime.Now;
                owner.Added = DateTime.Now;

                if (ModelState.IsValid && await repo.Insert<Owner>(owner)) {

                    return Json(true);
                }

                return Json(false);
            }
            catch {

                return Json(null);
            }
        }


        // GET: Owner/Edit/5
        public IActionResult Edit(int id) {

            var owner = repo.Get(id);
            owner.Equipment = repo.GetEquipment(owner);

            return View(owner);
        }


        // POST: Owner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Owner owner) {

            try {
                
                if (ModelState.IsValid) {

                    await repo.Update(owner);
                    owner.LastEdited = DateTime.Now;

                    return Json(true);
                }

                return Json(false);
            }
            catch {

                return Json(null);
            }
        }


        // GET: Owner/Delete/5
        public IActionResult Delete(int id) {

            return View(repo.Get(id));
        }


        // POST: Owner/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection) {

            try {

                repo.Delete<Owner>(id);

                return Json(true);
            }
            catch {

                return Json(null);
            }
        }


        [HttpGet]
        public JsonResult GetOwnerList() {

            var owners = repo.GetAll().Distinct().ToList();
            var dict = new Dictionary<int, string>();

            for (int i = 0; i < owners.Count; i++) {

                dict.Add(owners[i].ID, owners[i].FullName);
            }

            return Json(dict);
        }


        [HttpGet]
        public JsonResult GetOwner(int id) {

            return Json(repo.Get(id, false));
        }


        [HttpPost]
        public IActionResult Export(string exportType, string searchString) {

            var file = new ExportHandler().Export(repo.context, typeof(Owner), searchString, exportType);
            var stream = new MemoryStream(file.Data);
            stream.Position = 0;

            return File(stream, file.ContentType, file.FileName);
        }


    }
}