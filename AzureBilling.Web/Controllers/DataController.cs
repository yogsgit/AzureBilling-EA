﻿using AzureBillingAPI.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Controllers
{
    public class DataController : Controller
    {
        public JsonResult SpendingBySubscription(string monthId="")
        {

            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }

            var repo = new EntityRepo<EAUsageSubscriptionSummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });
            var array =  data.Select(p => new { name = p.SubscriptionName, y = p.Amount });
            return Json(array.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SpendingByAccount(string monthId="")
        {
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageAccountSummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });
            var array = data.Select(p => new { name = p.AccountName, y = p.Amount  });
            return Json(array.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SpendingByService(string monthId = "")
        {
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageMeterSummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });

            var aggregateUsage = from us in data
                                 group us by new
                                 {
                                     MeterCategory = us.MeterCategory,
                                 }
                            into fus
                                 select new
                                 {
                                     y = fus.Sum(x => x.Amount),
                                     name = fus.Key.MeterCategory,
                                 };
            return Json(aggregateUsage.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SpendingByServiceDaily(string monthId = "")
        {
            if (string.IsNullOrEmpty(monthId))
            {
                monthId = GetMonthId();
            }
            var repo = new EntityRepo<EAUsageMeterDailySummaryEntity>();
            var data = repo.Get(monthId, new List<Tuple<string, string>> { });

            var aggregateUsage = from us in data
                                 group us by new
                                 {
                                     Date = us.Day,
                                     MeterCategory = us.MeterCategory
                                 }
                            into fus
                                 select new
                                 {
                                     y = fus.Sum(x => x.Amount),
                                     name = fus.Key.MeterCategory,
                                     Date = fus.Key.Date
                                 };
            return Json(aggregateUsage.ToList(), JsonRequestBehavior.AllowGet);
        }

        private static string GetMonthId()
        {
            string monthId;
            string month = DateTime.UtcNow.Month < 10 ? "0" + DateTime.UtcNow.Month.ToString() : DateTime.UtcNow.Month.ToString();
            string year = DateTime.UtcNow.Year.ToString();
            monthId = string.Format("{0}-{1}", year, month);
            return monthId;
        }
    }
}