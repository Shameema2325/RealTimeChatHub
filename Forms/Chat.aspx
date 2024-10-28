<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="RealTimeChatHub.Forms.Chat" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="/Forms/Scripts/jquery-1.12.4.js"></script>
    <script src="/signalr/hubs"></script>
    <title>ChatBot</title>
    <style>
        body {
            background-color: #f8f9fa;
        }

        .chat-message {
            padding: 10px;
            border-radius: 5px;
            margin: 5px 0;
            max-width: 70%;
        }

        .message-sent {
            background-color: #007bff;
            color: white;
            margin-left: auto;
        }

        .message-received {
            background-color: #e7e7e7;
            color: black;
        }

        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid vh-100 d-flex flex-column">
            <!-- Chat Header -->
            <div class="row text-white p-3" style="background-color: teal">
                <div class="col">
                    <h3>Real-Time Chat Room</h3>
                </div>
                <div class="col-auto">
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn btn-danger" OnClick="btnLogout_Click" />
                </div>
            </div>

            <!-- User Selection -->
            <div class="row p-3">
                <div class="col-12">
                    <asp:DropDownList ID="userSelect" CssClass="form-control" runat="server"></asp:DropDownList>
                    <asp:HiddenField ID="hiddenSenderId" runat="server" />
                </div>
            </div>

            <!-- Chat Area -->
            <div class="row flex-grow-1 overflow-auto" id="chatArea">
                <div class="col-12">
                    <div class="p-3" id="messageContainer" style="height: 100%; overflow-y: auto;">
                        <!-- Messages will be appended here dynamically -->
                    </div>
                </div>
            </div>

            <!-- Message Input Area -->
            <div class="row bg-light p-3">
                <div class="col-10">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Type a message..."></asp:TextBox>
                </div>
                <div class="col-2">
                    <button id="btnSend" type="button" class="btn btn-primary btn-block" style="background-color: teal">Send</button>
                </div>
            </div>
        </div>
    </form>

    <script>
        $(function () {
            var chatHub;

            function initializeSignalR() {
                chatHub = $.hubConnection("http://localhost:51843/signalr");
                var chatProxy = chatHub.createHubProxy("chatHub");

                // Set up the message receiving function
                chatProxy.on("ReceiveMessage", function (user, message, senderId) {
                    appendMessage(user, message, senderId);
                });

                // Start the SignalR connection
                chatHub.start().then(function () {
                    console.log("Connected to the chat hub!");
                }).catch(function (err) {
                    console.log("Not connected to the chat hub!");
                    console.error(err.toString());
                });
            }

            function appendMessage(user, message, senderId) {
                const msgClass = senderId === $('#hiddenSenderId').val() ? 'message-sent' : 'message-received';
                const msg = `<div class="chat-message ${msgClass}"><strong>${user}</strong>: ${message}</div>`;
                $("#messageContainer").append(msg);
                $("#messageContainer").scrollTop($("#messageContainer")[0].scrollHeight); // Scroll to the bottom
            }

            function sendMessage() {
                var selectedUserId = $('#userSelect').val();
                var messageText = $('#txtMessage').val().trim();
                if (selectedUserId && messageText) {
                    $.ajax({
                        type: "POST",
                        url: "/api/messages/send",
                        contentType: "application/json",
                        data: JSON.stringify({
                            SenderId: $('#hiddenSenderId').val(),
                            ReceiverId: selectedUserId,
                            MessageText: messageText
                        }),
                        success: function () {
                            // Append sent message to message container
                            appendMessage("You", messageText, $('#hiddenSenderId').val());
                            $('#txtMessage').val(''); // Clear input
                        },
                        error: function () {
                            alert('Failed to send message. Please try again.');
                        }
                    });
                } else {
                    alert('Please select a user and enter a message.');
                }
            }

            // Attach click event to send button
            $('#btnSend').click(function () {
                sendMessage();
            });

            // Initialize SignalR connection
            initializeSignalR();
        });
    </script>
</body>
</html>
