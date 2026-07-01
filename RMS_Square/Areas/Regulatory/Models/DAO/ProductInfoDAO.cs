using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class ProductInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<ProductInfoBEL> GetProductList()
        {
            string Qry = @"SELECT 
                    C.COMPANY_CODE, C.COMPANY_NAME, C.LICENSE_NO, 
                    U.COMPANY_UNIT_CODE, U.COMPANY_UNIT_NAME,
                    P.PRODUCT_CODE,P.PRODUCT_NAME, P.SAP_PRODUCT_CODE, P.GENERIC_CODE, P.STRENGTH_CODE, S.STRENGTH_NAME, 
                    P.DOSAGE_FORM_CODE, D.DOSAGE_FORM_NAME, P.PACK_SIZE_NAME, P.BRAND_NAME, 
                    P.PRODUCT_CATEGORY, P.THERAPEUTIC_CLASS_CODE, T.THERAPEUTIC_CLASS_NAME, 
                    P.PRODUCT_SPECIFICATION, P.INTRODUCED_BANGLADESH, P.MANUFACTURING_TYPE, 
                    P.PRODUCT_TYPE_CODE, FN_PRODUCT_TYPE_NAME(P.PRODUCT_TYPE_CODE) AS PRODUCT_TYPE_NAME, P.PRODUCT_VARIANT,
                    P.STATUS, P.REMARKS, TO_CHAR(P.SET_ON, 'YYYYMM') AS YearMonth
                FROM PRODUCT_INFO P
                LEFT JOIN COMPANY_UNIT_INFO U      ON P.COMPANY_CODE = U.COMPANY_UNIT_CODE
                LEFT JOIN COMPANY_INFO C           ON U.COMPANY_CODE = C.COMPANY_CODE
                LEFT JOIN STRENGTH_INFO S          ON P.STRENGTH_CODE = S.STRENGTH_CODE
                LEFT JOIN DOSAGE_FORM_INFO D       ON P.DOSAGE_FORM_CODE = D.DOSAGE_FORM_CODE
                LEFT JOIN THERAPEUTIC_CLASS_INFO T ON P.THERAPEUTIC_CLASS_CODE = T.THERAPEUTIC_CLASS_CODE
                ORDER BY P.SET_ON DESC";

            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);

            var item = (from DataRow row in dt.Rows
                        select new ProductInfoBEL
                        {
                            // New Unit Fields
                            CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                            CompanyUnitName = row["COMPANY_UNIT_NAME"].ToString(),
                            ProductVariant = row["PRODUCT_VARIANT"].ToString(),

                            // Existing Fields
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            ProductName = row["PRODUCT_NAME"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenericCode = row["GENERIC_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            StrengthCode = row["STRENGTH_CODE"].ToString(),
                            StrengthName = row["STRENGTH_NAME"].ToString(),
                            DosageFormCode = row["DOSAGE_FORM_CODE"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            TherapeuticClassCode = row["THERAPEUTIC_CLASS_CODE"].ToString(),
                            TherapeuticClassName = row["THERAPEUTIC_CLASS_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString(),
                            IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                            ManufacturingType = row["MANUFACTURING_TYPE"].ToString(),
                            ProductTypeCode = row["PRODUCT_TYPE_CODE"].ToString(),
                            ProductTypeName = row["PRODUCT_TYPE_NAME"].ToString(),
                            Status = row["STATUS"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            YearMonth = row["YearMonth"].ToString()
                        }).ToList();

            return item;
        }

        //private string EscapeSql(string input)
        //{
        //    return string.IsNullOrWhiteSpace(input)
        //        ? ""
        //        : input.Replace("'", "''").Trim();
        //}

        //public bool SaveUpdate(ProductInfoBEL master, string userId)
        //{
        //    string qry = "";
        //    string setOnDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //    string pCode = "";

        //    try
        //    {
        //        string pName = EscapeSql(master.ProductName);

        //        if (string.IsNullOrWhiteSpace(master.ProductCode))
        //        {
        //            IUMode = "I";

        //            // Generate Product Code
        //            string maxIdQry = "SELECT NVL(MAX(TO_NUMBER(PRODUCT_CODE)),0)+1 FROM PRODUCT_INFO";

        //            DataTable dtMax = dbHelper.GetDataTable(dbConn.SAConnStrReader(), maxIdQry);

        //            pCode = dtMax.Rows[0][0].ToString();

        //            // Return generated code to controller
        //            this.MaxID = pCode;

        //            qry = "INSERT INTO PRODUCT_INFO " +
        //                  "(PRODUCT_CODE, PRODUCT_VARIANT, PRODUCT_NAME, COMPANY_CODE, SAP_PRODUCT_CODE, BRAND_NAME, " +
        //                  "GENERIC_CODE, STRENGTH_CODE, DOSAGE_FORM_CODE, PACK_SIZE_NAME, PRODUCT_CATEGORY, " +
        //                  "THERAPEUTIC_CLASS_CODE, PRODUCT_SPECIFICATION, INTRODUCED_BANGLADESH, MANUFACTURING_TYPE, " +
        //                  "PRODUCT_TYPE_CODE, STATUS, REMARKS, SET_BY, SET_ON) " +
        //                  "VALUES (" +
        //                  "'" + pCode + "'," +
        //                  "'" + EscapeSql(master.ProductVariant) + "'," +
        //                  "'" + pName + "'," +
        //                  "'" + EscapeSql(master.CompanyCode) + "'," +
        //                  "'" + EscapeSql(master.SAPProductCode) + "'," +
        //                  "'" + EscapeSql(master.BrandName) + "'," +
        //                  "'" + EscapeSql(master.GenericCode) + "'," +
        //                  "'" + EscapeSql(master.StrengthCode) + "'," +
        //                  "'" + EscapeSql(master.DosageFormCode) + "'," +
        //                  "'" + EscapeSql(master.PackSizeName) + "'," +
        //                  "'" + EscapeSql(master.ProductCategory) + "'," +
        //                  "'" + EscapeSql(master.TherapeuticClassCode) + "'," +
        //                  "'" + EscapeSql(master.ProductSpecification) + "'," +
        //                  "'" + EscapeSql(master.IntroducedInBD) + "'," +
        //                  "'" + EscapeSql(master.ManufacturingType) + "'," +
        //                  "'" + EscapeSql(master.ProductTypeCode) + "'," +
        //                  "'" + EscapeSql(master.Status) + "'," +
        //                  "'" + EscapeSql(master.Remarks) + "'," +
        //                  "'" + EscapeSql(userId) + "'," +
        //                  "TO_DATE('" + setOnDate + "','dd/MM/yyyy HH24:mi:ss'))";
        //        }
        //        else
        //        {
        //            IUMode = "U";

        //            pCode = master.ProductCode.Trim();

        //            this.MaxID = pCode;

        //            qry = "UPDATE PRODUCT_INFO SET " +
        //                  "PRODUCT_VARIANT = '" + EscapeSql(master.ProductVariant) + "', " +
        //                  "PRODUCT_NAME = '" + pName + "', " +
        //                  "SAP_PRODUCT_CODE = '" + EscapeSql(master.SAPProductCode) + "', " +
        //                  "COMPANY_CODE = '" + EscapeSql(master.CompanyCode) + "', " +
        //                  "BRAND_NAME = '" + EscapeSql(master.BrandName) + "', " +
        //                  "GENERIC_CODE = '" + EscapeSql(master.GenericCode) + "', " +
        //                  "STRENGTH_CODE = '" + EscapeSql(master.StrengthCode) + "', " +
        //                  "DOSAGE_FORM_CODE = '" + EscapeSql(master.DosageFormCode) + "', " +
        //                  "PACK_SIZE_NAME = '" + EscapeSql(master.PackSizeName) + "', " +
        //                  "PRODUCT_CATEGORY = '" + EscapeSql(master.ProductCategory) + "', " +
        //                  "THERAPEUTIC_CLASS_CODE = '" + EscapeSql(master.TherapeuticClassCode) + "', " +
        //                  "PRODUCT_SPECIFICATION = '" + EscapeSql(master.ProductSpecification) + "', " +
        //                  "INTRODUCED_BANGLADESH = '" + EscapeSql(master.IntroducedInBD) + "', " +
        //                  "MANUFACTURING_TYPE = '" + EscapeSql(master.ManufacturingType) + "', " +
        //                  "PRODUCT_TYPE_CODE = '" + EscapeSql(master.ProductTypeCode) + "', " +
        //                  "STATUS = '" + EscapeSql(master.Status) + "', " +
        //                  "REMARKS = '" + EscapeSql(master.Remarks) + "', " +
        //                  "UPDATE_BY = '" + EscapeSql(userId) + "', " +
        //                  "UPDATE_DATE = TO_DATE('" + setOnDate + "','dd/MM/yyyy HH24:mi:ss') " +
        //                  "WHERE PRODUCT_CODE = '" + pCode + "'";
        //        }

        //        return dbHelper.CmdExecute(dbConn.SAConnStrReader(), qry);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public bool SaveUpdate(ProductInfoBEL master, string userId)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string pCode = "";

            // ── TIER 1: PRODUCT_INFO ─────────────────────────────
            if (string.IsNullOrWhiteSpace(master.ProductCode))
            {
                IUMode = "I";
                pCode = GetNextId("SELECT NVL(MAX(TO_NUMBER(PRODUCT_CODE)),0)+1 FROM PRODUCT_INFO");
                MaxID = pCode;

                string qry =
                    "INSERT INTO PRODUCT_INFO " +
                    "(PRODUCT_CODE, COMPANY_CODE, PRODUCT_NAME, BRAND_NAME, GENERIC_CODE, " +
                    " DOSAGE_FORM_CODE, THERAPEUTIC_CLASS_CODE, PRODUCT_CATEGORY, " +
                    " MANUFACTURING_TYPE, PRODUCT_TYPE_CODE, PRODUCT_SPECIFICATION, " +
                    " INTRODUCED_BANGLADESH, STATUS, REMARKS, SET_BY, SET_ON) " +
                    "VALUES (" +
                    "'" + pCode + "'," +
                    "'" + E(master.CompanyCode) + "'," +
                    "'" + E(master.ProductName) + "'," +
                    "'" + E(master.BrandName) + "'," +
                    "'" + E(master.GenericCode) + "'," +
                    "'" + E(master.DosageFormCode) + "'," +
                    "'" + E(master.TherapeuticClassCode) + "'," +
                    "'" + E(master.ProductCategory) + "'," +
                    "'" + E(master.ManufacturingType) + "'," +
                    "'" + E(master.ProductTypeCode) + "'," +
                    "'" + E(master.ProductSpecification) + "'," +
                    "'" + E(master.IntroducedInBD) + "'," +
                    "'" + E(master.Status) + "'," +
                    "'" + E(master.Remarks) + "'," +
                    "'" + E(userId) + "'," +
                    "TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss'))";

                dbHelper.CmdExecute(dbConn.SAConnStrReader(), qry);
            }
            else
            {
                IUMode = "U";
                pCode = master.ProductCode.Trim();
                MaxID = pCode;

                string qry =
                    "UPDATE PRODUCT_INFO SET " +
                    " COMPANY_CODE           = '" + E(master.CompanyCode) + "'," +
                    " PRODUCT_NAME           = '" + E(master.ProductName) + "'," +
                    " BRAND_NAME             = '" + E(master.BrandName) + "'," +
                    " GENERIC_CODE           = '" + E(master.GenericCode) + "'," +
                    " DOSAGE_FORM_CODE       = '" + E(master.DosageFormCode) + "'," +
                    " THERAPEUTIC_CLASS_CODE = '" + E(master.TherapeuticClassCode) + "'," +
                    " PRODUCT_CATEGORY       = '" + E(master.ProductCategory) + "'," +
                    " MANUFACTURING_TYPE     = '" + E(master.ManufacturingType) + "'," +
                    " PRODUCT_TYPE_CODE      = '" + E(master.ProductTypeCode) + "'," +
                    " PRODUCT_SPECIFICATION  = '" + E(master.ProductSpecification) + "'," +
                    " INTRODUCED_BANGLADESH  = '" + E(master.IntroducedInBD) + "'," +
                    " STATUS                 = '" + E(master.Status) + "'," +
                    " REMARKS                = '" + E(master.Remarks) + "'," +
                    " UPDATE_BY              = '" + E(userId) + "'," +
                    " UPDATE_DATE            = TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss')" +
                    " WHERE PRODUCT_CODE = '" + pCode + "'";

                dbHelper.CmdExecute(dbConn.SAConnStrReader(), qry);
            }

            // ── TIER 2 & 3: VARIANTS + PACKS ────────────────────
            if (master.Variants != null && master.Variants.Count > 0)
            {
                int vIdx = 1;
                foreach (var v in master.Variants)
                {
                    // Delete করতে হবে
                    if (v.IsDeleted && !string.IsNullOrWhiteSpace(v.VariantCode))
                    {
                        dbHelper.CmdExecute(dbConn.SAConnStrReader(),
                            "DELETE FROM PRODUCT_PACK_INFO WHERE VARIANT_CODE='" + E(v.VariantCode) + "'");
                        dbHelper.CmdExecute(dbConn.SAConnStrReader(),
                            "DELETE FROM PRODUCT_VARIANT_INFO WHERE VARIANT_CODE='" + E(v.VariantCode) + "'");
                        vIdx++;
                        continue;
                    }

                    if (v.IsDeleted) { vIdx++; continue; } // নতুন কিন্তু deleted — skip

                    string vCode = "";

                    if (string.IsNullOrWhiteSpace(v.VariantCode))
                    {
                        // নতুন Variant
                        vCode = pCode + "-" + vIdx.ToString();

                        string vQry =
                            "INSERT INTO PRODUCT_VARIANT_INFO " +
                            "(VARIANT_CODE, PRODUCT_CODE, VARIANT_NAME, STATUS, REMARKS, SET_BY, SET_ON) " +
                            "VALUES (" +
                            "'" + vCode + "'," +
                            "'" + pCode + "'," +
                            "'" + E(v.VariantName) + "'," +
                            "'" + E(v.Status ?? "Active") + "'," +
                            "'" + E(v.Remarks) + "'," +
                            "'" + E(userId) + "'," +
                            "TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss'))";

                        dbHelper.CmdExecute(dbConn.SAConnStrReader(), vQry);
                    }
                    else
                    {
                        // আপডেট Variant
                        vCode = v.VariantCode;

                        string vQry =
                            "UPDATE PRODUCT_VARIANT_INFO SET " +
                            " VARIANT_NAME = '" + E(v.VariantName) + "'," +
                            " STATUS       = '" + E(v.Status) + "'," +
                            " REMARKS      = '" + E(v.Remarks) + "'," +
                            " UPDATE_BY    = '" + E(userId) + "'," +
                            " UPDATE_DATE  = TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss')" +
                            " WHERE VARIANT_CODE = '" + vCode + "'";

                        dbHelper.CmdExecute(dbConn.SAConnStrReader(), vQry);
                    }

                    // ── TIER 3: PACKS ────────────────────────────
                    if (v.Packs != null && v.Packs.Count > 0)
                    {
                        int pIdx = 1;
                        foreach (var pk in v.Packs)
                        {
                            if (pk.IsDeleted && !string.IsNullOrWhiteSpace(pk.PackCode))
                            {
                                dbHelper.CmdExecute(dbConn.SAConnStrReader(),
                                    "DELETE FROM PRODUCT_PACK_INFO WHERE PACK_CODE='" + E(pk.PackCode) + "'");
                                pIdx++;
                                continue;
                            }

                            if (pk.IsDeleted) { pIdx++; continue; }

                            if (string.IsNullOrWhiteSpace(pk.PackCode))
                            {
                                // নতুন Pack
                                string pkCode = vCode + "-" + pIdx.ToString();

                                string pkQry =
                                    "INSERT INTO PRODUCT_PACK_INFO " +
                                    "(PACK_CODE, VARIANT_CODE, PRODUCT_CODE, SAP_PRODUCT_CODE, " +
                                    " PACK_SIZE_NAME, STATUS, REMARKS, SET_BY, SET_ON) " +
                                    "VALUES (" +
                                    "'" + pkCode + "'," +
                                    "'" + vCode + "'," +
                                    "'" + pCode + "'," +
                                    "'" + E(pk.SAPProductCode) + "'," +
                                    "'" + E(pk.PackSizeName) + "'," +
                                    "'" + E(pk.Status ?? "Active") + "'," +
                                    "'" + E(pk.Remarks) + "'," +
                                    "'" + E(userId) + "'," +
                                    "TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss'))";

                                dbHelper.CmdExecute(dbConn.SAConnStrReader(), pkQry);
                            }
                            else
                            {
                                // আপডেট Pack
                                string pkQry =
                                    "UPDATE PRODUCT_PACK_INFO SET " +
                                    " SAP_PRODUCT_CODE = '" + E(pk.SAPProductCode) + "'," +
                                    " PACK_SIZE_NAME   = '" + E(pk.PackSizeName) + "'," +
                                    " STATUS           = '" + E(pk.Status) + "'," +
                                    " REMARKS          = '" + E(pk.Remarks) + "'," +
                                    " UPDATE_BY        = '" + E(userId) + "'," +
                                    " UPDATE_DATE      = TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss')" +
                                    " WHERE PACK_CODE = '" + E(pk.PackCode) + "'";

                                dbHelper.CmdExecute(dbConn.SAConnStrReader(), pkQry);
                            }
                            pIdx++;
                        }
                    }
                    vIdx++;
                }
            }

            return true;
        }

        // ── Helpers ──────────────────────────────────────────────
        // ── Helpers ──────────────────────────────────────────────
        private string E(string s)
        {
            return EscapeSql(s);
        }

        // Add this missing method here
        private string EscapeSql(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            // Replaces single quotes with two single quotes for Oracle SQL safety
            return input.Replace("'", "''").Trim();
        }

        private string GetNextId(string qry)
        {
            var dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qry);
            return dt.Rows[0][0].ToString();
        }
        
        //public List<ProductInfoBEL> GetAllActiveProduct(string companyCode)
        //{
        //    var query = new System.Text.StringBuilder();

        //    query.Append(" SELECT C.COMPANY_CODE,C.COMPANY_UNIT_CODE,C.COMPANY_UNIT_NAME,P.PRODUCT_CODE,P.PRODUCT_NAME,P.SAP_PRODUCT_CODE,P.GENERIC_CODE, P.PACK_SIZE_NAME,");
        //    query.Append(" P.BRAND_NAME, P.PRODUCT_CATEGORY,P.PRODUCT_SPECIFICATION,P.INTRODUCED_BANGLADESH, ");
        //    query.Append(" P.MANUFACTURING_TYPE,P.PRODUCT_TYPE_CODE, FN_PRODUCT_TYPE_NAME(P.PRODUCT_TYPE_CODE) PRODUCT_TYPE_NAME,P.STATUS,  P.REMARKS ,TO_CHAR(p.SET_ON, 'YYYY')||TO_CHAR(p.SET_ON, 'MM') as YearMonth ");
        //    query.Append(" FROM PRODUCT_INFO p,COMPANY_UNIT_INFO C ");
        //    query.Append(" WHERE P.COMPANY_CODE=C.COMPANY_UNIT_CODE(+) AND P.STATUS='Active' ");

        //    if (!string.IsNullOrEmpty(companyCode))
        //    {
        //        query.Append(" AND P.COMPANY_CODE ='" + companyCode + "'");
        //    }
        //    query.Append(" ORDER BY P.PRODUCT_CODE ");
        //    DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), query.ToString());
        //    List<ProductInfoBEL> item;

        //    item = (from DataRow row in dt.Rows
        //            select new ProductInfoBEL
        //            {
        //                ProductCode = row["PRODUCT_CODE"].ToString(),
        //                ProductName = row["PRODUCT_NAME"].ToString(),
        //                SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
        //                PackSizeName = row["PACK_SIZE_NAME"].ToString(),
        //                BrandName = row["BRAND_NAME"].ToString(),
        //                CompanyCode = row["COMPANY_UNIT_CODE"].ToString(),
        //                CompanyName = row["COMPANY_UNIT_NAME"].ToString(),
        //                ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
        //                ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString(),
        //                IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
        //                ManufacturingType = row["MANUFACTURING_TYPE"].ToString(),
        //                ProductTypeCode = row["PRODUCT_TYPE_CODE"].ToString(),
        //                ProductTypeName = row["PRODUCT_TYPE_NAME"].ToString(),
        //                Status = row["STATUS"].ToString(),
        //                Remarks = row["REMARKS"].ToString(),
        //                YearMonth = row["YearMonth"].ToString()
        //            }).ToList();
        //    return item;
        //}

        public List<ProductInfoBEL> GetAllActiveProduct(string companyCode)
        {
            var query = new System.Text.StringBuilder();

            query.Append(" SELECT C.COMPANY_CODE, C.COMPANY_UNIT_CODE, C.COMPANY_UNIT_NAME, ");
            query.Append("        P.PRODUCT_CODE, P.PRODUCT_NAME, P.SAP_PRODUCT_CODE, P.GENERIC_CODE, ");
            query.Append("        P.BRAND_NAME, P.PRODUCT_CATEGORY, P.PRODUCT_SPECIFICATION, ");
            query.Append("        P.INTRODUCED_BANGLADESH, P.MANUFACTURING_TYPE, P.PRODUCT_TYPE_CODE, ");
            query.Append("        FN_PRODUCT_TYPE_NAME(P.PRODUCT_TYPE_CODE) PRODUCT_TYPE_NAME, ");
            query.Append("        P.STATUS, P.REMARKS, ");
            query.Append("        TO_CHAR(P.SET_ON,'YYYY')||TO_CHAR(P.SET_ON,'MM') AS YearMonth, ");

            // Variant names — comma separated
            query.Append("        (SELECT LISTAGG(V.VARIANT_NAME, ', ') WITHIN GROUP (ORDER BY V.VARIANT_CODE) ");
            query.Append("           FROM PRODUCT_VARIANT_INFO V ");
            query.Append("          WHERE V.PRODUCT_CODE = P.PRODUCT_CODE ");
            query.Append("            AND V.STATUS = 'Active') AS PRODUCT_VARIANT, ");

            // Pack sizes — comma separated (across all variants)
            query.Append("        (SELECT LISTAGG(PK.PACK_SIZE_NAME, ', ') WITHIN GROUP (ORDER BY PK.PACK_CODE) ");
            query.Append("           FROM PRODUCT_PACK_INFO PK ");
            query.Append("          WHERE PK.PRODUCT_CODE = P.PRODUCT_CODE ");
            query.Append("            AND PK.STATUS = 'Active') AS PACK_SIZE_NAME ");

            query.Append(" FROM PRODUCT_INFO P, COMPANY_UNIT_INFO C ");
            query.Append(" WHERE P.COMPANY_CODE = C.COMPANY_UNIT_CODE(+) ");
            query.Append("   AND P.STATUS = 'Active' ");

            if (!string.IsNullOrEmpty(companyCode))
            {
                query.Append(" AND P.COMPANY_CODE = '" + companyCode + "'");
            }

            query.Append(" ORDER BY P.PRODUCT_CODE ");

            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), query.ToString());

            var item = (from DataRow row in dt.Rows
                        select new ProductInfoBEL
                        {
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            ProductName = row["PRODUCT_NAME"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            ProductVariant = row["PRODUCT_VARIANT"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            CompanyCode = row["COMPANY_UNIT_CODE"].ToString(),
                            CompanyName = row["COMPANY_UNIT_NAME"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString(),
                            IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                            ManufacturingType = row["MANUFACTURING_TYPE"].ToString(),
                            ProductTypeCode = row["PRODUCT_TYPE_CODE"].ToString(),
                            ProductTypeName = row["PRODUCT_TYPE_NAME"].ToString(),
                            Status = row["STATUS"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            YearMonth = row["YearMonth"].ToString()
                        }).ToList();

            return item;
        }
        public List<ProductInfoBEL> GetAllProduct(string companyCode)
        {
            var query = new System.Text.StringBuilder();
            query.Append("SELECT PRODUCT_CODE, SAP_PRODUCT_CODE,GENERIC_STRENGTH, PACK_SIZE_NAME, DOSAGE_FORM_NAME,COMPANY_CODE,COMPANY_NAME,DAR_NO,BRAND_NAME FROM MAC_PROD_INFO WHERE 1=1 ");
            if (!string.IsNullOrEmpty(companyCode))
            {
                query.Append(" AND COMPANY_CODE ='" + companyCode + "'");
            }
            
            query.Append(" ORDER BY PRODUCT_CODE");

            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), query.ToString());
            List<ProductInfoBEL> item;
            item = (from DataRow row in dt.Rows
                    select new ProductInfoBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericStrength = row["GENERIC_STRENGTH"].ToString(),
                        GenAndStrength = row["GENERIC_STRENGTH"].ToString(),
                        PackSize = row["PACK_SIZE_NAME"].ToString(),
                        PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                        DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                        DarNo = row["DAR_NO"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                    }).ToList();
            return item;
        }

        /*       public List<ProductInfoBEL> GetProductFromAnnex(string companyCode)
               {
                   var query = new System.Text.StringBuilder();

                   query.Append(" SELECT A.DAR_NO,A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.COMPANY_CODE,A.COMPANY_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,");
                   query.Append(" A.LICENSE_NO,A.DOSAGE_FORM_NAME FROM ( SELECT D.ANNEX_ID,D.REVISION_NO,D.RECIPE_ID,D.DAR_NO,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,P.PRODUCT_VARIANT, ");
                   query.Append(" C.COMPANY_CODE,C.COMPANY_NAME, C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  ");
                   query.Append(" FROM PRODUCT_REGISTRATION_INFO D LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE ");
                   query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE ");
                   query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE D.IS_DELETE <>'Y' ) A ");
                   query.Append(" INNER JOIN ( SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ");
                   query.Append(" ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO WHERE 1=1 ");

                   if (!string.IsNullOrEmpty(companyCode))
                   {
                       query.Append(" AND COMPANY_CODE ='" + companyCode + "'");
                   }

                   DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), query.ToString());
                   List<ProductInfoBEL> item;

                   item = (from DataRow row in dt.Rows
                           select new ProductInfoBEL
                           {
                               ProductCode = row["PRODUCT_CODE"].ToString(),
                               SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                               GenericStrength = row["GENERIC_CODE"].ToString(),
                               GenAndStrength = row["GENERIC_CODE"].ToString(),
                               PackSize = row["PACK_SIZE_NAME"].ToString(),
                               PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                               DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                               DarNo = row["DAR_NO"].ToString(),
                               BrandName = row["BRAND_NAME"].ToString(),
                               CompanyName = row["COMPANY_NAME"].ToString(),
                               CompanyCode = row["COMPANY_CODE"].ToString()

                           }).ToList();
                   return item;
               }*/

        public List<ProductInfoBEL> GetProductFromAnnex(string companyCode)
        {
            string query = @"
        SELECT PRODUCT_CODE,
               SAP_PRODUCT_CODE,
               GENERIC_CODE,
               STRENGTH_CODE,
               DOSAGE_FORM_CODE,
               BRAND_NAME,
               PRODUCT_CATEGORY,
               THERAPEUTIC_CLASS_CODE,
               PRODUCT_SPECIFICATION,
               INTRODUCED_BANGLADESH,
               MANUFACTURING_TYPE,
               PRODUCT_TYPE_CODE,
               STATUS,
               REMARKS,
               SET_BY,
               SET_ON,
               UPDATE_BY,
               UPDATE_DATE,
               PACK_SIZE_NAME,
               COMPANY_CODE,
               PRODUCT_NAME,
               PRODUCT_VARIANT
          FROM PRODUCT_INFO
         WHERE COMPANY_CODE = :COMPANY_CODE";

            OracleParameter[] parameters =
            {
        new OracleParameter("COMPANY_CODE", companyCode)
    };

            DataTable dt = dbHelper.GetDataTable(
                dbConn.SAConnStrReader(),
                query,
                parameters);

            return (from DataRow row in dt.Rows
                    select new ProductInfoBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericCode = row["GENERIC_CODE"].ToString(),
                        StrengthCode = row["STRENGTH_CODE"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                        PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        ProductName = row["PRODUCT_NAME"].ToString(),
                        ProductVariant = row["PRODUCT_VARIANT"].ToString(),
                        Status = row["STATUS"].ToString(),
                        Remarks = row["REMARKS"].ToString()
                    }).ToList();
        }


        public List<ProductInfoBEL> GetProductFromRecipe(string companyCode)
        {
            var query = new System.Text.StringBuilder();

            query.Append(" SELECT A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.COMPANY_CODE,A.COMPANY_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME, A.LICENSE_NO,A.DOSAGE_FORM_NAME FROM (");
            query.Append(" SELECT D.ID,D.REVISION_NO,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,C.COMPANY_CODE,C.COMPANY_NAME, C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME ");
            query.Append(" FROM RECIPE_INFO D LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE ");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE D.IS_DELETE <>'Y' ");
            if (!string.IsNullOrEmpty(companyCode))
            {
                query.Append(" AND D.COMPANY_CODE ='" + companyCode + "'");
            }
            query.Append(" ) A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ");

            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO ");
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), query.ToString());
            List<ProductInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ProductInfoBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericStrength = row["GENERIC_CODE"].ToString(),
                        PackSize = row["PACK_SIZE_NAME"].ToString(),
                        DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString()

                    }).ToList();
            return item;
        }


        public List<ProductInfoBEL> GetInfoByParams(ProductInfoBEL model, string orderBy)
        {
            var query = new System.Text.StringBuilder();

            query.Append(" SELECT P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.GENERIC_CODE ,P.STRENGTH_CODE,S.STRENGTH_NAME,P.DOSAGE_FORM_CODE,D.DOSAGE_FORM_NAME,P.PACK_SIZE_NAME,");
            query.Append(" P.BRAND_NAME, P.PRODUCT_CATEGORY,P.THERAPEUTIC_CLASS_CODE,T.THERAPEUTIC_CLASS_NAME,P.PRODUCT_SPECIFICATION,P.INTRODUCED_BANGLADESH, ");
            query.Append(" P.MANUFACTURING_TYPE,P.PRODUCT_TYPE_CODE, FN_PRODUCT_TYPE_NAME(P.PRODUCT_TYPE_CODE) PRODUCT_TYPE_NAME,P.STATUS,P.REMARKS,TO_CHAR(P.SET_ON, 'YYYY')||TO_CHAR(P.SET_ON, 'MM') as YearMonth  ");
            query.Append(" FROM PRODUCT_INFO P  LEFT JOIN  STRENGTH_INFO S ON S.STRENGTH_CODE=P.STRENGTH_CODE  LEFT JOIN DOSAGE_FORM_INFO D ON D.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            query.Append(" LEFT JOIN  THERAPEUTIC_CLASS_INFO T ON T.THERAPEUTIC_CLASS_CODE=P.THERAPEUTIC_CLASS_CODE ");
            query.Append("  WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND P.PRODUCT_CODE ='" + model.ProductCode + "'");
            }
            if (!string.IsNullOrEmpty(model.LastDays))
            {
                query.Append(" AND ROUND(((SELECT SYSDATE FROM DUAL)-( P.SET_ON)),0)<={0}");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND P.SET_ON BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            //if (string.IsNullOrEmpty(model.ProductCode) && string.IsNullOrEmpty(model.LastDays) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            //{
            //    query.Append(" AND ROUND(((SELECT SYSDATE FROM DUAL)-( P.SET_ON)),0)<= 60 ");
            //}
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(),string.Format(query.ToString(), model.LastDays));
            List<ProductInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ProductInfoBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericCode = row["GENERIC_CODE"].ToString(),
                        GenAndStrength = row["GENERIC_CODE"].ToString(),
                        StrengthCode = row["STRENGTH_CODE"].ToString(),
                        StrengthName = row["STRENGTH_NAME"].ToString(),
                        DosageFormCode = row["DOSAGE_FORM_CODE"].ToString(),
                        DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                        PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                        TherapeuticClassCode = row["THERAPEUTIC_CLASS_CODE"].ToString(),
                        TherapeuticClassName = row["THERAPEUTIC_CLASS_NAME"].ToString(),
                        ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString(),
                        IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                        ManufacturingType = row["MANUFACTURING_TYPE"].ToString(),
                        ProductTypeCode = row["PRODUCT_TYPE_CODE"].ToString(),
                        ProductTypeName = row["PRODUCT_TYPE_NAME"].ToString(),
                        Status = row["STATUS"].ToString(),
                        Remarks = row["REMARKS"].ToString(),
                        YearMonth = row["YearMonth"].ToString()

                    }).ToList();
            return item;
        }

        public DataTable GetProductInfoForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY R.ID)SN, R.ID,MA.MARKET_AUTHORIZATION_NO MANo, C.COMPANY_CODE,C.COMPANY_NAME, P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.BRAND_NAME,P.GENERIC_CODE GENERIC_STRENGTH,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY,DF.DOSAGE_FORM_NAME, PP.LAUNCHING_DATE, P.PACK_SIZE_NAME, PR.DTL_APPROVAL_DATE,");
            query.Append(" PR.DAR_NO,PR.INCLUSION_DATE, PP.PRICE_PER_UNIT,PP.PROPOSED_BY,MA.SUBMISSION_DATE,MA.APPROVAL_DATE FROM(SELECT A.ID, A.PRODUCT_CODE,A.COMPANY_CODE FROM RECIPE_INFO A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) ");
            query.Append(" B ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1");

            //if (!string.IsNullOrEmpty(model.CompanyCode))
            //{
            //    query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
            //}
            query.Append(" )R INNER JOIN(SELECT A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.RECIPE_ID,A.DAR_NO,A.INCLUSION_DATE, A.DTL_APPROVAL_DATE FROM PRODUCT_REGISTRATION_INFO A INNER JOIN (SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO ");
            query.Append(" )PR ON R.ID=PR.RECIPE_ID INNER JOIN (SELECT A.ANNEX_ID,A.PRICE_PER_UNIT,A.PROPOSED_BY, A.LAUNCHING_DATE FROM PRODUCT_PRICE A INNER JOIN (SELECT ANNEX_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_PRICE GROUP BY ANNEX_ID) B ON B.ANNEX_ID=A.ANNEX_ID AND B.MaxRvNo=A.REVISION_NO)PP ");
            query.Append(" ON PP.ANNEX_ID=PR.ANNEX_ID LEFT JOIN (SELECT A.MARKET_AUTHORIZATION_NO,A.ID, A.COMPANY_CODE,A.PRODUCT_CODE,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.VALID_UPTO FROM MARKET_AUTH_CERTIFICATE A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo ");
            query.Append(" FROM MARKET_AUTH_CERTIFICATE GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1 ");
            //if (!string.IsNullOrEmpty(model.CompanyCode))
            //{
            //    query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
            //}

            //if (model.ChooseOption != "All")
            //{
            //    if (model.ChooseOption == "SubmissionDate")
            //    {
            //        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //        {
            //            query.Append(" AND A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //        {
            //            query.Append(" AND A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        }
            //    }
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //    {
            //        query.Append(" AND ( A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        query.Append(" OR A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
            //    }
            //}

            query.Append("  )MA ON MA.COMPANY_CODE=R.COMPANY_CODE AND MA.PRODUCT_CODE=R.PRODUCT_CODE INNER JOIN PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE INNER JOIN COMPANY_INFO C ON C.COMPANY_CODE=P.COMPANY_CODE INNER JOIN DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            query.Append(" WHERE 1=1 ");
           
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND P.COMPANY_CODE ='" + model.CompanyCode + "'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME) ='" + model.BrandName.Trim() + "'");
            }
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), string.Format(query.ToString()));
            return dt;
        }

        public DataTable GetProductSummaryReport(ReportModel model)
        {
            var query = new StringBuilder();
            query.Append(" SELECT * FROM VW_RPT_PRODUCT_SUMMARY P");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND P.COMPANY_CODE ='" + model.CompanyCode + "'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME) ='" + model.BrandName.Trim() + "'");
            }
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), string.Format(query.ToString()));
            return dt;
        }

        public List<ProductVariantBEL> GetVariantsAndPacks(string productCode)
        {
            string vQry =
                "SELECT VARIANT_CODE, PRODUCT_CODE, VARIANT_NAME, STATUS, REMARKS " +
                "FROM PRODUCT_VARIANT_INFO " +
                "WHERE PRODUCT_CODE = '" + EscapeSql(productCode) + "' " +
                "ORDER BY VARIANT_CODE";

            DataTable variantDt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), vQry);
            List<ProductVariantBEL> variants = new List<ProductVariantBEL>();

            foreach (DataRow vRow in variantDt.Rows)
            {
                string vCode = vRow["VARIANT_CODE"].ToString();

                string pQry =
                    "SELECT PACK_CODE, VARIANT_CODE, PRODUCT_CODE, " +
                    "SAP_PRODUCT_CODE, PACK_SIZE_NAME, STATUS, REMARKS " +
                    "FROM PRODUCT_PACK_INFO " +
                    "WHERE VARIANT_CODE = '" + EscapeSql(vCode) + "' " +
                    "ORDER BY PACK_CODE";

                DataTable packDt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), pQry);

                List<ProductPackBEL> packs = new List<ProductPackBEL>();
                foreach (DataRow pRow in packDt.Rows)
                {
                    packs.Add(new ProductPackBEL
                    {
                        PackCode = pRow["PACK_CODE"].ToString(),
                        VariantCode = pRow["VARIANT_CODE"].ToString(),
                        ProductCode = pRow["PRODUCT_CODE"].ToString(),
                        SAPProductCode = pRow["SAP_PRODUCT_CODE"].ToString(),
                        PackSizeName = pRow["PACK_SIZE_NAME"].ToString(),
                        Status = pRow["STATUS"].ToString(),
                        Remarks = pRow["REMARKS"].ToString()
                    });
                }

                variants.Add(new ProductVariantBEL
                {
                    VariantCode = vCode,
                    ProductCode = vRow["PRODUCT_CODE"].ToString(),
                    VariantName = vRow["VARIANT_NAME"].ToString(),
                    Status = vRow["STATUS"].ToString(),
                    Remarks = vRow["REMARKS"].ToString(),
                    Packs = packs
                });
            }
            return variants;
        }
    }
}