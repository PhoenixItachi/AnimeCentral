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

  updateFbLikeBox();
  // Slideshow Commands
  $(".next-slide").click(function () {
    var currentSlide = $(".anime-recom .slides .active");
    var slideIndex = $(currentSlide).attr("data-index");
    var nextSlideIndex;

    if (slideIndex == 4)
      nextSlideIndex = 1;
    else
      nextSlideIndex = parseInt(slideIndex) + 1;

    makeSlideActive(nextSlideIndex);
  })

  $(".prev-slide").click(function () {
    var currentSlide = $(".anime-recom .slides .active");
    var slideIndex = $(currentSlide).attr("data-index");
    var nextSlideIndex;

    if (slideIndex == 1)
      nextSlideIndex = 4;
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
  animeAddError.text("");
  $.get("/Anime/GetAnimeForm?" + "malId=" + malId + "&title=" + animeTitle, function (data) {
    $(".add-anime-partial .form-container").html(data);
  })
    .fail(function (data) {
      $(animeAddError).text(data.responseText);
  });
})

