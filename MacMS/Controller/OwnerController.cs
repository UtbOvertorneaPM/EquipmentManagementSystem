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


    public class OwnerController : BaseController { 
            
        OwnerHandler repo;


        public OwnerController(ManagementContext ctx, IStringLocalizerFactory factory) : base(factory) {

            repo = new OwnerHandler(ctx);
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

            var data = Enumerable.Empty<Owner>();
            var pageSize = repo.PageSize;

            var pagedList = new PagedList<Owner>();

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

                data = repo.Sort(sortVariable, repo.GetAll());
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), repo.Count<Equipment>(), page, pageSize);

            }
            // Index request without modifiers
            else {

                data = repo.Sort("Date_desc", repo.GetAll());
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), repo.Count<Equipment>(), page, pageSize);
            }

            return PartialView(pagedList);
        }


        // GET: Owner/Create
        public IActionResult Create() {

            return View();
        }


        /// <summary>
        /// Creates AJAX modal for Owner Create
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateModal() {

            var owner = new Owner();
            return PartialView("_OwnerModalPartial", owner);
        }

        /// <summary>
        /// Post Owner Create Modal
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostModal(Owner owner) {

            try {

                owner.Added = DateTime.Now;
                if (ModelState.IsValid && repo.Insert<Owner>(owner)) {

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
        public IActionResult Create(Owner owner) {

            try {
                owner.LastEdited = DateTime.Now;
                owner.Added = DateTime.Now;

                if (ModelState.IsValid && repo.Insert<Owner>(owner)) {

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
        public IActionResult Edit(Owner owner) {

            try {
                
                if (ModelState.IsValid) {

                    repo.Update(owner);
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


        [HttpPost]
        public IActionResult Delete(List<string> names) {

            try {

                for (int i = 0; i < names.Count; i++) {

                    var id = repo.context.Set<Owner>().FirstOrDefault(e => names[i] == e.FullName).ID;
                    repo.Delete<Owner>(id);
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(null);
            }
        }


        /// <summary>
        /// AJAX get list of owners
        /// </summary>
        /// <returns></returns>
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
        [HttpGet]
        public IActionResult Export(string exportType, string searchString, string selection = null) {

            var file = new ExportHandler().Export(repo.context, typeof(Owner), searchString, exportType, selection);
            var stream = new MemoryStream(file.Data);
            stream.Position = 0;

            return File(stream, file.ContentType, file.FileName);
        }


        [HttpPost]
        public IActionResult DeleteSelection(string mail) {

            try {
                var mails = mail.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < mails.Count(); i++) {

                    var id = repo.context.Set<Owner>().FirstOrDefault(o => mails[i] == o.Mail).ID;
                    repo.Delete<Owner>(id);
                }

                return Json(true);
            }
            catch (Exception) {

                return Json(null);
            }
        }
    }
}