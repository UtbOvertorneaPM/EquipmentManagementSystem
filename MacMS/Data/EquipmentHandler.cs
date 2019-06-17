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


        public IQueryable<Equipment> GetAll() { return context.Set<Equipment>().Include(o => o.Owner); }

        public Equipment Get(int id, bool owner = true) { return context.Set<Equipment>().Where(e => e.ID == id).Include(o => o.Owner).FirstOrDefault(); }


        public Owner GetOwner(int id) { return context.Set<Owner>().Where(o => o.ID == id).FirstOrDefault(); }


        public IEnumerable<Equipment> Sort(IEnumerable<Equipment> equipment, string sortVariable) { return GetSorting(sortVariable, equipment); }


        public IEnumerable<Equipment> Search(string searchString) { return GetData(searchString, GetAll()); }
        

        /// <summary>
        /// Searches and sorts
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortVariable"></param>
        /// <returns></returns>
        public IEnumerable<Equipment> SearchSort(string searchString, string sortVariable) {

            var data = GetData(searchString, GetAll()).ToList();

            for (int i = 0; i < data.Count; i++) {

                if (data[i].Owner is null) {

                    var owner = new Owner();
                    owner.FirstName = "";
                    data[i].Owner = owner;
                }
            }

            return GetSorting(sortVariable, data);
        }
        

        private IQueryable<Equipment> GetData(string searchString, IQueryable<Equipment> data) {

            var queries = new List<Expression<Func<Equipment, bool>>>();

            var parameter = Expression.Parameter(typeof(Equipment), "type");
            
            var queryValues = searchString.Split(",");

            for (int i = 0; i < queryValues.Length; i++) {

                var query = queryValues[i].Split(":");
                ConstantExpression constant;

                if (query.Length > 1) {

                    constant = Expression.Constant(query[1], typeof(string));

                    switch (query[0]) {

                        case "LastEdited":

                            queries.AddRange(SearchDate(query[1], parameter));
                            break;

                        case "Model":
                        case "Serial":

                            queries.Add(Contains(query[0], constant));
                            break;

                        case "Owner":

                            queries.Add(Contains("FullName", constant, true));
                            break;

                        case "EquipType":

                            Enum.TryParse<Equipment.EquipmentType>(query[1], out var eqpVal);
                            queries.Add(SearchEquipmentType(eqpVal, parameter));
                            break;
                        
                        default:
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(query[0]) && !string.IsNullOrWhiteSpace(query[0])) {

                    constant = Expression.Constant(query[0]);
                    queries.AddRange(SearchWide(parameter, constant));
                }
            }

            return base.Search(queries, data);
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


        private Expression<Func<Equipment, bool>> SearchEquipmentType(Equipment.EquipmentType eqp, ParameterExpression parameter) {

            var property = Expression.Property(parameter, "EquipType");
            var constant = Expression.Constant(eqp, typeof(Equipment.EquipmentType));

            // (e => eqp == e.EquipType)
            var exp = Expression.Equal(property, constant);

            return Expression.Lambda<Func<Equipment, bool>>(exp, parameter);
        }


        private List<Expression<Func<Equipment, bool>>> SearchWide(ParameterExpression parameter, ConstantExpression constant) {

            return new List<Expression<Func<Equipment, bool>>>() {

                Contains("FirstName", constant, true),
                Contains("LastName", constant, true),
                Contains("Model",  constant),
                Contains("Serial", constant),
                Contains("Notes", constant),
                Contains("Location", constant)
            };
        }


        private Expression<Func<Equipment, bool>> Contains(string prop, ConstantExpression constant, bool owner = false) {

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


        private IEnumerable<Equipment> GetSorting(string sortOrder, IEnumerable<Equipment> data) {

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
