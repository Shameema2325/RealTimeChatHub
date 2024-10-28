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
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected async void BtnRegister_Click(object sender, EventArgs e)
        {
            string userName = txtUsername.Text;
            string password = txtPassword.Text;

            // Create the registration data
            var registerData = new { UserName = userName, Password = password };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51843/"); 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Serialize registration data and send POST request
                var content = new StringContent(JsonConvert.SerializeObject(registerData), System.Text.Encoding.UTF8, "application/json");
                try
                {
                    HttpResponseMessage response = await client.PostAsync("api/user/register", content);

                    if (response.IsSuccessStatusCode)
                    {
                        lblMessage.Text = "Registration successful! Please log in.";
                        Response.Redirect("Login.aspx"); // Redirect to login page if registration is successful
                    }
                    else
                    {
                        Response.Write("<script>alert('Registration failed: " + response.ReasonPhrase + "');</script>");
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"An error occurred: {ex.Message}";
                }

            }
        }
    }
}