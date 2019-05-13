﻿using System;
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

namespace EquipmentManagementSystem.Controller {


    public class OwnerController : Microsoft.AspNetCore.Mvc.Controller {

        OwnerHandler repo;
        private readonly Localizer Localizer;

        public OwnerController(ManagementContext ctx, IStringLocalizerFactory factory) {

            repo = new OwnerHandler(ctx);
            Localizer = new Localizer(factory);
        }

        // GET: Owner
        public IActionResult Index(string sortVariable, string searchString, string culture, int page = 0) {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            ViewData["SearchString"] = string.IsNullOrEmpty(searchString) ? ViewData["SearchString"] : searchString;
            ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;
            ViewData["Page"] = page;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(ViewData["Language"].ToString())),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            var data = Enumerable.Empty<Owner>();
            var pageSize = repo.PageSize;

            if (string.IsNullOrEmpty(sortVariable) && string.IsNullOrEmpty(searchString)) {

                data = repo.Sort(repo.GetAll(), "Date_desc", page);
                return View(new PagedList<Owner>(data, repo.Count<Owner>(), page, pageSize));
            }
            else if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(sortVariable)) {

                data = repo.Sort(repo.Search(searchString), sortVariable, page);
                return View(new PagedList<Owner>(data, data.Count(), page, pageSize));
            }
            else if (!string.IsNullOrEmpty(sortVariable)) {

                data = repo.Sort(repo.GetAll(), sortVariable, page);
                return View(new PagedList<Owner>(data, repo.Count<Owner>(), page, pageSize));
            }

            data = repo.Search(searchString);
            return View(new PagedList<Owner>(data, data.Count(), page, pageSize));
        }


        // GET: Owner/Create
        public IActionResult Create() {

            return View();
        }


        public IActionResult CreateModal() {

            var owner = new Owner();
            
            return PartialView("_OwnerModalPartial", owner);
        }

        [HttpPost]
        public async Task<IActionResult> PostModal(Owner owner) {

            try {

                if (ModelState.IsValid && await repo.Insert<Owner>(owner)) {

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
        public async Task<IActionResult> Create(Owner owner) {

            try {
                owner.LastEdited = DateTime.Now;

                if (ModelState.IsValid && await repo.Insert<Owner>(owner)) {

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

            return View(repo.Get(id));
        }


        // POST: Owner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Owner owner) {

            try {
                
                if (ModelState.IsValid) {

                    await repo.Update(owner);
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
    }
}