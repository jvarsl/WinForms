﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLaborai
{
    public static class Helper
    {
        public static string CnnVal()
        {
            return ConfigurationManager.ConnectionStrings["DB_blockbusteris"].ConnectionString;
        }
    }
}
