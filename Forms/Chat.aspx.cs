using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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
                string userIdQuery = Request.QueryString["UserId"];

                // Validate UserId in Query String or Session, then store in hidden field
                if (!string.IsNullOrEmpty(userIdQuery) && int.TryParse(userIdQuery, out int userId))
                {
                    hiddenSenderId.Value = userIdQuery;
                    LoadUserOptions(userId);
                }
                else
                {
                    // Redirect to login if UserId is missing or invalid
                    Response.Redirect("~/Forms/Login.aspx");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear the user session on logout
            Session.Abandon();
            Response.Redirect("~/Forms/Login.aspx"); // Redirect to the login page
        }

        protected void UserSelectChanged(object sender, EventArgs e)
        {
            int selectedUserId;
            if (int.TryParse(userSelect.SelectedValue, out selectedUserId))
            {
                LoadMessagesForUser(selectedUserId);
            }
        }

        private void LoadUserOptions(int loggedInUserId)
        {
            try
            {
                using (var dbContext = new ChatAppDBContext())
                {
                    // Fetch users from the database excluding the logged-in user
                    var users = dbContext.Users
                        .Where(u => u.UserId != loggedInUserId)
                        .Select(u => new { u.UserId, u.UserName })
                        .ToList();

                    // Check if users list is not empty
                    if (users.Any())
                    {
                        // Bind the fetched users to the dropdown list
                        userSelect.DataSource = users;
                        userSelect.DataTextField = "UserName";
                        userSelect.DataValueField = "UserId";
                        userSelect.DataBind();

                        // Add a default prompt item
                        userSelect.Items.Insert(0, new ListItem("Select User", ""));
                    }
                    else
                    {
                        // Show a message if no other users are available
                        userSelect.Items.Insert(0, new ListItem("No other users available", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and redirect to an error page
                Console.WriteLine("An error occurred: " + ex.Message);
                Response.Redirect("~/Forms/Error.aspx");
            }
        }

        private void LoadMessagesForUser(int selectedUserId)
        {
            try
            {
                using (var dbContext = new ChatAppDBContext())
                {
                    int loggedInUserId = int.Parse(hiddenSenderId.Value); // Get logged-in user ID

                    // Fetch messages exchanged between the logged-in user and the selected user
                    var messages = dbContext.Messages
                        .Where(m => (m.SenderId == loggedInUserId && m.ReceiverId == selectedUserId) ||
                                    (m.SenderId == selectedUserId && m.ReceiverId == loggedInUserId))
                        .OrderBy(m => m.Timestamp) // Ensure messages are ordered by timestamp
                        .Select(m => new
                        {
                            UserName = m.Sender.UserName,
                            m.MessageText,
                            m.Timestamp,
                            IsRead = m.IsDelivered
                        })
                        .ToList();

                    // Clear previous messages
                    messageContainer.Controls.Clear();

                    // Populate the message display
                    foreach (var message in messages)
                    {
                        // Create a label for each message
                        var messageLabel = new Label
                        {
                            Text = $"{message.UserName}: {message.MessageText} <br/>",
                            CssClass = message.IsRead ? "message read" : "message unread" // Optional: apply different styles based on read status
                        };
                        messageContainer.Controls.Add(messageLabel);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and redirect to an error page
                Console.WriteLine("An error occurred: " + ex.Message);
                Response.Redirect("~/Forms/Error.aspx");
            }
        }

        public void AppendMessage(string userId, string message, bool isRead)
        {
            var messageContainer = (HtmlGenericControl)FindControl("messageContainer");
            if (messageContainer != null)
            {
                // Construct the message HTML
                string msgClass = (Session["UserId"].ToString() == userId) ? "message-sent" : "message-received";
                string readReceipt = isRead ? " - Read" : "";
                string msg = $"<div class='chat-message {msgClass}'><strong>{userId}</strong>: {message}{readReceipt}</div>";

                // Append message to the message container
                messageContainer.InnerHtml += msg;

                // Clearfix div to ensure layout integrity
                messageContainer.InnerHtml += "<div class='clearfix'></div>";
            }
        }


        protected void RoomChanged(object sender, EventArgs e)
        {
            // Handle room selection changes here
        }
    }
}
