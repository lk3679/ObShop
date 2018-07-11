using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using iTextSharp.tool.xml;
using System.Net;
using OBShopWeb.Poslib;
using BarcodeLib;
using System.Collections;
using POS_Library.Public;
using POS_Library.ShopPos.DataModel;
using System.Threading;
using System.Web.Configuration;

namespace OBShopWeb.Poslib
{
    public class Print
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

        public void PrintPickDetail(List<TicketShelfTemp> TSList, string InvoiceNo, string TicketName)
        {
            string Date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            Rectangle pageSize = new Rectangle(240, (145 + TSList.Count * 13)); //227, (145 + TSList.Count*13)
            Document doc = new Document(pageSize, -20, -10, 0, 0); //橫式 Rotate()
            MemoryStream ms = new MemoryStream();
            string Filepath = HttpContext.Current.Server.MapPath(".") + "\\PDF_Pick" + _areaId;
            //多筆加結尾分頁數(2015-0205新增)
            var 結尾 = (string.IsNullOrEmpty(InvoiceNo) 
                && !string.IsNullOrEmpty(TicketName) && TicketName.Length >= 2) ? "_" + TicketName.Substring(TicketName.Length - 2, 2) : "";

            string FileName = String.Format(@"{0}\PickList_{1}.pdf", Filepath, Date + 結尾);
            string WebUrl = String.Format(@"{0}\PickList_{1}.pdf", "\\PDF_Pick" + _areaId, Date + 結尾);

            //測試模式修改路徑
            if (testmode)
            {
                Filepath = HttpContext.Current.Server.MapPath(".") + "\\PDF_Pick" + _areaId + "_Test";
                FileName = String.Format(@"{0}\PickList_{1}.pdf", Filepath, Date + 結尾);
                WebUrl = String.Format(@"{0}\PickList_{1}.pdf", "\\PDF_Pick" + _areaId + "_Test", Date + 結尾);
            }

            //不存在則建立資料夾
            if (!Directory.Exists(Filepath))
                Directory.CreateDirectory(Filepath);

            Exception ExceptionToThrow = null;

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(FileName, FileMode.Create));
                PdfPTable table = new PdfPTable(new float[] { 1.5f, 0.1f, 3, 0.1f, 0.3f, 1 });
                table = PickContent(table, TSList, TicketName);
                doc.Open();
                doc.Add(table);
            }
            catch (Exception e)
            {
                ExceptionToThrow = e;
            }
            finally
            {
                ClosePdf(doc);
                doc.Close();
                //ExcutePrint(WebUrl);
            }

            if (ExceptionToThrow != null)
            { throw ExceptionToThrow; }

        }

        public string PrintPickList(List<TicketShelfTemp> TSList, string InvoiceNo, string TicketName)
        {
            string PrintLogFileName="";
            string Date = DateTime.Now.ToString("yyyyMMdd");
            Rectangle pageSize = new Rectangle(240, (145 + TSList.Count * 13)); //227, (145 + TSList.Count*13)            
            Document doc = new Document(pageSize, -20, -10, 0, 0); //橫式 Rotate()
            MemoryStream ms = new MemoryStream();
            string Filepath = HttpContext.Current.Server.MapPath(".") + "\\PDF_Pick" + _areaId;
            //多筆加結尾分頁數 調出用(2015-0205新增)
            var 結尾 = (string.IsNullOrEmpty(InvoiceNo) 
                && !string.IsNullOrEmpty(TicketName) && TicketName.Length >= 2) ? "_" + TicketName.Substring(TicketName.Length - 2, 2) : "";

            //FILENAME 是真正的檔案位置，web用來讀檔案用
            string FileName = String.Format(@"{0}\PickList_{1}_{2}.pdf", Filepath, Date, TSList[0].Barcode + 結尾);
            string WebUrl = String.Format(@"{0}\PickList_{1}_{2}.pdf", "\\PDF_Pick" + _areaId, Date, TSList[0].Barcode + 結尾);

             PrintLogFileName= String.Format("PickList_{0}_{1}.pdf", Date,TSList[0].Barcode + 結尾);

            //測試模式修改路徑
            if (testmode)
            {
                Filepath = HttpContext.Current.Server.MapPath(".") + "\\PDF_Pick" + _areaId + "_Test";
                FileName = String.Format(@"{0}\PickList_{1}_{2}.pdf", Filepath, Date, TSList[0].Barcode + 結尾);
                WebUrl = String.Format(@"{0}\PickList_{1}_{2}.pdf", "\\PDF_Pick" + _areaId + "_Test", Date, TSList[0].Barcode + 結尾);
            }
            //不存在則建立資料夾
            if (!Directory.Exists(Filepath))
                Directory.CreateDirectory(Filepath);

            Exception ExceptionToThrow = null;

            try
            {

                PdfWriter.GetInstance(doc, new FileStream(FileName, FileMode.Create));
                PdfPTable table = new PdfPTable(new float[] { 1.5f, 0.1f, 3, 0.1f, 0.3f, 1 }); //new float[] { 3.5f, 0.005f, 4, 0.1f, 0.005f, 1 }
                //table.TotalWidth = 220f;
                //table.LockedWidth = true;

                BarcodeLib.BarcodeModel bar = new BarcodeLib.BarcodeModel();
                var barImage = bar.GetBarcode("Code 39", TSList[0].Barcode, 600, 150, true, false);
                byte[] img = Utility.ImageToBuffer(barImage, System.Drawing.Imaging.ImageFormat.Png);

                Image jpg = Image.GetInstance(img);
                jpg.ScaleToFit(220f, 50f);

                PdfPCell cell = new PdfPCell(jpg);
                cell.Colspan = 6;
                cell.BorderColor = new BaseColor(255, 255, 255);
                cell.VerticalAlignment = Convert.ToInt32(PdfAlignType.Center);
                cell.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Left);

                table.AddCell(cell);

                if (!string.IsNullOrEmpty(InvoiceNo))
                {
                    PdfPCell Invoice = new PdfPCell(new Phrase(InvoiceNo.Substring(InvoiceNo.Length - 4, 4), new Font(bfMs, 25f)));
                    Invoice.Colspan = 6;
                    Invoice.BorderColor = new BaseColor(255, 255, 255);
                    Invoice.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
                    table.AddCell(Invoice);
                }

                table = PickContent(table, TSList, TicketName);

                PdfPCell Num = new PdfPCell(new Phrase("交易序號:" + TSList[0].ShipId, new Font(bfMs, 10f)));
                Num.Colspan = 6;
                Num.BorderColor = new BaseColor(255, 255, 255);
                Num.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Left);
                table.AddCell(Num);

                doc.Open();
                doc.Add(table);


            }
            catch (Exception e)
            {
                ExceptionToThrow = e;
            }
            finally
            {
                ClosePdf(doc);
                doc.Close();
            }

            if (ExceptionToThrow != null)
            { throw ExceptionToThrow; }

            if (TicketName.IndexOf("補印") == -1)
                InsertPrintListLog(TSList[0].ShipId, PrintLogFileName, (int)PdfPrintType.結帳自動列印);
            else
                InsertPrintListLog(TSList[0].ShipId, PrintLogFileName, (int)PdfPrintType.手動補印);

            return "Success";

        }

        public PdfPTable PickContent(PdfPTable table,List<TicketShelfTemp> TSList, string TicketName)
        {
            string Filepath = HttpContext.Current.Server.MapPath(".")+@"\Image\";
            Image OBLogo = Image.GetInstance(Filepath + "OBLogo.jpg");
            OBLogo.ScalePercent(50, 50);
            PdfPCell cell = new PdfPCell(OBLogo);
            cell.Colspan = 6;
            cell.BorderColor = new BaseColor(255, 255, 255);
            cell.VerticalAlignment = Convert.ToInt32(PdfAlignType.Center);
            cell.HorizontalAlignment= Convert.ToInt32(PdfAlignType.Center);

            table.AddCell(cell);

            PdfPCell header = new PdfPCell(new Phrase(TicketName, new Font(bfMs, 14f)));
            header.Colspan = 6;
            header.BorderColor = new BaseColor(255, 255, 255);
            header.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
            table.AddCell(header);

            PdfPCell hr = new PdfPCell(new Phrase("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", new Font(bfMs, 8f)));

            hr.Colspan = 6;
            hr.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
            hr.BorderColor = new BaseColor(255, 255, 255);
            table.AddCell(hr);

            PdfPCell title = new PdfPCell(new Phrase("揀貨單", new Font(bfMs, 12f)));
            title.Colspan = 6;
            title.BorderColor = new BaseColor(255, 255, 255);
            title.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
            table.AddCell(title);

           
            foreach (TicketShelfTemp TS in TSList)
            {
                ArrayList TrList = new ArrayList();
                TrList.Add((TS.Division == "Z99010105" ? "展售儲位" : TS.Division));
                TrList.Add("");

                if (string.IsNullOrEmpty(TS.ProductColor))
                    TrList.Add(TS.ProductId);
                else
                    TrList.Add(TS.ProductId + "(" + TS.ProductColor + ")");
                    //TrList.Add(TS.ProductId + "(深灰/藍色)");
                    
                TrList.Add("");
                TrList.Add("X");
                TrList.Add(TS.Quantity);
                CreateNewCell(table, TrList, PdfAlignType.Left);
            }

            //結尾加小計
            int TotalQuantitySum = TSList.Sum(x => x.Quantity);
            ArrayList TrListBottom = new ArrayList();
            TrListBottom.Add("小計件數：");
            TrListBottom.Add("");
            TrListBottom.Add("");
            TrListBottom.Add("");
            TrListBottom.Add("");
            TrListBottom.Add(TotalQuantitySum);
            CreateNewCell(table, TrListBottom, PdfAlignType.Left);

            PdfPCell colspace = new PdfPCell(new Phrase(" ", new Font(bfMs, 9f)));
            colspace.Colspan = 6;
            colspace.BorderColor = new BaseColor(255, 255, 255);
            table.AddCell(colspace);

            table.AddCell(hr);

            PdfPCell Time = new PdfPCell(new Phrase("印單時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), new Font(bfMs, 10f)));
            Time.Colspan = 6;
            Time.BorderColor = new BaseColor(255, 255, 255);
            Time.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Left);
            table.AddCell(Time);

            return table;
        }

        protected void CreateNewCell(PdfPTable table, ArrayList cellData, PdfAlignType align)
        {
            iTextSharp.text.Font oFont = new Font(bfMs, 9f, Font.BOLD);
            int counter = 0;
           
            foreach (var data in cellData)
            {
                string Value = data.ToString();
                if (counter == 5)
                {
                    if (int.Parse(Value) < 10)
                    {
                        Value = string.Format("  {0}", data.ToString());
                    }
                }

                PdfPCell oCell = new PdfPCell(new Phrase(Value, oFont));
                oCell.HorizontalAlignment = Convert.ToInt32(align);
                oCell.BorderColor = new BaseColor(255, 255, 255);
                table.AddCell(oCell);
                counter++;
            }
        }

        public string ExcutePrint(string FileName)
        {
            try
            {
                string IPaddress = HttpContext.Current.Request.UserHostAddress;
                string PrintMachineNo = PosNumber.GetPrintMachineNo(IPaddress);
                string MachineName = HttpContext.Current.Server.MachineName;
                string FilePath = string.Format(@"\\{0}{1}", MachineName, FileName);
                PosPrintBarcode.PrintClient pc = new PosPrintBarcode.PrintClient();
                var aa = pc.PrintPick(PrintMachineNo, FilePath);
                //測試(2015-0205新增)
                //var aa = pc.PrintSingle("XX", "barcodeNum=00006164151,txtLogisticsId=Tester");
                return aa.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }

        public bool InsertPrintListLog(string OrderID,string FileName,int Type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Insert into [PosClient].[dbo].[PrintListLog](OrderID,FileName,Type,CreateTime) ");
            sb.AppendLine("values(@OrderID,@FileName,@Type,GETDATE()) ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            param.Add("FileName", FileName);
            param.Add("Type", Type);
            string sql = sb.ToString();
            return DB.DBNonQuery(sql, param, "PosClient");
        }
    }
}