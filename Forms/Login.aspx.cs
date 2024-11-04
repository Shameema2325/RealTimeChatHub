using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.UI;

namespace RealTimeChatHub.Forms
{   
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected async void BtnLogin_Click(object sender, EventArgs e)
        {
            // Prepare login data
            var loginData = new { UserName = txtUsername.Text, Password = txtPassword.Text };

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

                        Application["UserId"] = result.UserId; // Set User Id

                        Response.Redirect("Chat.aspx"); // Redirect to main chat page
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