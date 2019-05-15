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
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Threading;

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
        }


        private void SetLanguage(string culture) => ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;


        // GET: Home
        public IActionResult Index(string sortVariable, string searchString, string culture, int page = 0) {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            ViewData["SearchString"] = string.IsNullOrEmpty(searchString) ? ViewData["SearchString"] : searchString;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            SetLanguage(culture);

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
        public IActionResult Create() {
            
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
        public IActionResult Edit(int id) {

            ViewData["IDCheck"] = false;
            return View(repo.Get(id));
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
        public IActionResult Delete(int id) {

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