using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgilvyOne.Common
{
    public class OOErrorHandler
    {
        public static void WriteElmaLog(Exception ex)
        {
            ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
}
