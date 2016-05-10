using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models.Billing
{
    public class OfferTerm
    {
        public string Name { get; set; }
        public dynamic TieredDiscount { get; set; }
        public List<object> ExcludedMeterIds { get; set; }
        public string EffectiveDate { get; set; }
    }

    public class MeterRates
    {
        public double _0 { get; set; }
    }

    public class Meter
    {
        public string MeterId { get; set; }
        public string MeterName { get; set; }
        public string MeterCategory { get; set; }
        public string MeterSubCategory { get; set; }
        public string Unit { get; set; }
        public List<object> MeterTags { get; set; }
        public dynamic MeterRates { get; set; }
        public string EffectiveDate { get; set; }
        public double IncludedQuantity { get; set; }
    }

    public class BillingAggregate
    {
        public List<OfferTerm> OfferTerms { get; set; }
        public List<Meter> Meters { get; set; }
    }
}