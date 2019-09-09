using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EquipmentManagementSystem.Domain.Data;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Service.Export;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagementSystem.Domain.Business {

    public interface IRequestHandler {

        Task<bool> Create<T>(T equipment) where T : class;
        Task DeleteSelection(string serial);
        Task<FileStreamResult> Export(string searchString, string selection, ExportType exportType);
        IQueryable<T> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> GetAll<T>() where T : class;

        Task<bool> Remove<T>(T equipment) where T : class;
        Task<bool> Update<T>(T equipment) where T : class;
    }
}