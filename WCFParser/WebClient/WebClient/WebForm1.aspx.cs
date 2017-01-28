using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebClient.ImageService;

namespace WebClient
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        ImageService.ImageServiceClient _client = new ImageServiceClient("BasicHttpBinding_IImageService");
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSetWord_Click(object sender, EventArgs e)
        {
            txtWnid.Text = "";
            txtName.Text = "";
            
        }

        protected void btnGetWord_Click(object sender, EventArgs e)
        {
            if (txtWnid.Text.Length != 0)
            {
                var wnid = txtWnid.Text;
                var findName = _client.GetWord(wnid, "fromId");
                txtName.Text = findName.Name;
            }
            else if (txtName.Text.Length != 0)
            {
                var name = txtName.Text;
                var findName = _client.GetWord(name, "fromWord");
                txtWnid.Text = findName.Wnid;
            }
        }
    }
}