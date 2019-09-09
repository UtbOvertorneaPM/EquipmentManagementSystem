using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace EquipmentManagementSystem.Controller {


    [Authorize("Administrators")]
    public abstract class BaseController : Microsoft.AspNetCore.Mvc.Controller, IDisposable {


        protected readonly Localizer Localizer;


        public BaseController(IStringLocalizerFactory factory) {

            Localizer = new Localizer(factory);
        }


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
        public void SetLanguage(string culture) => ViewData["Language"] = string.IsNullOrEmpty(culture) ? "en-GB" : culture;

        
        // GET: Home    
        /// <summary>
        /// Used for JQuery Updating table index page
        /// </summary>
        /// <param name="sortVariable"></param>
        /// <param name="searchString"></param>
        /// <param name="culture"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual IActionResult Index(string sortVariable, string searchString, string culture, int page = 0) {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortVariable) ? "Date_desc" : sortVariable;
            culture = ViewData.ContainsKey("Language") ? ViewData["Language"].ToString() : culture;
            ViewData["Page"] = page;

            SetSearchString(ref searchString);

            SetCultureCookie(culture, Response);

            SetLanguage(culture);

            return View();
        }
        

        public void SetSearchString(ref string searchString) {

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


        public void SetCultureCookie(string culture, HttpResponse Response) {

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

    }
}
