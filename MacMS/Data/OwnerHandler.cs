using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data {

    public class OwnerHandler : CommandHandler<ManagementContext> {


        public OwnerHandler(ManagementContext ctx) : base(ctx) { }


        public IQueryable<Owner> GetAll() {

            return context.Set<Owner>();
        }


        public Owner Get(int id, bool include = true) {

            return context.Set<Owner>().Where(o => o.ID == id).FirstOrDefault();
        }

        public Owner Get(string name) {

            return context.Set<Owner>().Where(o => o.FullName == name).FirstOrDefault();
        }


        public IEnumerable<Owner> Sort(string sortOrder, IEnumerable<Owner> data) {

            var parameter = Expression.Parameter(typeof(Owner), "type");

            switch (sortOrder) {

                case "FirstName":
                case "FirstName_desc":

                    return sortOrder == "FirstName" ? GetSorted<Owner, string>(data, new List<string>() { "FirstName" }) : GetSorted<Owner, string>(data, new List<string>() { "FirstName" }, true);

                case "LastName":
                case "LastName_desc":

                    return sortOrder == "LastName" ? GetSorted<Owner, string>(data, new List<string>() { "LastName" }) : GetSorted<Owner, string>(data, new List<string>() {"LastName" }, true);

                case "Date":
                case "Date_desc":

                    return sortOrder == "Date" ? GetSorted<Owner, DateTime>(data, new List<string>() { "LastEdited" }) : GetSorted<Owner, DateTime>(data, new List<string>() { "LastEdited" }, true);

                case "Created":
                case "Created_desc":

                    return sortOrder == "Date" ? GetSorted<Owner, DateTime>(data, new List<string>() { "Added" }) : GetSorted<Owner, DateTime>(data, new List<string>() { "Added" }, true);
                default:
                    break;
            }

            return data;
        }


        public IEnumerable<Owner> Search(string searchString) { return GetData(searchString, GetAll()); }


        /// <summary>
        /// Searches and sorts
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortVariable"></param>
        /// <returns></returns>
        public IEnumerable<Owner> SearchSort(string searchString, string sortVariable) {

            var data = GetData(searchString, GetAll()).ToList();

            return Sort(sortVariable, data);
        }


        public IQueryable<Owner> GetData(string searchString, IQueryable<Owner> data) {

            var queries = new List<Expression<Func<Owner, bool>>>();

            var parameter = Expression.Parameter(typeof(Owner), "type");

            var queryValues = searchString.Split(", ");

            for (int i = 0; i < queryValues.Length; i++) {

                var query = queryValues[i].Split(":");
                ConstantExpression constant;

                if (query.Length > 1) {

                    constant = Expression.Constant(query[1], typeof(string));

                    switch (query[0]) {

                        case "LastEdited":
                        case "Added":

                            queries.AddRange(SearchDate(query[1], query[0], parameter));
                            break;

                        case "FirstName":
                        case "LastName":

                            queries.Add(SearchName(query[0], parameter, constant));
                            break;


                        case "Address":

                            queries.Add(Contains("Address", constant));
                            break;

                        default:
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(query[0])) {

                    constant = Expression.Constant(query[0]);
                    queries.AddRange(SearchWide(constant));
                }
            }

            return base.Search(queries, data);
        }


        private List<Expression<Func<Owner, bool>>> SearchDate(string date, string prop, ParameterExpression parameter) {

            // Gets the LastEdited property
            var property = Expression.Property(parameter, prop);

            property = Expression.Property(property, "Date");

            // Gets the object.ToString() method
            var tostring = typeof(object).GetMethod("ToString");

            // Gets the string.Contains method
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            date = date.Replace("/", " ");
            var dates = date.Split(' ');
            var queries = new List<Expression<Func<Owner, bool>>>();

            for (int i = 0; i < dates.Count(); i++) {

                var constant = Expression.Constant(dates[i], typeof(string));

                // (e => e.LastEdited.Date.ToString().Contains(constant) == searchString
                var exp = Contains(prop, constant, true);

                queries.Add(Expression.Lambda<Func<Owner, bool>>(exp, parameter));
            }

            return queries;
        }



        private Expression<Func<Owner, bool>> SearchName(string prop, ParameterExpression parameter, ConstantExpression constant) {

            var property = Expression.Property(parameter, prop);
            return Contains(prop, constant);
        }


        /// <summary>
        /// Generates an expression tree for calling string.Contains()
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="parameter"></param>
        /// <param name="constant"></param>
        /// <param name="property"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        private Expression<Func<Owner, bool>> Contains(string prop, ConstantExpression constant, bool toString = false) {

            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var parameter = Expression.Parameter(typeof(Owner), "type");
            var property = Expression.Property(parameter, prop);
            
            Expression exp;

            // If the property requires calling ToString() method on it first
            if (toString) {

                var toStringMethod = typeof(object).GetMethod("ToString");
                exp = Expression.Call(Expression.Call(property, toStringMethod), method, constant);
            }
            else {

                exp = Expression.Call(property, method, constant);
            }

            return Expression.Lambda<Func<Owner, bool>>((MethodCallExpression)exp, parameter);
        }

        public List<Expression<Func<Owner, bool>>> SearchWide(ConstantExpression constant) {

            return new List<Expression<Func<Owner, bool>>>() {

                Contains("FullName", constant),
                Contains("SSN",  constant),
                Contains("Address", constant)
            };
        }


        public List<Equipment> GetEquipment(Owner owner) {

            var test = context.Set<Equipment>().Where(e => e.Owner == owner).ToList();
            return test;
        }


    }
}
