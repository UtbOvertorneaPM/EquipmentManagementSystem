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

namespace EquipmentManagementSystem.Controller {


    public class HomeController : Microsoft.AspNetCore.Mvc.Controller, IDisposable {

        // TODO: Add [Authorized] attributes, switch project to windows authentication
        // TODO: Remove comment tags in Properties\launchSettings.json and web.config
        EquipmentHandler repo;

        public HomeController(ManagementContext ctx) {
            ctx.Database.EnsureCreated();
            repo = new EquipmentHandler(ctx);
        }


        // GET: Home
        public IActionResult Index(string sortVariable, string searchString, int page = 0) {

            ViewData["CurrentSort"] = sortVariable is null ? "Date_desc" : sortVariable;
            ViewData["SearchString"] = string.IsNullOrEmpty(searchString) ? ViewData["SearchString"] : searchString;
            ViewData["Page"] = page;
            
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

        public IActionResult Export(string searchString, string exportType) {

            var data = Enumerable.Empty<Equipment>();
            data = string.IsNullOrEmpty(searchString) ? repo.GetAll() : repo.Search(searchString);

            throw new NotImplementedException();

            try {

                switch (exportType) {
                    case "JSON":
                        break;
                    case "Excel":
                        break;
                    default:
                        break;
                }

                return Json(true);
            }
            catch (Exception) {
                throw;
                return null;
            }
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