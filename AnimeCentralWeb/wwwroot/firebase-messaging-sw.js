importScripts("https://www.gstatic.com/firebasejs/4.3.0/firebase-app.js");
importScripts("https://www.gstatic.com/firebasejs/4.3.0/firebase-messaging.js");

var config = {
  apiKey: "AIzaSyB6VH7RhduKr_kE4EW9p8RfrL2ix3xj-sg",
  authDomain: "animecentral-64740.firebaseapp.com",
  databaseURL: "https://animecentral-64740.firebaseio.com",
  projectId: "animecentral-64740",
  storageBucket: "animecentral-64740.appspot.com",
  messagingSenderId: "358676794332"
};
firebase.initializeApp(config);

var messaging = firebase.messaging();
messaging.setBackgroundMessageHandler(function (payload) {
  console.log('[firebase-messaging-sw.js] Received background message ', payload);
  // Customize notification here
  const notificationTitle = payload.data.title;
  const notificationOptions = {
    body: payload.data.body,
  };
  return self.registration.showNotification(payload.notication.title, payload);
});