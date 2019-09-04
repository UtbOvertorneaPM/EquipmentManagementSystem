using System.Threading.Tasks;
using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagementSystem.Controller {

    public interface IController<T> where T : class {

        Task<PartialViewResult> Table(string sortVariable, string searchString, string culture, int page = 0);

        IActionResult Create();
        Task<IActionResult> Create(T entity);

        Task<IActionResult> Delete(int id);
        Task<IActionResult> Delete(T entity, IFormCollection collection);

        Task<IActionResult> DeleteSelection(string serial);

        Task<IActionResult> Edit(int id);
        Task<IActionResult> Edit(T entity);


        Task<IActionResult> Export(string exportType, string searchString, string selection = null);
        Task<IActionResult> Import(string source, IFormFile file, bool IsEquipment = true);
        
    }
}