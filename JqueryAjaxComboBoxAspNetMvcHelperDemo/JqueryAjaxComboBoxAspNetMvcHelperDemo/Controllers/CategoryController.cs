﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using NHibernate.Linq;
using JqueryAjaxComboBoxAspNetMvcHelperDemo.Models;



namespace JqueryAjaxComboBoxAspNetMvcHelperDemo.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Lookup(string q_word, string primary_key, int per_page, int page_num)
        {


            using (var svc = SessionFactoryBuilder.GetSessionFactory().OpenSession())
            {

                var FilteredCategory = svc.Query<Category>()
                                        .Where(x => q_word == "" || x.CategoryName.Contains(q_word));


                var PagedFilter = FilteredCategory.OrderBy(x => x.CategoryName)
                                  .LimitAndOffset(per_page, page_num)
                                  .ToList();

                return Json(
                    new
                    {
                        cnt = FilteredCategory.Count()

                        ,primary_key = PagedFilter.Select(x => x.CategoryId)

                        ,candidate = PagedFilter.Select(x => x.CategoryName)

                        ,cnt_page = PagedFilter.Count()

                        ,attached = PagedFilter.Select(x =>                    
                                new[]
                                {
                                    new string[] { "Code", x.CategoryCode } ,
                                    new string[] { "Ranking", x.Ranking.ToString() }
                                }
                            )
                    }
                    );

            }//using
        }//Lookup


        [HttpPost]
        public string Caption(string q_word)
        {            
            using (var svc = SessionFactoryBuilder.GetSessionFactory().OpenSession())
            {
                if (string.IsNullOrEmpty(q_word)) return "";

                int categoryId;
                bool isOk = int.TryParse(q_word, out categoryId);


                return
                    isOk ?
                    svc.Query<Category>()
                    .Where(x => x.CategoryId == categoryId)
                    .Select(x => x.CategoryName)
                    .SingleOrDefault()
                    : "";
            }

        }

    }
}
