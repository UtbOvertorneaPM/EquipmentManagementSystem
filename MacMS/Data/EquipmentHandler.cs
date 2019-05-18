using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EquipmentManagementSystem.Models;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Diagnostics;


namespace EquipmentManagementSystem.Data {


    public class EquipmentHandler : CommandHandler<ManagementContext> {
   

        public EquipmentHandler(ManagementContext ctx) : base(ctx) { }

        public IQueryable<Equipment> GetAll() {

            return context.Set<Equipment>().Include(o => o.Owner);
        }

        public IQueryable<Equipment> GetAll(int page) {

            return context.Set<Equipment>().Include(o => o.Owner).Skip(PAGESIZE * page).Take(PAGESIZE);
        }


        public Equipment Get(int id, bool owner = true) {

            return context.Set<Equipment>().Where(e => e.ID == id).Include(o => o.Owner).FirstOrDefault();
        }

        public Owner GetOwner(int id) {

            return context.Set<Owner>().Where(o => o.ID == id).FirstOrDefault();
        }


        public IEnumerable<Equipment> Sort(IQueryable<Equipment> equipment, string sortVariable) {

            return GetSorting(sortVariable, equipment);
        }


        public IQueryable<Equipment> Search(string searchString) {

            var queryableData = GetAll();
            return GetData(searchString, queryableData);
        }



        private IQueryable<Equipment> GetData(string searchString, IQueryable<Equipment> queryableData) {

            var queries = new List<Expression<Func<Equipment, bool>>>();

            var parameter = Expression.Parameter(typeof(Equipment), "type");
            var constant = Expression.Constant(searchString, typeof(string));

            switch (searchString) {

                // Checks if searchString matches standard datetime conventions i.e / and - separators
                case var someVal when Regex.IsMatch(searchString, @"^\d[\d-\/\\]*"):
                    //case var someVal when DateTime.TryParse(searchString[i], out var date):

                    queries.AddRange(SearchDate(searchString, parameter));
                    break;

                // Checks if searchString matches any EquipmentType
                case var someVal when Enum.TryParse(searchString, true, out Equipment.EquipmentType eqp):

                    queries.Add(SearchEquipmentType(searchString, eqp, parameter, constant));
                    break;

                default:

                    queries.AddRange(SearchWide(searchString, queryableData, parameter, constant));
                    break;
            }

            return base.Search(queries, queryableData);
        }


        private List<Expression<Func<Equipment, bool>>> SearchDate(string date, ParameterExpression parameter) {

            // Gets the LastEdited property
            var property = Expression.Property(parameter, "LastEdited");

            property = Expression.Property(property, "Date");

            // Gets the object.ToString() method
            var tostring = typeof(object).GetMethod("ToString");

            // Gets the string.Contains method
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            // e.LastEdited.Date.ToString()
            var tostringValue = Expression.Call(property, tostring);

            date = date.Replace("/", " ");
            var dates = date.Split(' ');
            var queries = new List<Expression<Func<Equipment, bool>>>();

            for (int i = 0; i < dates.Count(); i++) {

                var constant = Expression.Constant(dates[i], typeof(string));

                // (e => e.LastEdited.Date.ToString().Contains(constant) == searchString
                var exp = Expression.Call(tostringValue, method, constant);

                queries.Add(Expression.Lambda<Func<Equipment, bool>>(exp, parameter));
            }

            return queries;
        }


        private Expression<Func<Equipment, bool>> SearchEquipmentType(string searchString, Equipment.EquipmentType eqp, ParameterExpression parameter, ConstantExpression constant) {

            var property = Expression.Property(parameter, "EquipType");
            constant = Expression.Constant(eqp, typeof(Equipment.EquipmentType));

            // (e => eqp == e.EquipType)
            var exp = Expression.Equal(property, constant);

            return Expression.Lambda<Func<Equipment, bool>>(exp, parameter);
        }


        private List<Expression<Func<Equipment, bool>>> SearchWide(string searchString, IQueryable<Equipment> queryableData, ParameterExpression parameter, ConstantExpression constant) {

            return new List<Expression<Func<Equipment, bool>>>() {

                Contains("FirstName", queryableData, constant, true),
                Contains("LastName", queryableData, constant, true),
                Contains("Model", queryableData, constant),
                Contains("Serial", queryableData, constant),
                Contains("Notes", queryableData, constant),
                Contains("Location", queryableData, constant)
            };
        }


        private Expression<Func<Equipment, bool>> Contains(string prop, IQueryable<Equipment> queryableData, ConstantExpression constant, bool owner = false) {

            Expression exp = null;

            // Gets the string.Contains method
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            
            var parameter = Expression.Parameter(typeof(Equipment), "type");
            var property = owner ? null : Expression.Property(parameter, prop);

            if (owner) {

                property = Expression.Property(parameter, "Owner");
                var childProperty = Expression.Property(property, prop);

                exp = Expression.Call(childProperty, method, constant);
            }
            else {

                exp = Expression.Call(property, method, constant);
            }

            return Expression.Lambda<Func<Equipment, bool>>(exp, parameter);
        }



        private IEnumerable<Equipment> GetSorting(string sortOrder, IQueryable<Equipment> data) {

            var parameter = Expression.Parameter(typeof(Equipment), "type");

            switch (sortOrder) {

                case "Owner":
                case "Owner_desc":

                    return sortOrder == "Owner" ? GetSorted<Equipment, string>(data, new List<string>() { "Owner", "FirstName" }) : GetSorted<Equipment, string>(data, new List<string>() { "Owner", "FirstName" }, true);
                    
                case "Date":
                case "Date_desc":

                    return sortOrder == "Date" ? GetSorted<Equipment, DateTime>(data, new List<string>() { "LastEdited" }) : GetSorted<Equipment, DateTime>(data, new List<string>() { "LastEdited" }, true);

                default:
                    break;
            }

            return data;
        }


    }
}
