using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.newData;
using EquipmentManagementSystem.newData.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Controller {

    public class ChromebookController  : BaseController {

        private ChromebookService<Equipment> _service;
        private int pageSize = 25;


        public ChromebookController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            ctx.Database.EnsureCreated();
            _service = new ChromebookService<Equipment>(ctx, new EquipmentValidator());
        }


        /// <summary>
        /// JQuery Table update route
        /// </summary>
        /// <param name="sortVariable"></param>
        /// <param name="searchString"></param>
        /// <param name="culture"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> Table(string sortVariable, string searchString, string culture, int page = 0) {


            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            SetSearchString(ref searchString);
            SetCultureCookie(culture, Response);
            SetLanguage(culture);

            return PartialView(await _service.HandleRequest(sortVariable, searchString, page, pageSize));
        }


        public IActionResult Import(string source, IFormFile file, bool IsEquipment = true) {

            throw new NotImplementedException();

            var migration = new DataMigrations();
            try {
                switch (source) {

                    case "MacService":
                        if (file is null || file.Length == 0 || !string.Equals(file.ContentType, "application/json", StringComparison.OrdinalIgnoreCase)) {
                            throw new Exception("No appropriate file selected!");
                        }

                        // Used to import Macservice data
                        //migration.InsertMacServiceJson(_service, new OwnerHandler(_service._context), file);
                        break;

                    case "Backup":

                        if (file is null || file.Length == 0 || !string.Equals(file.ContentType, "application/json", StringComparison.OrdinalIgnoreCase)) {
                            throw new Exception("No appropriate file selected!");
                        }

                        //migration.InsertBackupJson(file, IsEquipment, _service, new OwnerHandler(_service._context));
                        break;

                    case "Random":

                        //migration.InsertRandomData(_service, new OwnerHandler(_service._context));
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
        public async Task<IActionResult> Export(string exportType, string searchString, string selection = null) {

            return await _service.Export(searchString, exportType, selection);

        }


        // GET: Home/Create
        public IActionResult Create() {

            return View(new Equipment());
        }


        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipment equipment) {


            // If Owner doesn't exists
            if ((bool)equipment.IDCheck) {

                return Json(false);
            }


            if (equipment.Owner.ID != -1) {
                equipment.OwnerID = equipment.Owner.ID;
                equipment.Owner = null;
            }


            //_service.context.Entry(equipment.Owner).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

            if (ModelState.IsValid) {

                equipment.LastEdited = DateTime.Now;
                await _service.Create(equipment);

                return Json(true);
            }

            return Json(false);
        }


        // GET: Home/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {

            ViewData["IDCheck"] = false;
            return View(await _service.GetById(id));
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
                await _service.Update(equipment);

                return Json(true);
            }
            catch (Exception) {

                throw;
            }
        }


        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int id) {

            return View(await _service.GetById(id));
        }


        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Equipment equipment, IFormCollection collection) {

            try {

                await _service.Remove(equipment);
                return Json(true);
            }
            catch {

                return Json(null);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelection(string serial) {

            try {
                var serials = serial.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < serials.Count(); i++) {

                    await _service.Remove(await _service.FirstOrDefault(e => e.Serial == serials[i]));
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(null);
            }
        }

    }
}
