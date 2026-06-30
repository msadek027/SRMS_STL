using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductVariantBEL
{
    public string VariantCode   { get; set; }
    public string ProductCode   { get; set; }
    public string VariantName   { get; set; }   // "500mg", "250mg/5ml" ইত্যাদি
    public string Status        { get; set; }
    public string Remarks       { get; set; }
    public bool   IsDeleted     { get; set; }   // UI থেকে delete মার্ক করতে

    private List<ProductPackBEL> _packs;
    public List<ProductPackBEL> Packs
    {
        get { return _packs ?? (_packs = new List<ProductPackBEL>()); }
        set { _packs = value; }
    }
}
}