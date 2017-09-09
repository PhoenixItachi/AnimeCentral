var hub;
$(function () {
  // Chat
  var connection = $.hubConnection();
  hub = connection.createHubProxy('chat');
  var broadcastMessage = function (user, msg) {
    if (user != null) {
      console.log(user);
      $("<p>").text(msg).appendTo(".chat-signalR");
    }
  }

  hub.on('broadcastMessage', broadcastMessage);
  connection.start(function () {
    hub.invoke("join");
  });
  console.log("connection started");
})

