using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;

using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.Data;
using Microsoft.Extensions.Localization;


namespace EquipmentManagementSystem.Controller {


    //[Authorize("Administrators")]
    //public class HomeController : Microsoft.AspNetCore.Mvc.Controller, IDisposable {
    public class HomeController : BaseController { 

        EquipmentHandler repo;
        //private readonly Localizer Localizer;


        public HomeController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            ctx.Database.EnsureCreated();
            repo = new EquipmentHandler(ctx);
            //Localizer = new Localizer(factory);
        }

        /*
        /// <summary>
        /// Handles language persistence between controllers/actions
        /// </summary>
        /// <param name="context"></param>
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


        /// <summary>
        /// Sets the language in ViewData
        /// </summary>
        /// <param name="culture"></param>
        private void SetLanguage(string culture) => ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;


        // GET: Home    
        public IActionResult Index(string sortVariable, string searchString, string culture, int page = 0) {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            SetSearchString(ref searchString);

            SetCultureCookie(culture, Response);

            SetLanguage(culture);

            return HandleIndexRequest(sortVariable, searchString, culture, page);
        }


        private void SetSearchString(ref string searchString) {

            // Searchstring priority, if both searchString and ViewData is present, use searchString
            if (!(string.IsNullOrEmpty(searchString)) && ViewData.ContainsKey("SearchString")) {

                ViewData["SearchString"] = searchString;
            }
            // If ViewData search string is present and searchString is null/empty
            else if (string.IsNullOrEmpty(searchString) && ViewData.ContainsKey("SearchString")) {

                searchString = ViewData["SearchString"].ToString();
            }
            else {

                ViewData["SearchString"] = searchString;
            }
        }


        /// <summary>
        /// Sets culture cookie
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="Response"></param>
        private void SetCultureCookie(string culture, HttpResponse Response) {

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
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
        public async Task<IActionResult> Create(Equipment equipment) {

            try {
                // Owner does not exist
                if (equipment.IDCheck) {

                    return Json(false);
                }

                // If Owner was chosen in dropdown
                if (equipment.Owner.ID != -1) {

                    equipment.Owner = repo.GetOwner(equipment.Owner.ID);
                }

                if (ModelState.IsValid) {

                    equipment.LastEdited = DateTime.Now;
                    await repo.Insert(equipment);
                    
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

                // If Owner exists
                if (equipment.IDCheck) {

                    return Json(false);
                }

                // If Owner was chosen in dropdown
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