using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductPriceMasterBEL
    {
        public string PriceMstSlno { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PriceRevisionNo { get; set; }
        public string PriceRevisionDate { get; set; }
        public string EffectStartDate { get; set; }
        public string EffectEndDate { get; set; }
        public string EffectStatus { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Remarks { get; set; }
        public virtual ICollection<ProductPriceDetailBEL> PricingDetailList { get; set; }
    }

    public class ProductPriceDetailBEL
    {
        public string PriceDtlSlNo { get; set; }
        public string PriceMstSlNo { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderTypeName { get; set; }
        public string PriceTypeCode { get; set; }
        public string PriceTypeName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyShortName { get; set; }
        public string ProductPrice { get; set; }
        public string LCPrice { get; set; }

        
    }
}