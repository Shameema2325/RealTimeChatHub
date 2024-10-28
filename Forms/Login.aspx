<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RealTimeChatHub.Forms.Login" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container d-flex justify-content-center align-items-center" style="height: 100vh;">
            <div class="card p-4" style="width: 300px;">
                <h3 class="text-center mb-3">Login</h3>
                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />
                <div class="form-group">
                    <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUsername" Text="Username" />
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUsername" ErrorMessage="Enter Username" ForeColor="Maroon"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="Password" />
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword" ErrorMessage="Enter password" ForeColor="Maroon"></asp:RequiredFieldValidator>
                </div>
                <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary btn-block" style="background-color: teal" Text="Login" OnClick="BtnLogin_Click" />
                <p class="mt-3 text-center"><a href="Register.aspx">Register</a></p>
            </div>
        </div>
    </form>
</body>
</html>
