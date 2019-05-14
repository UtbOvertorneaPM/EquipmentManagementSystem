using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.Data;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using System.Net.Http;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace EquipmentManagementSystem.Controller {


    public class HomeController : Microsoft.AspNetCore.Mvc.Controller, IDisposable {

        // TODO: Add [Authorized] attributes, switch project to windows authentication
        // TODO: Remove comment tags in Properties\launchSettings.json and web.config
        EquipmentHandler repo;
        private readonly Localizer Localizer;

        public HomeController(ManagementContext ctx, IStringLocalizerFactory factory) {

            ctx.Database.EnsureCreated();
            repo = new EquipmentHandler(ctx);
            Localizer = new Localizer(factory);
        }


        // GET: Home
        public IActionResult Index(string sortVariable, string searchString, string culture, int page = 0) {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            ViewData["SearchString"] = string.IsNullOrEmpty(searchString) ? ViewData["SearchString"] : searchString;
            ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;
            ViewData["Page"] = page;
             /*
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(ViewData["Language"].ToString())),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            */
            var data = Enumerable.Empty<Equipment>();
            var pageSize = repo.PageSize;

            if (string.IsNullOrEmpty(sortVariable) && string.IsNullOrEmpty(searchString)) {

                data = repo.Sort(repo.GetAll((int)page), "Date_desc", page);

                return View(new PagedList<Equipment>(data, repo.Count<Equipment>(), page, pageSize));
            }
            else if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(sortVariable)) {

                data = repo.Sort(repo.Search(searchString), sortVariable, page);
                return View(new PagedList<Equipment>(data, data.Count(), page, pageSize));

            }
            else if (!string.IsNullOrEmpty(sortVariable)) { 

                data = repo.Sort(repo.GetAll(), sortVariable, page);
                return View(new PagedList<Equipment>(data, repo.Count<Equipment>(), page, pageSize));
            }

            data = repo.Search(searchString).Skip(page * pageSize).Take(pageSize);
            return View(new PagedList<Equipment>(data, data.Count(), page, pageSize));
        }


        public IActionResult Import() {

            //OneTimeMigration.GetJson(repo, new OwnerHandler(repo.context));
            RandomMigration.GetRandomTest(repo, new OwnerHandler(repo.context));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Export(string exportType, string searchString) {

            var file = new ExportHandler().Export(repo, searchString, exportType);
            var stream = new MemoryStream(file.Data);
            stream.Position = 0;

            return File(stream, file.ContentType, file.FileName);
        }

        [HttpGet]
        public FileStreamResult ExportFile(ExportFile file) {

            var stream = new MemoryStream(file.Data);
            stream.Position = 0;

            return File(stream, file.ContentType, file.FileName);
        }


        // GET: Home/Create
        public IActionResult Create(string culture) {

            ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;
            return View(new Equipment());
        }


        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipment formModel) {

            try {
                // Owner does not exist
                if (formModel.IDCheck) {

                    return Json(false);
                }

                if (ModelState.IsValid) {

                    await repo.Insert(formModel);
                    formModel.LastEdited = DateTime.Now;

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
        public IActionResult Edit(int id, string culture) {

            ViewData["IDCheck"] = false;
            ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;
            var test = repo.Get(id);
            return View(test);
        }   


        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Equipment equipment) {

            try {

                if (equipment.IDCheck) {

                    return Json(false);
                }

                if (equipment.Owner.ID != -1) {

                    equipment.Owner = repo.GetOwner(equipment.Owner.ID);
                }

                equipment.LastEdited = DateTime.Now;
                await repo.Update<Equipment>(equipment);

                return Json(true);
            }
            catch(Exception e) {

                throw e;
            }
        }


        // GET: Home/Delete/5
        public IActionResult Delete(int id, string culture) {

            ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;

            return View(repo.Get(id));
        }


        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection) {

            try {

                await repo.Delete<Equipment>(id);

                return Json(true);
            }
            catch {

                return Json(null);
            }
        }


    }
}