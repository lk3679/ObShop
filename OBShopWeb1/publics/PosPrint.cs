using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBShopWeb.publics
{
    public class PosPrint :  POS_Library.Public.BarcodePrint.IPrint
    {
        public bool PrintBarcode(string printId, List<string> msgs)
        {
            PosPrintBarcode.PrintClient pc = new PosPrintBarcode.PrintClient();
            return pc.Print(printId, msgs );
            
        }
    }
}