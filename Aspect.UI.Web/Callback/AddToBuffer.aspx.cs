﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Callback
{
    public partial class AddToBuffer : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["pid"]))
            {
                this.AddProductToMultiBuffer(Request["pid"].Split(',').Select(i => new Guid(i)));
            }
        }

    }
}
