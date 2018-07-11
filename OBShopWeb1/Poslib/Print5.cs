using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Data;
using iTextSharp.text;
using System.Text;
using iTextSharp.tool.xml;
using System.Net;
using OBShopWeb.Poslib;
using BarcodeLib;
using System.Collections;
using POS_Library.Public;
using POS_Library.ShopPos.DataModel;


namespace OBShopWeb.Poslib
{

    public class Print5
    {

        private BaseFont bfMs = BaseFont.CreateFont(@"C:\Windows\Fonts\msjh.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private BaseFont bfTimes = BaseFont.CreateFont(@"C:\Windows\Fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        public enum PdfAlignType
        {
            Left = Element.ALIGN_LEFT,
            Right = Element.ALIGN_RIGHT,
            Center = Element.ALIGN_CENTER,
            Middle = Element.ALIGN_MIDDLE
        }

        //關閉PDF
        public void ClosePdf(Document oDocument)
        {
            if (oDocument.IsOpen()) oDocument.Close();
        }


        public void PrintPickList(List<TicketShelfTemp> TSList, string InvoiceNo)
        {
            Document doc = new Document(PageSize.A8, 0, -10, 0, 0); //橫式 Rotate()
            MemoryStream ms = new MemoryStream();
            string Filepath = HttpContext.Current.Server.MapPath(".");
            string FileName = String.Format("{0}PickList_{1}_{2}.pdf", Filepath, DateTime.Now.ToString("yyyyMMdd"), InvoiceNo);
            PdfWriter.GetInstance(doc, new FileStream(FileName, FileMode.Create));

            //輸出Table
            PdfPTable table = new PdfPTable(new float[] { 3.5f, 0.1f, 4, 0.1f, 0.5f, 1 }); //new float[] { 2, 1, 1, 3 }
            table.TotalWidth = 145f;
            table.LockedWidth = true;

            if (!string.IsNullOrEmpty(InvoiceNo))
            {
                PdfPCell Invoice = new PdfPCell(new Phrase(InvoiceNo.Substring(InvoiceNo.Length - 4, 4), new Font(bfMs, 20f)));
                Invoice.Colspan = 6;
                Invoice.BorderColor = new BaseColor(255, 255, 255);
                Invoice.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
                table.AddCell(Invoice);
            }
            
            PdfPCell logo = new PdfPCell(new Phrase("OB嚴選 LOGO", new Font(bfMs, 10f)));
            logo.Colspan = 6;
            logo.BorderColor = new BaseColor(255, 255, 255);
            logo.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
            logo.BackgroundColor = new BaseColor(255, 255, 255);
            logo.Padding = 7f;
            table.AddCell(logo);

            PdfPCell header = new PdfPCell(new Phrase("交易明細", new Font(bfMs, 10f, Font.BOLD)));
            header.Colspan = 6;
            header.BorderColor = new BaseColor(255, 255, 255);
            header.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
            table.AddCell(header);

            PdfPCell hr = new PdfPCell(new Phrase("- - - - - - - - - - - - - - - - - - - - - - - - - - - -", new Font(bfMs, 7f)));
            hr.Colspan = 6;
            hr.BorderColor = new BaseColor(255, 255, 255);
            table.AddCell(hr);

            PdfPCell title = new PdfPCell(new Phrase("檢貨單", new Font(bfMs, 9.3f, Font.BOLD)));
            title.Colspan = 6;
            title.BorderColor = new BaseColor(255, 255, 255);
            title.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Center);
            table.AddCell(title);

            foreach (TicketShelfTemp TS in TSList)
            {
                ArrayList TrList = new ArrayList();
                TrList.Add(TS.Division);
                TrList.Add("");
                TrList.Add(TS.ProductId);
                TrList.Add("");
                TrList.Add("X");
                TrList.Add(TS.Quantity);
                CreateNewCell(table, TrList, PdfAlignType.Left);
            }

            PdfPCell colspace = new PdfPCell(new Phrase(" ", new Font(bfMs, 8f)));
            colspace.Colspan = 6;
            colspace.BorderColor = new BaseColor(255, 255, 255);
            table.AddCell(colspace);

            table.AddCell(hr);

            PdfPCell Time = new PdfPCell(new Phrase("交易時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), new Font(bfMs, 9.3f)));
            Time.Colspan = 6;
            Time.BorderColor = new BaseColor(255, 255, 255);
            Time.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Left);
            table.AddCell(Time);


            PdfPCell Num = new PdfPCell(new Phrase("交易序號:" + TSList[0].ShipId, new Font(bfMs, 9.3f)));
            Num.Colspan = 6;
            Num.BorderColor = new BaseColor(255, 255, 255);
            Num.HorizontalAlignment = Convert.ToInt32(PdfAlignType.Left);
            table.AddCell(Num);


            BarcodeLib.BarcodeModel bar = new BarcodeLib.BarcodeModel();
            var barImage = bar.GetBarcode("Code 39", TSList[0].Barcode, 600, 150, true, false);
            byte[] img = Utility.ImageToBuffer(barImage, System.Drawing.Imaging.ImageFormat.Png);


            Image jpg = Image.GetInstance(img);
            jpg.ScaleToFit(150f, 50f);


            doc.Open();
            doc.Add(jpg);
            doc.Add(table);
            ClosePdf(doc);

            //System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(FileName);
            //psi.Verb = "PRINT";
            //Process.Start(psi);
        }
        protected void CreateNewCell(PdfPTable table, ArrayList cellData, PdfAlignType align)
        {
            iTextSharp.text.Font oFont = new Font(bfMs, 9.3f, Font.BOLD);
            foreach (var data in cellData)
            {
                PdfPCell oCell = new PdfPCell(new Phrase(data.ToString(), oFont));
                oCell.HorizontalAlignment = Convert.ToInt32(align);
                oCell.BorderColor = new BaseColor(255, 255, 255);
                table.AddCell(oCell);
            }
        }
    }
}