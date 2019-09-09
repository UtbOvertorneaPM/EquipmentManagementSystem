using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Data.Export;
using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data;
using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Data.Validation;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Export;
using EquipmentManagementSystem.Models;

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



    public class ChromebookController : BaseController {

        private IRequestHandler _service;
        private int pageSize = 25;

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

            sortVariable = sortVariable == "Date_desc" ? "Date" : "Date_desc";
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            SetSearchString(ref searchString);
            SetCultureCookie(culture, Response);
            SetLanguage(culture);

            if (searchString == "Find model / date / owner...") {

                searchString = "EquipType:Chromebook";
            }
            else {
                searchString += "EquipType: Chromebook";
            }

            return PartialView(await ((EquipmentRequestHandler)_service).IndexRequest<EquipmentViewModel>(
                new IndexRequestModel() {
                    SortVariable = sortVariable,
                    Page = page,
                    SearchString = searchString,
                    PageSize = pageSize,
                    Type = "Chromebook"
                })
            );
        }


        public async Task<IActionResult> Import(string source, IFormFile file, bool IsEquipment = true) {

            var data = new List<Equipment>();

            var migration = new DataMigrations();
            try {
                switch (source) {

                    case "MacService":
                        if (file is null || file.Length == 0 || !string.Equals(file.ContentType, "application/json", StringComparison.OrdinalIgnoreCase)) {
                            throw new Exception("No appropriate file selected!");
                        }

                        // Used to import Macservice data
                        await migration.ImportMacServiceJson(_service, file);
                        break;

                    case "Backup":

                        //Restore from .json Export
                        if (file is null || file.Length == 0 || !string.Equals(file.ContentType, "application/json", StringComparison.OrdinalIgnoreCase)) {
                            throw new Exception("No appropriate file selected!");
                        }

                        data = (await migration.InsertBackupJson<Equipment>(file, IsEquipment)).ToList();
                        break;

                    case "Random":

                        //Random for testing
                        await migration.InsertRandomData(_service);

                        return Json(true);

                    default:

                        return Json(false);
                }

                for (int i = 0; i < data.Count; i++) {

                    await _service.Create(data[i]);
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

            return View(new EquipmentViewModel());
        }


        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentViewModel viewModel) {

            viewModel.Equipment.LastEdited = DateTime.Now;

            if (string.IsNullOrEmpty(viewModel.Equipment.OwnerName) is false) {

                await viewModel.AddOwner(_service, viewModel.Equipment.OwnerName);
            }            

            if (await _service.Create(viewModel.Equipment) is false) {

                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));            
        }


        // GET: Home/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {

            ViewData["IDCheck"] = false;
            var viewModel = new EquipmentViewModel {
                Equipment = await _service.FirstOrDefault<Equipment>(e => e.ID == id).FirstOrDefaultAsync()
            };

            if (string.IsNullOrEmpty(viewModel.Equipment.OwnerName) is false) {

                await viewModel.AddOwner(_service, viewModel.Equipment.OwnerName);
            }
            
            return View(viewModel);
        }


        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Edit(Equipment equipment) {
        public async Task<IActionResult> Edit(EquipmentViewModel viewModel) {

            viewModel.Equipment.LastEdited = DateTime.Now;

            if (await _service.Update(viewModel.Equipment) is false) {

                return View(viewModel.Equipment);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int id) {

            var viewModel = new EquipmentViewModel {
                Equipment = await _service.FirstOrDefault<Equipment>(e => e.ID == id).FirstOrDefaultAsync()
            };

            if (string.IsNullOrEmpty(viewModel.Equipment.OwnerName) is false) {

                await viewModel.AddOwner(_service, viewModel.Equipment.OwnerName);
            }

            return View(viewModel);
        }


        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EquipmentViewModel viewModel) {

            try {

                await _service.Remove(viewModel.Equipment);

                return RedirectToAction(nameof(Index));
            }
            catch {

                return View(viewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelection(string serial) {

            try {

                await _service.DeleteSelection(serial);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception) {

                return Json(null);
            }
        }



        public async Task<ActionResult> AutoComplete(string term) {

            if (!string.IsNullOrEmpty(term)) {

                var request = _service.GetAll<Owner>();
                var data = await request.Select(e => e.FullName).ToListAsync();
                return Json(data);
            }

            return null;
        }
        

    }
}
