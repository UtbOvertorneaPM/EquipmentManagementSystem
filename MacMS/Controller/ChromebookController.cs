using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Data.Export;
using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Export;
using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.newData;
using EquipmentManagementSystem.newData.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Controller {



    public class ChromebookController : BaseController { //Microsoft.AspNetCore.Mvc.Controller { //, IController<Equipment> {

        private EquipmentRequestHandler _service;
        private int pageSize = 25;

        /*
        public ChromebookController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            ctx.Database.EnsureCreated();
            _service = new EquipmentRequestHandler<T>(new GenericService(ctx));
        }
        */

        public ChromebookController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            ctx.Database.EnsureCreated();
            _service = new EquipmentRequestHandler(new GenericService(ctx), new EquipmentValidator());

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

            return PartialView(await _service.IndexRequest<Equipment>(sortVariable, searchString, page, pageSize));
        }


        public async Task<IActionResult> Import(string source, IFormFile file, bool IsEquipment = true) {

            //throw new NotImplementedException();
            
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
                        var data = migration.InsertRandomData();

                        for (int i = 0; i < data.Count; i++) {

                            await _service.Create<Equipment>(data[i]);
                        }
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

            Enum.TryParse(exportType, out ExportType exportTypes);
            return await _service.Export(searchString, selection, exportTypes);
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

            return View(await _service
                .FirstOrDefault<Equipment>(e => e.ID == id)
                .AddIncludes(DataIncludes.Owner)
                .FirstOrDefaultAsync()
                );
        }


        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Edit(Equipment equipment) {
        public async Task<IActionResult> Edit(Equipment equipment) {

            try {

                equipment.LastEdited = DateTime.Now;
                //equipment.Owner = await _service.GetById<Owner>(equipment.Owner.ID);

                if (await _service.Update(equipment) is false) {

                    return View(equipment);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e) {

                return View(equipment);
            }
        }


        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int id) {

            return View(await _service.GetById<Equipment>(id));
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

                    await _service.Remove(await _service.FirstOrDefault<Equipment>(e => e.Serial == serials[i]).FirstOrDefaultAsync());
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(null);
            }
        }



        public async Task<ActionResult> AutoComplete(string term) {

            if (!string.IsNullOrEmpty(term)) {

                var request = _service.GetAll<Owner>();
                return Json(await request.Select(e => e.FullName).ToListAsync());
            }

            return null;
        }
        

    }
}
