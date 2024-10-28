using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RealTimeChatHub.Forms
{   
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected async void BtnLogin_Click(object sender, EventArgs e)
        {
            string userName = txtUsername.Text;
            string password = txtPassword.Text;

            // Prepare login data
            var loginData = new { UserName = userName, Password = password };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51843/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Serialize login data and send POST request
                var content = new StringContent(JsonConvert.SerializeObject(loginData), System.Text.Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("api/user/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<LoginResponse>(responseData);

                        // Store token in session for future use
                        Session["AuthToken"] = result.Token;
                        Response.Redirect("Chat.aspx?UserId=" + result.UserId + ""); // Redirect to main chat page
                    }
                    else
                    {
                        lblMessage.Text = "Invalid username or password.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"An error occurred: {ex.Message}";
                }
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}