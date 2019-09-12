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
using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;
using OwnerManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data.Validation;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data;
using Microsoft.EntityFrameworkCore;
using EquipmentManagementSystem.Domain.Service.Export;

namespace EquipmentManagementSystem.Controller {


    public class OwnerController : BaseController  {

        private IRequestHandler _service;
        private int pageSize = 25;


        public OwnerController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            var service = new GenericService(ctx);
            _service = new OwnerRequestHandler(service);
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
            ViewData["Page"] = page;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            
            SetSearchString(ref searchString);
            SetCultureCookie(culture, page.ToString(), searchString, sortVariable, Response);
            SetLanguage(culture);

            return PartialView(await ((OwnerRequestHandler)_service).IndexRequest<Owner>(
                new IndexRequestModel() { 
                    SortVariable = ViewData["CurrentSort"].ToString(),
                    Page = page,
                    SearchString = searchString,
                    PageSize = pageSize
                })
            );
        }


        // GET: Owner/Create
        public IActionResult Create() {

            return View(new OwnerViewModel());
        }


        /// <summary>
        /// Creates AJAX modal for Owner Create
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateModal() {

            return PartialView("_OwnerModalPartial", new Owner());
        }

        /// <summary>
        /// Post Owner Create Modal
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostModal(Owner owner) {

            try {

                owner.Added = DateTime.Now;
                if (await _service.Create(owner) is false) {

                    return Json(owner);
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(owner);
            }

        }


        // POST: Owner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OwnerViewModel viewModel) {

            try {
                viewModel.Owner.LastEdited = DateTime.Now;
                viewModel.Owner.Added = DateTime.Now;

                if (await _service.Create(viewModel.Owner) is false) {

                    return View(viewModel);
                }

                return RedirectToAction(nameof(Index));
            }
            catch {

                return View(viewModel);
            }
        }


        // GET: Owner/Edit/5
        public async Task<IActionResult> Edit(int id) {

            var viewModel = new OwnerViewModel {

                Owner = await _service.Get<Owner>(o => o.ID == id).FirstAsync()
            };
            viewModel.Equipment = await _service.Get<Equipment>(e => e.OwnerName == viewModel.Owner.FullName).ToListAsync();

            return View(viewModel);
        }


        // POST: Owner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OwnerViewModel viewModel) {

            viewModel.Owner.LastEdited = DateTime.Now;

            if (await _service.Update(viewModel.Owner) is false) {

                return View(viewModel);
            }

            foreach (Equipment equipment in await _service.Get<Equipment>(e => e.OwnerName == viewModel.Owner.FullName).ToListAsync()) {

                equipment.OwnerName = viewModel.Owner.FullName;

                await _service.Update(equipment);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Owner/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id) {

            var viewModel = new OwnerViewModel {

                Owner = await _service.Get<Owner>(o => o.ID == id).FirstAsync()
            };
            viewModel.Equipment = await _service.Get<Equipment>(e => e.OwnerName == viewModel.Owner.FullName).ToListAsync();

            return View(viewModel);
        }


        // POST: Owner/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(OwnerViewModel viewModel) {

            try {

                if (await _service.Remove(viewModel.Owner) is false) {

                    return View(viewModel);
                }

                foreach (Equipment equipment in await _service.Get<Equipment>(e => e.OwnerName == viewModel.Owner.FullName).ToListAsync()) {

                    equipment.OwnerName = null;
                    equipment.OwnerID = -1;

                    await _service.Update(equipment);
                }

                return RedirectToAction(nameof(Index));
            }
            catch {

                return View(viewModel);
            }
        }


        public async Task<IActionResult> Import(string source, IFormFile file, bool IsEquipment = false) {

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

                        await migration.InsertBackupJson<Owner>(file, IsEquipment);
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


        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Export(string exportType, string searchString, string selection = null) {

            Enum.TryParse(exportType, out ExportType exportTypes);
            return await _service.Export(searchString, selection, exportTypes);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelection(string mail) {

            try {
                var mails = mail.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < mails.Count(); i++) {

                    var owner = _service.FirstOrDefault<Owner>(o => mails[i] == o.Mail);
                    await _service.Remove(owner);
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(null);
            }
        }
        
    }
}