var hub;
$(function () {
  // Chat

  var chat = $(".chat.chat-signalR");
  var messageBox = $(chat).find(".messages-box");
  $(chat).showSpinner();
  var connection = $.hubConnection();
  hub = connection.createHubProxy('chat');
  var broadcastMessage = function (user, msg) {
    if (user != null) {
      newMessage(user, msg).appendTo(messageBox);
    }
  }

  hub.on('broadcastMessage', broadcastMessage);
  connection.start().done(function () {
    $(chat).hideSpinner();
  });
});

$(document).on("keypress", ".message-form .message-input", function (e) {
  var form = $(this).closest(".message-form");
  if (form == null || form == undefined)
    return;

  if (e.which == 13) {
    $(form).submit();
    $(this).val("");
    e.preventDefault();
  }
});


$(document).on("submit", ".chat-box .message-form", function (e) {
  e.preventDefault();
  var messageInput = $(this).find(".message-input");
  if (messageInput.length != 0 && $(messageInput).val() != "") {
    hub.invoke('send', $(messageInput).val());
  }
});

function newMessage(user, message) {
  return $("<div class='message-entry'>"
    + "<div class='message-date'>" + message.date + "</div>"
    + "<div class='message-user'>"
    + "<img src='" + user.image + "' class='profile-pic' />"
    + "<div class='user-name'>" + user.username + ":</div>"
    + "</div>"
    + "<div class='message-content'>" + bbcode.render(message.content) + "</div>"
    + "</div>");
}