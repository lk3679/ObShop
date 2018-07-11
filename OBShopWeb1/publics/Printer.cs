using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBShopWeb.publics
{
    public class Printer :  POS_Library.Public.BarcodePrint.IPrint
    {
        public bool PrintBarcode(string printId, List<string> msgs)
        {
            OBPrintBarcode.PrintClient pc = new OBPrintBarcode.PrintClient();
            return pc.Print(printId, msgs );
            
        }
    }
}