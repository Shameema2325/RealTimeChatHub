using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RealTimeChatHub.Models;

namespace RealTimeChatHub.Forms
{
    public partial class Chat : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Ensure this only runs on the first load
            {
                if (Request.QueryString["UserId"] != null)
                {
                    hiddenSenderId.Value = Request.QueryString["UserId"]; // Store the logged-in user's ID in a hidden field
                    LoadUserOptions(Convert.ToInt32(hiddenSenderId.Value)); // Load other users for selection
                }
                else
                {
                    // Redirect to login if the session does not have UserId
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear the user session on logout
            Session.Abandon(); 
            Response.Redirect("~/Forms/Login.aspx"); // Redirect to the login page
        }

        private void LoadUserOptions(int loggedInUserId)
        {
            using (var dbContext = new ChatAppDBContext())
            {
                // Fetch users from the database excluding the logged-in user
                var users = dbContext.Users
                    .Where(u => u.UserId != loggedInUserId)
                    .Select(u => new { u.UserId, u.UserName })
                    .ToList();

                // Bind the fetched users to the dropdown list
                userSelect.DataSource = users;
                userSelect.DataTextField = "UserName";
                userSelect.DataValueField = "UserId";
                userSelect.DataBind();

                // Optionally add a default item to prompt user selection
                userSelect.Items.Insert(0, new ListItem("Select User", ""));
            }
        }
    }
}
