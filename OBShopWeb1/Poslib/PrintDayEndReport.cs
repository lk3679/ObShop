using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace OBShopWeb.Poslib
{
    public class PrintDayEndReport
    {
        private BaseFont bfMs = BaseFont.CreateFont(@"C:\Windows\Fonts\msjh.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private BaseFont bfTimes = BaseFont.CreateFont(@"C:\Windows\Fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        //儲位所在地
        static private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        private bool testmode = false;

        public enum PdfAlignType
        {
            Left = Element.ALIGN_LEFT,
            Right = Element.ALIGN_RIGHT,
            Center = Element.ALIGN_CENTER,
            Middle = Element.ALIGN_MIDDLE
        }

        public void ClosePdf(Document oDocument)
        {
            if (oDocument.IsOpen())
                oDocument.Close();
        }

        public string PrintReport(DayEndReport deReport)
        {
            string Date = DateTime.Now.ToString("yyyyMMdd");
            Rectangle pageSize = new Rectangle(240, (400 +(deReport.FailCardInvoiceList.Count+deReport.FailCashInvoiceList.Count) * 13)); //227, (145 + TSList.Count*13)            
            Document doc = new Document(pageSize, -20, -10, 0, 0); 
            MemoryStream ms = new MemoryStream();
            string Filepath = HttpContext.Current.Server.MapPath(".") + "\\PDF_Pick" + _areaId;

            string FileName = String.Format(@"{0}\DayEndReport_{1}_POS{2}.pdf", Filepath, Date, deReport.PosNo);
            string WebUrl = String.Format(@"{0}\DayEndReport_{1}_POS{2}.pdf", "\\PDF_Pick" + _areaId, Date, deReport.PosNo);

            if (testmode)
            {
                Filepath = HttpContext.Current.Server.MapPath(".") + "\\PDF_Pick" + _areaId + "_Test";
                FileName = String.Format(@"{0}\DayEndReport_{1}_POS{2}.pdf", Filepath, Date, deReport.PosNo);
                WebUrl = String.Format(@"{0}\DayEndReport_{1}_POS{2}.pdf", "\\PDF_Pick" + _areaId + "_Test", Date, deReport.PosNo);
            }
            //不存在則建立資料夾
            if (!Directory.Exists(Filepath))
                Directory.CreateDirectory(Filepath);

              Exception ExceptionToThrow = null;

              try
              {

                  PdfWriter.GetInstance(doc, new FileStream(FileName, FileMode.Create));
                  PdfPTable table = new PdfPTable(new float[] { 1f });

                  table.AddCell(NewCell("交易日期  :", deReport.Date));
                  table.AddCell(NewCell("店號:" + deReport.StoreNo, "                                 機號: " + deReport.PosNo));

                  PdfPCell hr = new PdfPCell(new Phrase("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", new Font(bfMs, 8f)));
                  hr.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Left);
                  hr.BorderColor = new BaseColor(255, 255, 255);
                  table.AddCell(hr);

                  table.AddCell(NewCell("收銀員名稱   ：  ", deReport.ClerkName));
                  table.AddCell(NewCell(" ", " "));
                  table.AddCell(NewCell("發票銷售額   ：  ", deReport.TotalOrderAmount));
                  table.AddCell(NewCell("應稅銷售額   ：  ", 0));
                  table.AddCell(NewCell("免稅銷售額   ：  ", 0));
                  table.AddCell(NewCell("總稅額   ：  ", 0));
                  table.AddCell(NewCell(" ", " "));
                  table.AddCell(NewCell("開始交易序號：  ", deReport.StartOrderID));
                  table.AddCell(NewCell("結束交易序號：  ", deReport.EndOrderID));
                  table.AddCell(NewCell("現金筆數     ： ", deReport.TotalCashOrderNum));
                  table.AddCell(NewCell("現金收入     ： ", deReport.CashIncome));
                  table.AddCell(NewCell("現金折讓     ： ", deReport.AllowancesCashAmount));
                  table.AddCell(NewCell("信用卡筆數 ： ", deReport.TotalCardOrderNum));
                  table.AddCell(NewCell("信用卡收入 ： ", deReport.CardIncome));
                  table.AddCell(NewCell("信用卡折讓 ： ", deReport.AllowancesCardAmount));
                  table.AddCell(NewCell("實收            ： ", (deReport.CashIncome + deReport.CardIncome)- deReport.ReturnTotalAmount));
                  table.AddCell(NewCell(" ", " "));
                  foreach (InvoiceRoll IR in deReport.InvoiceList)
                  {
                      table.AddCell(NewCell("開始交易序號 : ", IR.StartInvoice));
                      table.AddCell(NewCell("結束交易序號 : ", IR.EndInvoice));
                      table.AddCell(NewCell("現金    ： ", IR.Cash));
                      table.AddCell(NewCell("信用卡： ", IR.Credit));
                      table.AddCell(NewCell("小計    ： ", (IR.Cash+IR.Credit)));
                  }

                  table.AddCell(NewCell(" ", " "));

                  table.AddCell(NewCell("現金作廢 ", " "));
                  table.AddCell(NewCell("發票號碼              ", "               金額"));
                  foreach (Order.FailInvoice inv in deReport.FailCashInvoiceList)
                  {
                      table.AddCell(NewCell(inv.InvoiceNo + "      ", "                " + inv.InvoiceNoAmount));
                  }

                  table.AddCell(NewCell(" ", " "));
                  table.AddCell(NewCell("現金作廢金額     ： ", deReport.ReturnTotalCashAmount));
                  table.AddCell(NewCell(" ", " "));

                  table.AddCell(NewCell("現金折讓", " "));
                  table.AddCell(NewCell("發票號碼              ", "               金額"));
                  foreach (Order.FailInvoice inv in deReport.AllowancesCashList)
                  {
                      table.AddCell(NewCell(inv.InvoiceNo + "      ", "                " + inv.InvoiceNoAmount));
                  }

                  table.AddCell(NewCell(" ", " "));

                  table.AddCell(NewCell("現金折讓", " "));
                  table.AddCell(NewCell("發票號碼              ", "               金額"));
                  foreach (Order.FailInvoice inv in deReport.AllowancesCashList)
                  {
                      table.AddCell(NewCell(inv.InvoiceNo + "      ", "                " + inv.InvoiceNoAmount));
                  }

                  table.AddCell(NewCell(" ", " "));

                  table.AddCell(NewCell("信用卡作廢 ", " "));
                  table.AddCell(NewCell("發票號碼              ", "               金額"));
                  foreach (Order.FailInvoice inv in deReport.FailCardInvoiceList)
                  {
                      table.AddCell(NewCell(inv.InvoiceNo + "      ", "                " + inv.InvoiceNoAmount));
                  }
                  table.AddCell(NewCell(" ", " "));
                  table.AddCell(NewCell("信用卡作廢金額   ：", deReport.ReturnTotalCardAmount));
                  table.AddCell(NewCell(" ", " "));

                  table.AddCell(NewCell("信用卡折讓", " "));
                  table.AddCell(NewCell("發票號碼              ", "               金額"));
                  foreach (Order.FailInvoice inv in deReport.AllowancesCardList)
                  {
                      table.AddCell(NewCell(inv.InvoiceNo + "      ", "                " + inv.InvoiceNoAmount));
                  }

                  table.AddCell(NewCell(" ", " "));

                  table.AddCell(NewCell("退貨總金額   ： ", deReport.ReturnTotalAmount));
                  table.AddCell(NewCell(" ", " "));
                  table.AddCell(NewCell("其他作廢發票 ", " "));
                  table.AddCell(NewCell("發票號碼              ", "               金額"));
                  foreach (Order.FailInvoice inv in deReport.FailOhterInvoiceList)
                  {
                      table.AddCell(NewCell(inv.InvoiceNo + "      ", "                " + inv.InvoiceNoAmount));
                  }
                  table.AddCell(NewCell(" ", " "));
                  table.AddCell(NewCell("發票更正總筆數,張數:", deReport.FailOrderNum.ToString() + "," + deReport.FailInvoiceNum.ToString()));
                  doc.Open();
                  doc.Add(table);
              }
              catch (Exception e)
              {
                  ExceptionToThrow = e;
              }finally{
                  ClosePdf(doc);
                  doc.Close();
              }

              if (ExceptionToThrow != null)
              { throw ExceptionToThrow; }

              return "Success";

        }

        public PdfPCell NewCell(string Name,object Value)
        {
            PdfPCell cell = new PdfPCell(new Phrase(Name + Value.ToString(), new Font(bfMs, 10f)));
            cell.BorderColor = new BaseColor(255, 255, 255);
            return cell;
        }
    }

    public class InvoiceRoll
    {
        public string StartInvoice;
        public string EndInvoice;
        public int Cash;
        public int Credit;
    }

    public class DayEndReport
    {
        public string Date = "";
        public string StoreNo = "";
        public string PosNo = "";
        public string StartOrderID = "";
        public string EndOrderID = "";
        public List<InvoiceRoll> InvoiceList = new List<InvoiceRoll>();
        public int TotalOrderNum = 0;
        public int TotalOrderAmount = 0;
        public int TotalQuantity = 0;
        public int TotalCashOrderNum = 0;
        public int TotalCardOrderNum = 0;
        public int CashIncome = 0;
        public int CardIncome = 0;
        public int ReturnTotalAmount = 0;
        public int ReturnTotalCashAmount = 0;
        public int ReturnTotalCardAmount = 0;
        public string ClerkName="";
        public List<Order.FailInvoice> FailCashInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> FailCardInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> FailOhterInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> AllowancesCashList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> AllowancesCardList = new List<Order.FailInvoice>();
        public int FailInvoiceNum = 0;
        public int FailOrderNum = 0;
        public int AllowancesCashAmount = 0;
        public int AllowancesCardAmount = 0;
    }
}