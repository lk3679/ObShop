using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace OBShopWeb.Poslib
{
    public class SalesTax
    {

        public string GetTaxID()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("InvoiceInfo.xml"));
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/shop/store");
            string busNo = "";
            string TaxID = "38646825";
            if (nodes.Count > 0)
            {
                busNo = nodes[0].SelectSingleNode("busNo").InnerText.Trim();
            }

            TaxID = busNo.Replace("NO:", "");
            return TaxID;
        }

        private void _exportInvoiceToCSV(DataTable dt, string filename, bool IsAllowenceInvoice)
        {
            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms, Encoding.GetEncoding("UTF-8"));

            var order = dt.AsEnumerable().Select(r => new
            {
                RowNo = r["RowNo"],
                OrderID = r["OrderID"],
                StartInvoice = r["起始號碼"],
                EndInvoice = r["結束號碼"],
                TrDate = r["開立年月"],
                UpdateDate = r["作廢異動日期"],
                TrAmount = (int)r["發票金額"],
                InvoiceCancel = (r["是否作廢"].ToString() == "Y") ? "F" : "1",
                Status = (int)r["Status"]
            }).Where(x => (Convert.ToInt32(x.RowNo) == 1 || x.TrAmount == 0)).ToList();

            if (IsAllowenceInvoice)
            {
                order = (from r in order
                         where r.Status == 6
                         select r).ToList();
            }
            else
            {
                order = (from r in order
                         where r.Status != 6
                         select r).ToList();
            }

            tw.WriteLine("訂單編號,開立年月,發票起,發票迄,發票金額,作廢異動日期,是否作廢");
            foreach (var q in order)
            {
                tw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",q.OrderID, q.TrDate, q.StartInvoice, q.EndInvoice, q.TrAmount.ToString("0"), q.UpdateDate, q.InvoiceCancel));
            }
                
            
            //Download
            tw.Flush();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/force-download";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();
            HttpContext.Current.Response.End();
        }

        private void _exportInvoiceToTxt(DataTable dt,string filename,bool IsAllowenceInvoice)
        {
            string TaxID = GetTaxID();

            int index = 1;
            var order=dt.AsEnumerable().Select(r => new {
                Type="32",
                ID = "142822094",
                TaxID = TaxID,
                RowNo = r["RowNo"],
                OrderID = r["OrderID"],
                Invoice1 = r["InvoiceNo"],
                TrDate = r["開立年月民國年"],
                TrAmount = (int)r["發票金額"],
                InvoiceCancel = (r["是否作廢"].ToString() == "Y") ? "F" : "1",
                Tax = "0000000000",
                Status=(int)r["Status"]
            }).Where(x => (Convert.ToInt32(x.RowNo) == 1 || x.TrAmount == 0)).ToList();

            if (IsAllowenceInvoice)
            {
                order = (from r in order
                         where r.Status == 6
                         select r).ToList();
            }
            else
            {
                order = (from r in order
                         where r.Status != 6
                         select r).ToList();
            }
               
            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);

            foreach (var q in order)
            {
                string row = q.Type + q.ID + String.Format("{0:0000000}", index) + q.TrDate + "        " + q.TaxID + q.Invoice1;
                row += q.InvoiceCancel =="1"?String.Format("{0:000000000000}", q.TrAmount):String.Format("{0:000000000000}", 0);
                row += q.InvoiceCancel + "0000000000" + "         ";

                tw.WriteLine(row);
                index++;
            }

            tw.Flush();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/force-download";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();
            HttpContext.Current.Response.End();

        }

        public void ExportSalesInvoiceTxtFile(string sdt, string edt, string cancelType, string updStartdt = "", string updEnddt = "")
        {
            string TaxID = GetTaxID();
            string filename = string.Format("{0}_32.txt", TaxID);
            DataTable dt = SalesTaxQuery.QuerySalesInvoice(sdt, edt, cancelType, updStartdt, updEnddt);

            _exportInvoiceToTxt(dt, filename,false);
        }

        public void ExportAllowenceInvoiceTxtFile(string sdt, string edt, string cancelType, string updStartdt = "", string updEnddt = "")
        {
            string TaxID = GetTaxID();
            string filename = string.Format("{0}_34.txt", TaxID);
            DataTable dt = SalesTaxQuery.QuerySalesInvoice(sdt, edt, cancelType, updStartdt, updEnddt);

            _exportInvoiceToTxt(dt, filename,true);
        }

        public void ExportSalesInvoice(string sdt, string edt, string cancelType, string updStartdt = "", string updEnddt = "")
        {
            string filename = String.Format("銷貨_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss"));
            DataTable dt = SalesTaxQuery.QuerySalesInvoice(sdt, edt, cancelType, updStartdt, updEnddt);
            _exportInvoiceToCSV(dt, filename,false);
        }

        public void ExportAllowencesInvoice(string sdt, string edt, string cancelType, string updStartdt = "", string updEnddt = "")
        {
            string filename = String.Format("{0}-{1}-34.csv", sdt.Replace("-", ""), edt.Replace("-", ""));
            DataTable dt = SalesTaxQuery.QuerySalesInvoice(sdt, edt, cancelType, updStartdt, updEnddt);
            _exportInvoiceToCSV(dt, filename, true);
        }

    }

    public class SalesTaxQuery{

       public static DataTable QuerySalesInvoice(string sdt, string edt, string cancelType,string cancelsdt="",string canceledt=""){
           StringBuilder sb = new StringBuilder();
           sb.AppendLine("select ROW_NUMBER() OVER(PARTITION BY o.OrderID Order BY oi.InvoiceNo) AS RowNo, ");
           sb.AppendLine("isnull(l.使用張數,1) UseNum,  ");
           sb.AppendLine("o.OrderID,IsNull(oi.InvoiceNo, ni.InvoiceNo) InvoiceNo, ");
           sb.AppendLine("o.OrderID,isnull(oi.InvoiceNo, ni.InvoiceNo) as 起始號碼, ");
           sb.AppendLine("isnull(SUBSTRING(oi.InvoiceNo,0,3)+CAST((SUBSTRING(oi.InvoiceNo,3,8)+l.使用張數-1) as varchar),ni.InvoiceNo) 結束號碼, ");
           sb.AppendLine("IsNull(right(cast(1000+ datepart(yy, o.OrderDate)-1911 as char(4)),3)+right(cast(100 + datepart(mm, o.OrderDate) as char(3)), 2),right(cast(1000+ datepart(yy, ni.NullifiedTime)-1911 as char(4)),3)+right(cast(100 + datepart(mm, ni.NullifiedTime) as char(3)), 2)) 開立年月民國年,  ");
           sb.AppendLine("format(o.OrderDate, 'yyyyMM') 開立年月, ");
           sb.AppendLine("IsNull(o.Amount,0) 發票金額, ");
           sb.AppendLine("iif(ni.NullifiedTime is null, '', format(ni.NullifiedTime, 'yyyy-MM-dd HH:mm:ss')) 作廢異動日期, ");
           sb.AppendLine("iif(ni.NullifiedTime is null,'N','Y') 是否作廢, ");
           sb.AppendLine("IsNull(o.Status,0) Status ");
           sb.AppendLine("from OrderInvoices as oi ");
           sb.AppendLine("left outer join Orders as o on o.OrderID = oi.OrderID ");
           sb.AppendLine("left join (select k.OrderID,count(k.InvoiceNo) 使用張數 from PosClient..OrderInvoices k group by k.OrderID) l on l.OrderID=o.OrderID ");
           sb.AppendLine("full outer join NullifiedInvoices as ni on ni.InvoiceNo = oi.InvoiceNo ");
           sb.AppendLine("where ");
           sb.AppendLine("o.OrderDate between @sdt and @edt ");

           switch (cancelType)
           {
               case "cancelwith":
                   sb.AppendLine("or (ni.NullifiedTime >=@sdt and ni.NullifiedTime < convert(datetime, @edt, 101)+1) ");
                   break;
               case "cancelwithout":
                   sb.AppendLine("and ni.NullifiedTime is null ");
                   break;
               case "cancelonly":
                   sb.AppendLine("and ni.NullifiedTime is not null ");
                   break;
               default:
                   sb.AppendLine("");
                   break;
           }

           if (cancelsdt.Length > 0 && canceledt.Length > 0)
           {
               sb.AppendLine("or (ni.NullifiedTime >= @cancelsdt and ni.NullifiedTime <convert(datetime, @canceledt, 101)+1 ) ");
           }

           Dictionary<string, object> param = new Dictionary<string, object>();
           param.Add("sdt", sdt);
           param.Add("edt", edt);
           param.Add("cancelsdt", cancelsdt);
           param.Add("canceledt",canceledt);
           DataTable dt = new DataTable();
           string sql = sb.ToString();
           dt = DB.DBQuery(sql, param, "PosClient");
           return dt;
       }
    
    }
}