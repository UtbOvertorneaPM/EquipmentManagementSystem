using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.Data;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace EquipmentManagementSystem.Controller {


    public class HomeController : BaseController { 

        EquipmentHandler repo;


        public HomeController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            ctx.Database.EnsureCreated();
            repo = new EquipmentHandler(ctx);
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


        public IActionResult Import(string source, IFormFile file, bool IsEquipment = true) {

            if (file is null || file.Length == 0 || !string.Equals(file.ContentType, "application/json", StringComparison.OrdinalIgnoreCase)) {
                throw new Exception("No appropriate file selected!");
            }

            var migration = new DataMigrations();
            try {
                switch (source) {

                    case "MacService":

                        // Used to import Macservice data
                        migration.InsertMacServiceJson(repo, new OwnerHandler(repo.context), file);
                        break;

                    case "Backup":

                        migration.InsertBackupJson(file, IsEquipment, repo, new OwnerHandler(repo.context));
                        break;

                    default:

                        return Json(false);
                }
            }
            catch (Exception) {

                throw;
            }

            
            return Json(true);
        }


        /// <summary>
        /// Exports current table data
        /// </summary>
        /// <param name="exportType">Excel or JSON</param>
        /// <param name="searchString">String used to narrow data</param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        public IActionResult Export(string exportType, string searchString, string selection = null) {

            var handler = new ExportHandler();
            var file = handler.Export(repo.context, typeof(Equipment), searchString, exportType, selection);
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


            // If Owner doesn't exists
            if ((bool)equipment.IDCheck) {

                return Json(false);
            }


            if (equipment.Owner.ID != -1) {
                equipment.OwnerID = equipment.Owner.ID;
                equipment.Owner = null;
            }


            //repo.context.Entry(equipment.Owner).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

            if (ModelState.IsValid) {

                equipment.LastEdited = DateTime.Now;
                repo.Insert(equipment);

                return Json(true);
            }

            return Json(false);
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
                if ((bool)equipment.IDCheck) {

                    return Json(false);
                }

                // If Owner was chosen in dropdown
                if (equipment.Owner.ID != -1) {
                    equipment.OwnerID = equipment.Owner.ID;
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


        [HttpPost]
        public IActionResult DeleteSelection(string serial) {

            try {
                var serials = serial.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < serials.Count(); i++) {

                    var id = repo.context.Set<Equipment>().FirstOrDefault(e => serials[i] == e.Serial).ID;
                    repo.Delete<Equipment>(id);
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(null);
            }
        }


    }
}