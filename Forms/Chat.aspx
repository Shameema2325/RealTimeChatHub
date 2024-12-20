﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="RealTimeChatHub.Forms.Chat" EnableSessionState="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="/Scripts/jquery.signalR-2.4.2.min.js"></script>
    <script src="/signalr/hubs"></script>
    <title>ChatBot</title>
    <style>
        /* General message styling */
        .message {
            display: block;
            max-width: 60%;
            margin: 10px 0;
            padding: 10px;
            border-radius: 10px;
            position: relative;
            font-size: 14px;
        }

        /* Styling for sender messages (right-aligned) */
        .sender {
            align-self: flex-end;
            background-color: #d1e7dd; /* Light green for sender */
            text-align: right;
            float: right;
            clear: both;
        }

        /* Styling for receiver messages (left-aligned) */
        .receiver {
            align-self: flex-start;
            background-color: #f8d7da; /* Light red for receiver */
            text-align: left;
            float: left;
            clear: both;
        }

        /* Styling for read/unread indicators */
        .read {
            opacity: 0.6;
        }
        .unread {
            font-weight: bold;
        }        

        /* Clearfix to prevent container collapse */
        .clearfix::after {
            content: "";
            clear: both;
            display: table;
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

            <!-- Existing Rooms Dropdown and Room Creation -->
            <div class="row p-3">
                <div class="col-12">
                    <asp:DropDownList ID="roomSelect" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RoomChanged" CssClass="form-control">
                        <asp:ListItem Text="Select Room" Value="" />
                    </asp:DropDownList>
                    <asp:TextBox ID="roomNameTextBox" runat="server" CssClass="form-control mt-2" Placeholder="Enter new chat room name..." />
                    <asp:Button ID="btnCreateRoom" runat="server" Text="Create Room" CssClass="btn btn-primary mt-2" OnClick="btnCreateRoom_Click" />
                </div>
            </div>


            <!-- User Selection -->
            <div class="row p-3">
                <div class="col-12">
                    <asp:DropDownList ID="userSelect" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UserSelectChanged"></asp:DropDownList>
                    <asp:HiddenField ID="hiddenSenderId" runat="server" />
                </div>
            </div>

            <!-- Chat Area -->
            <div class="row flex-grow-1 overflow-auto" id="chatArea">
                <div class="col-12">
                    <div class="p-3" id="messageContainer" runat="server" style="height: 100%; overflow-y: auto;">
                        <!-- Messages will be appended here dynamically -->
                    </div>
                </div>
            </div>

            <!-- Message Input Area -->
            <div class="row bg-light p-3">
                <div class="col-10">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Type a message..."></asp:TextBox>
                    <button type="button" id="emojiPickerButton" class="btn btn-secondary mt-1">😊</button>
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
                var userId = $('#hiddenSenderId').val();
                chatHub = $.hubConnection();
                var chatProxy = chatHub.createHubProxy("chatHub");

                // Listen for incoming messages
                chatProxy.on("receiveMessage", function (senderId, message, timestamp, isRead) {
                    appendMessage(senderId, message, senderId, isRead);
                });

                chatHub.start()
                    .done(function () {
                        console.log("Connected to the chat hub!");
                    })
                    .fail(function (err) {
                        console.error("Connection failed: " + err.toString());
                    });
            }

            function appendMessage(user, message, senderId, isRead) {
                const msgClass = senderId === $('#hiddenSenderId').val() ? 'sender' : 'receiver';
                const readReceipt = isRead ? `<span class="badge badge-secondary">Read</span>` : '';
                const msg = `<div class="message ${msgClass} clearfix"><strong>${user}</strong>: ${message} ${readReceipt}</div>`;
                $("#messageContainer").append(msg);
                $("#messageContainer").scrollTop($("#messageContainer")[0].scrollHeight);
            }

            $('#btnCreateRoom').click(function () {
                var roomName = $('#roomNameTextBox').val().trim();
                if (roomName) {
                    $.ajax({
                        type: "POST",
                        url: "/api/rooms/create", // Adjust the API endpoint accordingly
                        contentType: "application/json",
                        data: JSON.stringify({ RoomName: roomName }),
                        success: function () {
                            $('#roomNameTextBox').val(''); // Clear the text box
                            loadChatRooms(); // Refresh room list after creation
                        },
                        error: function () {
                            alert('Failed to create chat room. Please try again.');
                        }
                    });
                } else {
                    alert('Please enter a room name.');
                }
            });

            function loadChatRooms() {
                $.ajax({
                    url: "/api/rooms/list",
                    method: "GET",
                    success: function (data) {
                        $("#roomSelect").empty(); // Clear existing options
                        $("#roomSelect").append('<option value="">Select Room</option>'); 
                        data.forEach(function (room) {
                            $("#roomSelect").append(`<option value="${room.Id}">${room.RoomName}</option>`);
                        });
                    },
                    error: function () {
                        alert('Failed to load chat rooms.');
                    }
                });
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
                            appendMessage("You", messageText, $('#hiddenSenderId').val(), true);
                            $('#txtMessage').val('');
                        },
                        error: function () {
                            alert('Failed to send message. Please try again.');
                        }
                    });
                } else {
                    alert('Please select a user and enter a message.');
                }
            }

           $('#btnSend').click(function () {
            sendMessage();
            });

            $(document).ready(function () {
                loadChatRooms(); // Load chat rooms when the page is ready
                initializeSignalR();
            });
        });
    </script>


</body>
</html>
