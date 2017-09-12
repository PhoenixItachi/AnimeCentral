$(document).ready(function () {
  var width = $(".place").width();
  $(".place").height(width / 79 * 106);
  $(".score").each(function () {
    $(this).find("span").css("font-size", "100%");
    if ($(this).height() < $(".place").height() * 0.8) {
      return;
    }
    var newFontSizePerc = 80;
    while ($(this).height() > $(".place").height() * 0.8) {
      $(this).find("span").css("font-size", newFontSizePerc + "%");
      newFontSizePerc - 10;
      if (newFontSizePerc == 30)
        break;
    }
  });

  var slideShow = $(".slideshow");
  var currentWidth = $(slideShow).width();
  var resizedHeight = currentWidth / 16 * 9;
  $(slideShow).height(resizedHeight);

  $(".show-top-10").click(function () {
    var targetToShow = $(this).attr("data-target");
    $(targetToShow).toggleClass("show");
    if ($(targetToShow).hasClass("show")) {
      $(this).find("i").removeClass("fa-angle-double-down");
      $(this).find("i").addClass("fa-angle-double-up");
    } else {
      $(this).find("i").addClass("fa-angle-double-down");
      $(this).find("i").removeClass("fa-angle-double-up");
    }
  })
});

$(function () {

  var messaging = firebase.messaging();
  messaging.requestPermission().then(function () {
    console.log("Have permission");
    messaging.getToken()
      .then(function (currentToken) {
        $.post("Anime/SetNotificationToken", $.param({ token: currentToken }), function () {
          console.log("Token Registered.");
        }).fail(function () {
          console.log("An error appeared.");
        });
      })
      .catch(function (err) {
        console.log('An error occurred while retrieving token. ', err);
      });
  }).catch(function (err) {
    console.log(err);
  });

  messaging.onMessage(function (payload) {
    console.log("onMessage: ", payload);
  });

  updateFbLikeBox();
  // Slideshow Commands
  $(".next-slide").click(function () {
    var currentSlide = $(".anime-recom .slides .active");
    var slidesCount = $(".anime-recom .slides .slide").length - 1;
    var slideIndex = $(currentSlide).attr("data-index");
    var nextSlideIndex;
    if (slideIndex == slidesCount)
      nextSlideIndex = 0;
    else
      nextSlideIndex = parseInt(slideIndex) + 1;

    makeSlideActive(nextSlideIndex);
  })

  $(".prev-slide").click(function () {
    var currentSlide = $(".anime-recom .slides .active");
    var slideIndex = $(currentSlide).attr("data-index");
    var slidesCount = $(".anime-recom .slides .slide").length - 1;
    var nextSlideIndex;

    if (slideIndex == 0)
      nextSlideIndex = slidesCount;
    else
      nextSlideIndex = parseInt(slideIndex) - 1;

    makeSlideActive(nextSlideIndex);
  })

  function makeSlideActive(slideIndex) {
    var slideToShow = $('.anime-recom .slides .slide[data-index="' + slideIndex + '"]');
    $(".anime-recom .slides .active").removeClass("active");
    $(slideToShow).addClass("active");
  }



  $(window).resize(function () {
    var slideShow = $(".slideshow");
    var currentWidth = $(slideShow).width();
    var resizedHeight = currentWidth / 16 * 9;
    $(slideShow).height(resizedHeight);

    var width = $(".place").width();
    $(".place").height(width / 79 * 106);

    $(".score").each(function () {
      $(this).find("span").css("font-size", "100%");
      if ($(this).height() > $(".place").height() * 0.8) {
        $(this).find("span").css("font-size", "70%");
        return;
      }
    });

    updateAnimeListWidth();
    updateFbLikeBox();
  })

  $(".more-genre-btn").click(function () {
    var moreGenre = $(".more-genre");
    if (moreGenre.css("display") == "none") {
      $(".more-genre").css("display", "inline");
      $(this).removeClass("fa-angle-double-down");
      $(this).addClass("fa-angle-double-up");
    } else {
      $(".more-genre").css("display", "none");
      $(this).removeClass("fa-angle-double-up");
      $(this).addClass("fa-angle-double-down");
    }
  });

  $(".show-chat").click(function () {
    var chat = $(".chat");
    if (!chat.hasClass("chat-showed")) {
      $(chat).addClass("chat-showed");
      $(this).find("i").removeClass("fa-angle-double-up");
      $(this).find("i").addClass("fa-angle-double-down");
    } else {
      $(chat).removeClass("chat-showed");
      $(this).find("i").removeClass("fa-angle-double-down");
      $(this).find("i").addClass("fa-angle-double-up");
    }
  });

  $(".show-disclaimer").click(function () {
    $(".disclaimer").toggleClass("show-disc");
  })

  $(".switch-theme").click(function () {
    $("body").toggleClass("dark-theme");

    if ($("body").hasClass("dark-theme"))
      $(this).find("b").text("light theme");
    else
      $(this).find("b").text("dark theme");
  })
})


function updateFbLikeBox() {
  var likeBox = $("#facebook-like-box");
  var likeBoxContainer = $(".fb-like-box-container");
  var containerWidthInt = parseInt($(likeBoxContainer).width());
  if (containerWidthInt > 500)
    containerWidthInt = 500;
  $(likeBox).attr("src", srcFacebookFormat(containerWidthInt, $(likeBox).height()));
  $(likeBox).attr("width", containerWidthInt);
}

function updateAnimeListWidth() {
  var animeList = $(".content-body .anime-list");
  if (animeList.length > 0) {
    var padding = animeList.innerWidth() - animeList.width();
    var contentWidth = animeList.closest(".content").width() - padding;
    var newWidth = parseInt(contentWidth / 200) * 200;
    animeList.width(newWidth);
  }
}

function srcFacebookFormat(width, height) {
  return "https://www.facebook.com/plugins/page.php?href=https%3A%2F%2Fwww.facebook.com%2Fwiensub%2F&tabs&width=" + width + "&height=" + height + "&small_header=false&adapt_container_width=true&hide_cover=false&show_facepile=false&appId=531637330233211";
}

var searchTimeOut;
$(document).on("input", ".add-anime-partial .anime-search-input", function () {
  var partial = $(".add-anime-partial");
  var spinner = partial.find(".spinner");
  var noResult = partial.find(".no-results");
  var animeSearchResult = partial.find(".anime-search-results");
  var resultList = partial.find(".result-list");
  var result = getResultTemplate();
  var searchValue = partial.find(".search-value");

  spinner.show();
  clearTimeout(searchTimeOut);
  noResult.hide();
  var animeSearchTitle = $(this).val();
  if (animeSearchTitle == "") {
    animeSearchResult.hide();
    var resultTemplate = result.clone();
    resultTemplate.css("display", "none");
    console.log(resultList);
    $(resultList).html(resultTemplate[0].outerHTML);
    return;
  }

  animeSearchResult.show();
  var resultListTop = resultList.offset().top - $(window).scrollTop();
  var distanceToBottom = $(window).innerHeight() - resultListTop;
  console.log("Result list to top: " + resultListTop);
  console.log("Window scrool top" + $(window).scrollTop());

  resultList.css("max-height", distanceToBottom - spinner.height() - noResult.height());

  searchValue.text(animeSearchTitle);
  searchTimeOut = setTimeout(function () {
    $.ajax({
      url: "/Anime/SearchAnime" + "?searchText=" + animeSearchTitle,
      success: function (data) {
        if (data.more != undefined) {
          $(resultList).hide();
          $(spinner).hide();
          $(noResult).show();
          return;
        }
        $(noResult).hide();
        var resultsList = "";
        for (i = 0; i < data.length; i++) {
          var resultTemplate = $(result).clone();
          resultTemplate.find(".anime-image img").attr("src", data[i].image);
          resultTemplate.find(".anime-title b").text(data[i].title);
          resultTemplate.find(".anime-title .anime-type").text(data[i].type);
          resultTemplate.find(".anime-episodes").text("Episoade: " + data[i].noOfEpisodes);
          resultTemplate.find(".anime-score .result-score").text(data[i].score);
          resultTemplate.find(".anime-status").text(data[i].status);
          resultTemplate.css("display", "table");
          resultTemplate.attr("data-malid", data[i].malId);
          resultsList += resultTemplate[0].outerHTML;
        }
        $(resultList).html(resultsList);
        $(resultList).show();
        $(spinner).hide();
      }
    });
  }, 500);
});

function getResultTemplate() {
  return $('<div class="result" style="display:none">'
    + '<div class="anime-image"><img /></div>'
    + '<div class="details">'
    + '<div class="anime-title"><b></b> (<span class="anime-type"></span>)</div>'
    + '<div class="anime-score"><span class="glyphicon glyphicon-star" style="color:gold;"></span><span class="result-score"></span></div>'
    + '<div class="anime-episodes"></div>'
    + '<div class="anime-status"></div>'
    + '</div>'
    + '</div>');
}

$(document).on("click", ".add-anime-partial .result", function () {
  var malId = $(this).attr("data-malid");
  var animeTitle = $(this).find(".anime-title b").text();
  var animeAddError = $(this).closest(".add-anime-partial").find(".add-anime-error");
  var searchResultsList = $(this).closest(".anime-search-results");

  animeAddError.text("");
  $.get("/Anime/GetAnimeForm?" + "malId=" + malId + "&title=" + animeTitle, function (data) {
    $(".add-anime-partial .form-container").html(data);
    searchResultsList.hide();
  })
    .fail(function (data) {
      $(animeAddError).text(data.responseText);
    });

})

// Anime Partial/PopUp Events
$(document).on("click", ".add-episode", function () {
  var animeId = $(this).attr("data-anime-id");
  $.get("Anime/GetAddEpisodePartial?animeId=" + animeId, function (data) {
    $(".pop-up").append(data);
  }).fail(function () {
    alert("A aparut o eroare in adaugarea episodul.");
  });
})

$(document).on("submit", '.add-episode-form', function (e) {
  e.preventDefault();
  var form = $(this).closest(".content");
  $.ajax({
    type: "POST",
    url: "/Anime/AddEpisode",
    data: new FormData(this),
    processData: false,
    contentType: false
  }).done(function (data) {
    alert('Episod adaugat!');
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });

});

$(document).on("click", ".edit-anime-btn", function () {
  var animeId = $(this).attr("data-anime-id");
  $.get("Anime/GetEditAnimePartial?id=" + animeId, function (data) {
    $(".pop-up").append(data);
  }).fail(function (data) {
    alert("A aparut o eroare in cautarea episodului.");
  })
});

// Add Anime Partial/Popup Events
$(document).on('submit', '.add-anime-form', (function (e) {
  e.preventDefault();
  var form = $(this);
  $.post('/Anime/AddAnime', $(this).serialize(), function (result) {
    alert('Anime adaugat!');
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
}));

// Add Announcement Partial/Popup Events
$(document).on('submit', '.add-announcement-form', (function (e) {
  e.preventDefault();
  var form = $(this).closest(".content");
  $.post('/Anime/AddAnnouncement', $(this).serialize(), function (result) {
    alert('Anunt adaugat!');
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
}));

// Add Episode Partial/PopUp Events
$(document).on("click", ".add-source", function () {
  var source = $(".sources .source").first().clone();
  var sourcesCurrentCount = $(".sources .source").length;
  $(".remove-source").show();
  source.find(".source-tag").attr("name", "Sources[" + sourcesCurrentCount + "].Label");
  source.find(".source-link").attr("name", "Sources[" + sourcesCurrentCount + "].Link");
  $(".sources").not(".local").append(source);
});

$(document).on("click", ".remove-source", function () {
  var sourcesCurrentCount = $(".sources").not(".local").find(".source").length;

  if (sourcesCurrentCount == 1)
    return;

  if (sourcesCurrentCount == 2)
    $(this).hide();


  $(".sources").not(".local").find(".source").last().remove();
});

// Edit Episode Partial/PopUp Events
$(document).on('submit', '.edit-episode-form', function (e) {
  e.preventDefault();
  var form = $(this).closest(".content");
  $.post('/Anime/EditEpisode', $(this).serialize(), function (result) {
    alert('Episod a fost salvat!');
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
});


// Anime List Partial/PopUp Events
$(document).on("input", ".anime-list-partial .anime-search .anime-search-input", function () {
  var searchText = $(this).val().toLowerCase();
  if (searchText == "") {
    $(".anime-list .anime-list-item").removeClass("hide-by-name");
    return;
  }

  $(".anime-list .anime-list-item[data-name*='" + searchText + "']").removeClass("hide-by-name");
  $(".anime-list .anime-list-item").not("[data-name*='" + searchText + "']").addClass("hide-by-name");
});

$(document).on("change", ".by-genre input", function () {
  $(this).closest(".option").toggleClass("active");
  var checkedOptions = $(".by-genre input:checked");
  if (checkedOptions.length == 0) {
    $(".anime-list .anime-list-item").removeClass("hide-by-genre");
    return;
  }

  var genres = $.map(checkedOptions, function (el) {
    return $(el).val();
  });

  var animeWithGenres = $(".anime-list .anime-list-item").filter(function () {
    var animeGenres = $(this).attr("data-genre");
    var toHide = false;
    $(genres).each(function (index, el) {
      if (animeGenres.indexOf(el) < 0) {
        toHide = true;
        return false;
      }
    });

    if (toHide)
      $(this).addClass("hide-by-genre");
    else
      $(this).removeClass("hide-by-genre");
  });
})

$(document).on("click", ".by-status .option", function () {
  $(".by-status .option").removeClass("active");
  $(this).addClass("active");

  if ($(this).text() == "Toate") {
    $(".anime-list .anime-list-item").removeClass("hide-by-status");
    return;
  }

  $(".anime-list .anime-list-item[data-status='" + $(this).text() + "']").removeClass("hide-by-status");
  $(".anime-list .anime-list-item").not("[data-status='" + $(this).text() + "']").addClass("hide-by-status");
});


// Edit Anime Partial/PopUp Events
$(document).on('submit', '.edit-anime-form', function (e) {
  e.preventDefault();
  var form = $(this).closest(".content");
  var body = $(this).closest(".content-body");
  $(body).showSpinner();
  $.post('/Anime/EditAnime', $(this).serialize(), function (result) {
    alert('Anime editat!');
    $(body).hideSpinner();
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
});

// Episode Partial/PopUp Events
$(document).on("submit", ".new-comment-form", function (event) {
  event.preventDefault();
  var form = $(this).closest(".new-comment-form");
  $.post("Anime/AddComment", $(this).serialize(), function (data) {
    RefreshComments();
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
});

$(document).on("click", ".delete-comment", function () {
  var commentId = $(this).attr("data-comment-id");
  $.post("Anime/DeleteComment?id=" + commentId, function () {
    RefreshComments();
  }).fail(function (data) {
    alert(data.responseText);
  })
});

$(document).on('click', ".edit-episode", function (e) {
  var episodeId = $(this).attr("data-episode-id");
  e.stopPropagation();
  console.log("ASD");
  $.get("Anime/GetEditEpisodePartial?id=" + episodeId, function (data) {
    $(".pop-up").append(data);
  }).fail(function () {
    alert("A aparut o problema!");
  });
})

// Login & Register Partial/PopUp Events
$(document).on("submit", ".login-form", function (e) {
  e.preventDefault();
  var form = $(this).closest(".content");
  $.post('Account/Login', $(this).serialize(), function (data) {
    if (data.success)
      window.location = '/';
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
});

$(document).on("submit", ".register-form", function (e) {
  e.preventDefault();
  var form = $(this).closest(".content");
  $.post('/Account/Register', $(this).serialize(), function (data) {
    if (data.success)
      window.location = '/';
  }).fail(function (data) {
    if (data.status == 477) {
      $(form).replaceWith(data.responseText);
    }
    else
      alert(data.responseText);
  });
});

// Change Translate Status
$(document).on("change", ".set-translate-status", function () {
  var newStatus = $(this).val();
  var animeId = $(this).attr("data-anime-id");
  console.log(newStatus);
  console.log(animeId);
  $.post("/Anime/SetAnimeTranslateStatus", { Id: animeId, TranslateStatus: newStatus }, function () {
    alert("Status modificat");
  }).fail(function () {
    alert("A aparut o problema");
  });
});

$(document).on("click", ".genre-filter .genre-list span", function () {
  $(this).toggleClass("active");
});

$(document).on("click", ".genre-filter .search-button", function () {
  var genres = $(this).closest(".genre-filter").find(".genre-list span.active");
  var clear = $(this).attr("data-popup-clear");
  if (clear == "true")
    $(".pop-up").find(".content").not(".spinner-container").remove();

  $(".pop-up").css("z-index", 300);
  $(".pop-up").css("opacity", 1);
  $.get("Anime/GetAllAnime", function (data) {
    $(".pop-up").append(data);
    var advancedSearch = $(".pop-up .anime-list-partial").find(".advanced-search");
    $(genres).each(function () {
      var genre = $(this).text();
      $(advancedSearch).find(".option input[type='checkbox'][value='" + genre + "']").prop("checked", true);
      $(advancedSearch).find(".option input[type='checkbox'][value='" + genre + "']").change();
    })
    updateAnimeListWidth();
  }).fail(function (data) {
    console.log(data);
  });
})

$(document).on("click", ".mal-source", function (event) {
  event.stopPropagation();
});

// Forgot Password Form
$(document).on("submit", ".forgot-password-form", function (event) {
  event.preventDefault();
  var form = $(this).closest(".content");
  $.post("Account/ForgotPassword", $(this).serialize(), function (result) {
    alert("Cerere de resetare a parolei trimisa.");
  }).fail(function (data) {
    if (data.status == 477)
      $(form).replaceWith(data.responseText);
    else
      alert(data.responseText);
  });
})

$(document).on("click", ".forgot-password-popup", function () {
  $.get("Account/ForgotPassword", function (result) {
    $(".pop-up").append(result);
  });
});

// Resend Activation Link Form
$(document).on("submit", ".resend-activation-link", function (event) {
  event.preventDefault();
  var form = $(this).closest(".content");
  $.post("Account/ResendActivationLink", $(this).serialize(), function () {
    window.location = "/";
  }).fail(function (data) {
    if (data.status == 477) {
      $(form).replaceWith(data);
    }
    else {
      alert(data);
    }
  })
})

$(document).on("click", ".resend-activation-link-popup", function () {
  $.get("Account/ResendActivationLink", function (result) {
    $(".pop-up").append(result);
  });
});

// Extensions

// Spinner
$.fn.showSpinner = function () {
  var container = $(this);
  var spinner = $(container).children(".spinner-injected");
  if ($(spinner).length != 0) {
    $(spinner).show();
    return;
  }
  else {
    var spinner = CreateSpinner();
    $(container).append(spinner);

  }
}

$.fn.hideSpinner = function () {
  var container = $(this);
  var spinner = $(container).children(".spinner-injected");
  if (spinner.length != 0)
    $(spinner.hide());
};

function CreateSpinner() {
  return $("<div class='spinner-injected'>"
    + "<div class='spinner'>"
    + "<div class='bounce1'></div>"
    + "<div class='bounce2'></div>"
    + "<div class='bounce3'></div>"
    + "<span> Se incarca...</span>"
    + "</div>"
    + "</div>");
}


// Autocomplete
$(document).on("DOMNodeInserted", function (evt) {
  var target = $(evt.target);
  var objects = [];
  if ($(target).attr("data-multiple") != undefined) {
    setAutocomplete($(target));
    return;
  }

  $(target).find("input[data-multiple]").each(function () {
    setAutocomplete($(this));
  })
});

function setAutocomplete(element) {
  var optionsString = $(element).attr("data-list");
  if (optionsString == undefined)
    return;

  var options = optionsString.split(",").map(function (item) {
    return item.trim();
  });

  if (options.length == 0)
    return;

  $(element).on("keydown", function (event) {
    if (event.keyCode === $.ui.keyCode.TAB &&
      $(this).autocomplete("instance").menu.active) {
      event.preventDefault();
    }
  })
    .autocomplete({
      minLength: 0,
      selectFirst: true,
      source: function (request, response) {
        // delegate back to autocomplete, but extract the last term
        response($.ui.autocomplete.filter(
          options, extractLast(request.term)));
      },
      focus: function () {
        // prevent value inserted on focus
        return false;
      },
      select: function (event, ui) {
        var terms = split(this.value);
        // remove the current input
        terms.pop();

        // add if doesn't exist already
        if (terms.indexOf(ui.item.value) < 0)
          terms.push(ui.item.value);

        // add the selected item
        // add placeholder to get the comma-and-space at the end
        terms.push("");
        this.value = terms.join(", ");
        return false;
      }
    });
}

function split(val) {
  return val.split(/,\s*/);
}
function extractLast(term) {
  return split(term).pop();
}

