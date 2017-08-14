"use strict";

var gulp = require("gulp"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  htmlmin = require("gulp-htmlmin"),
  uglify = require("gulp-uglify"),
  merge = require("merge-stream"),
  del = require("del"),
  gutil = require('gulp-util'),
  bundleconfig = require("./bundleconfig.json"),
  less = require('gulp-less');
var regex = {
  css: /\.css$/,
  html: /\.(html|htm)$/,
  js: /\.js$/,
  less: /\.less$/
};

gulp.task("min", ["min:js", "min:css", "min:html", "less"]);

gulp.task("min:js", function () {
  var tasks = getBundles(regex.js).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(uglify())
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
});

gulp.task("min:css", function () {
  var tasks = getBundles(regex.css).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(cssmin())
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
});

gulp.task("min:html", function () {
  var tasks = getBundles(regex.html).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
});

gulp.task('less', function (file) {
  gutil.log(this);
  var tasks = getBundles(regex.less).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(less())
      .pipe(cssmin())
      .pipe(gulp.dest('.'));
  })
  return merge(tasks);
});

gulp.task("clean", function () {
  var files = bundleconfig.map(function (bundle) {
    return bundle.outputFileName;
  });

  return del(files);
});

gulp.task("watch", function () {
  getBundles(regex.js).forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:js"]);
  });

  getBundles(regex.css).forEach(function (bundle) {
    var cssFiles = bundle.inputFiles.filter(function (file) {
      return regex.css.test(file);
    });
    var lessFiles = bundle.inputFiles.filter(function (file) {
      return regex.less.test(file);
    });

    gulp.watch(cssFiles, function () {
      gutil.log("Css merge started.");
      gulp.src(cssFiles, { base: "." })
        .pipe(concat(bundle.outputFileName))
        .pipe(cssmin())
        .pipe(gulp.dest('.'));
      gutil.log("Css merge ended.");
    });

    gulp.watch(lessFiles, function () {
      gutil.log("Less merge started.");
      gulp.src(lessFiles, { base: "." })
        .pipe(concat(bundle.outputFileName))
        .pipe(less())
        .pipe(cssmin())
        .pipe(gulp.dest('.'));
      gutil.log("Less merge ended.");
    });

    //bundle.inputFiles.forEach(function (file) {
    //  if (regex.less.test(file)) {
    //    gulp.watch(file, function () {
    //      gulp.src(file, { base: "." })
    //        .pipe(concat(bundle.outputFileName))
    //        .pipe(less())
    //        .pipe(cssmin())
    //        .pipe(gulp.dest('.'));
    //    });
    //  } else {
    //    gulp.watch(file, function () {
    //      gulp.src(file, { base: "." })
    //        .pipe(concat(bundle.outputFileName))
    //        .pipe(cssmin())
    //        .pipe(gulp.dest('.'));
    //    });
    //}
  });

  getBundles(regex.html).forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:html"]);
  });

  getBundles(regex.less).forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["less"]);
  });
});

function getBundles(regexPattern) {
  return bundleconfig.filter(function (bundle) {
    return regexPattern.test(bundle.outputFileName);
  });
}