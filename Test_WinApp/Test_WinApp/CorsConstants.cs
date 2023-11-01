﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_WinApp
{
    internal class CorsConstants
    {
        internal const string Origin = "Origin";

        internal const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";

        internal const string AccessControlRequestMethod = "Access-Control-Request-Method";

        internal const string AccessControlRequestHeaders = "Access-Control-Request-Headers";

        internal const string AccessControlAllowMethods = "Access-Control-Allow-Methods";

        internal const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        internal const string PreflightSuffix = "_preflight_";

        public CorsConstants()
        {
        }
    }
}
