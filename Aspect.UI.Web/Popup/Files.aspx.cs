using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;

namespace Aspect.UI.Web.Popup
{
    [ScriptService]
    public partial class Files : System.Web.UI.Page
    {

        [WebMethod]
        public static string Upload()
        {
            string result = String.Empty;
            string filepath = "C:\\Uploads";
            HttpFileCollection uploadedFiles = HttpContext.Current.Request.Files;

            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                HttpPostedFile userPostedFile = uploadedFiles[i];

                try
                {
                    if (userPostedFile.ContentLength > 0)
                    {
                        result += "<u>File #" + (i + 1) +
                           "</u><br>";
                        result += "File Content Type: " +
                           userPostedFile.ContentType + "<br>";
                        result += "File Size: " +
                           userPostedFile.ContentLength + "kb<br>";
                        result += "File Name: " +
                           userPostedFile.FileName + "<br>";

                        userPostedFile.SaveAs(filepath + "\\" +
                           System.IO.Path.GetFileName(userPostedFile.FileName));

                        result += "Location where saved: " +
                           filepath + "\\" +
                           System.IO.Path.GetFileName(userPostedFile.FileName) +
                           "<p>";
                    }
                }
                catch (Exception Ex)
                {
                    result += "Error: <br>" + Ex.Message;
                }
            }
            return result;
        }
    }
}