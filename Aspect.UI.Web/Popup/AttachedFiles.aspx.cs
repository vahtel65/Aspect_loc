using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Aspect.Model.ProductDomain;
using Aspect.Domain;

namespace Aspect.UI.Web.Popup
{
    public partial class AttachedFiles : Basic.PageBase
    {
        public class FileRow
        {  
            public Guid ID { get; set; }
            public string UserName { get; set; }
            public string FileName { get; set; }          
        }

        private Guid _pid = Guid.Empty; 
        private Guid GetPID
        {
            get
            {
                if (_pid.Equals(Guid.Empty))
                {
                    using (ProductProvider provider = new ProductProvider())
                    {                        
                        var requestPID = new Guid(Request["pid"]);
                        var product = provider.Products.SingleOrDefault(it => it.ID == requestPID);

                        if (product == null)
                        {
                            // используется словарь, берём его
                            _pid = requestPID;
                        }
                        else
                        {
                            // используется продукт, берём номенклатуру
                            _pid = product._dictNomenID.Value;
                        }
                    }
                }
                return _pid;
            }
        }

        private List<FileRow> fileTables;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataBind();
            }
        }        

        protected void DataBind()
        {
            using (ProductProvider provider = new ProductProvider())
            {
                fileTables = new List<FileRow>();
                Guid pid = GetPID;
                var userFiles = provider.UserFiles.Where(file => file.pid == pid);

                foreach (var file in userFiles)
                {
                    fileTables.Add(new FileRow() 
                    { 
                        ID = file.id, 
                        UserName = file.username,
                        FileName = String.Format("../UserFiles/{0}", Server.UrlEncode(file.filename))
                    });
                }

                Repeater1.DataSource = fileTables;
                Repeater1.DataBind();
            }
        }

        protected void ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "com.delete")
            { 
                using (ProductProvider provider = new ProductProvider())
                {
                    var userFile = provider.UserFiles.SingleOrDefault(it => it.id == new Guid((string)e.CommandArgument));

                    if (userFile != null)
                    {
                        provider.UserFiles.DeleteOnSubmit(userFile);

                        String savePath = Path.Combine(Request.PhysicalApplicationPath, "UserFiles");
                        File.Delete(Path.Combine(savePath, userFile.filename));

                        provider.SubmitChanges();
                    }
                }
            }

            DataBind();
        }

        public static Control FindControlRecursive(Control Root, string Id)
        {
            if (Root.ID == Id)
                return Root;
            foreach (Control Ctl in Root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, Id);
                if (FoundCtl != null)
                    return FoundCtl;
            }
            return null;
        }

        protected void SavaFileNames(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                Repeater repeater = (Repeater)FindControlRecursive(this.Master, "Repeater1");
                
                foreach (var repeaterItem in repeater.Controls)
                {
                    var hidden = (repeaterItem as RepeaterItem).FindControl("hiddenID") as HiddenField;
                    var edit = (repeaterItem as RepeaterItem).FindControl("textUserName") as TextBox;

                    var userFile = provider.UserFiles.Single(it => it.id == new Guid(hidden.Value));
                    if (userFile != null && userFile.username != edit.Text)
                    {
                        userFile.username = edit.Text;
                    }
                }

                provider.SubmitChanges();
            }
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            // Specify the path on the server to
            // save the uploaded file to.
            String savePath = Path.Combine(Request.PhysicalApplicationPath, "UserFiles");

            // Before attempting to perform operations
            // on the file, verify that the FileUpload 
            // control contains a file.
            if (FileUpload1.HasFile)
            {
                // Get the name of the file to upload.
                String fileName = FileUpload1.FileName;
                String savedFileName = String.Empty;
                int postfix = 0;

                // Append the name of the file to upload to the path.

                do 
                {
                    
                    if (postfix == 0)
                    {
                        savedFileName = Path.Combine(new string[] {savePath, fileName});
                    }
                    else 
                    {
                        savedFileName = Path.Combine(new string[] {savePath, Path.GetFileNameWithoutExtension(fileName) + "_" + postfix + Path.GetExtension(fileName)});
                    }
                    postfix++;
                } while (File.Exists(savedFileName));
                                
                FileUpload1.SaveAs(savedFileName);

                using (ProductProvider provider = new ProductProvider())
                {
                    var userFile = new UserFile()
                    {
                        id = Guid.NewGuid(),
                        pid = GetPID,
                        cid = Guid.Empty,
                        filename = Path.GetFileName(savedFileName),
                        username = Path.GetFileNameWithoutExtension(fileName)
                    };

                    provider.UserFiles.InsertOnSubmit(userFile);
                    provider.SubmitChanges();
                }

                DataBind();
                
                // Notify the user of the name of the file
                // was saved under.
                // UploadStatusLabel.Text = "Your file was saved as " + fileName;
            }
            else
            {
                // Notify the user that a file was not uploaded.
                // UploadStatusLabel.Text = "You did not specify a file to upload.";
            }

        }
    }
}