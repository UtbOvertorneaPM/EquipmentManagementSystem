using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.Data;
using Microsoft.Extensions.Localization;

namespace EquipmentManagementSystem.Controller {


    public class HomeController : BaseController { 

        EquipmentHandler repo;


        public HomeController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            ctx.Database.EnsureCreated();
            repo = new EquipmentHandler(ctx);
        }


        /// <summary>
        /// Handles request to Index action
        /// </summary>
        /// <param name="sortVariable"></param>
        /// <param name="searchString"></param>
        /// <param name="culture"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected override IActionResult HandleIndexRequest(string sortVariable, string searchString, string culture, int page) {

            var data = Enumerable.Empty<Equipment>();
            var pageSize = repo.PageSize;

            var pagedList = new PagedList<Equipment>();

            // Search then sort
            if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(sortVariable)) {
                //var test = repo.Search(searchString);
                data = repo.SearchSort(searchString, sortVariable);
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

        // GET: Home    
        /// <summary>
        /// Used for JQuery Updating table index page
        /// </summary>
        /// <param name="sortVariable"></param>
        /// <param name="searchString"></param>
        /// <param name="culture"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IActionResult PartialIndex(string sortVariable, string searchString, string culture, int page = 0) {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            SetSearchString(ref searchString);

            SetCultureCookie(culture, Response);

            SetLanguage(culture);

            return View();
        }


        /// <summary>
        /// JQuery Table update route
        /// </summary>
        /// <param name="sortVariable"></param>
        /// <param name="searchString"></param>
        /// <param name="culture"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PartialViewResult Table(string sortVariable, string searchString, string culture, int page = 0) {

            //return PartialView(HandleIndexRequest(sortVariable, searchString, culture, page));


            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            SetSearchString(ref searchString);

            SetCultureCookie(culture, Response);

            SetLanguage(culture);

            var data = Enumerable.Empty<Equipment>();
            var pageSize = repo.PageSize;

            var pagedList = new PagedList<Equipment>();

            // Search then sort
            if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(sortVariable)) {
                //var test = repo.Search(searchString);
                data = repo.SearchSort(searchString, sortVariable);
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

            return PartialView(pagedList);
        }


        public IActionResult Import() {

            // Used to import Macservice data
            //OneTimeMigration.GetJson(repo, new OwnerHandler(repo.context));

            // Generates 500 Equipment/Owners for testing purposes
            RandomMigration.GetRandomTest(repo, new OwnerHandler(repo.context));


            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exports current table data
        /// </summary>
        /// <param name="exportType">Excel or JSON</param>
        /// <param name="searchString">String used to narrow data</param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        public IActionResult Export(string exportType, string searchString) {

            var file = new ExportHandler().Export(repo.context, typeof(Equipment), searchString, exportType);
            var stream = new MemoryStream(file.Data);
            stream.Position = 0;

            return File(stream, file.ContentType, file.FileName);
        }


        // GET: Home/Create
        public IActionResult Create() {
            
            return View(new Equipment());
        }


        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Equipment equipment) {

            try {

                // If Owner doesn't exists
                if (equipment.IDCheck) {

                    return Json(false);
                }

                // If Owner was chosen in dropdown
                if (equipment.Owner.ID != -1) {

                    equipment.OwnerID = equipment.Owner.ID;
                    equipment.Owner = null;
                }
                else if (equipment.Owner.ID == -1) {

                    equipment.Owner = null;
                }

                if (ModelState.IsValid) {

                    equipment.LastEdited = DateTime.Now;
                    repo.Insert(equipment);
                    
                    return Json(true);
                }

                return Json(false);
            }
            catch {

                return Json(null);
            }
        }


        // GET: Home/Edit/5
        [HttpGet]
        public IActionResult Edit(int id) {

            ViewData["IDCheck"] = false;
            return View(repo.Get(id));
        }   


        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Edit(Equipment equipment) {
        public async Task<IActionResult> Edit(Equipment equipment) {

            try {

                // If Owner doesn't exists
                if (equipment.IDCheck) {

                    return Json(false);
                }

                // If Owner was chosen in dropdown
                if (equipment.Owner.ID != -1) {

                    equipment.OwnerID = equipment.Owner.ID;
                }                
                else if (equipment.Owner.ID == -1) {
                    
                    equipment.Owner = null;
                }
                
                equipment.LastEdited = DateTime.Now;
                repo.Update(equipment);

                return Json(true);
            }
            catch(Exception) {

                throw;
            }
        }


        // GET: Home/Delete/5
        public IActionResult Delete(int id) {

            return View(repo.Get(id));
        }


        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection) {

            try {

                repo.Delete<Equipment>(id);                
                return Json(true);
            }
            catch {

                return Json(null);
            }
        }


    }
}