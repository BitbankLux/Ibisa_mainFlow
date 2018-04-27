const express = require('express');
const bodyParser = require('body-parser');
const app = express();
const path = require('path');

app.use(function (req, res, next) {
  res.header("Access-Control-Allow-Origin", "*");
  res.header("Access-Control-Allow-Header", "Origin, X-Requested-With, Content-Type, Accept");
  next();
});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

require('app-root-dir').set(__dirname);

// Hanlde system level exceptions.
var logger = require('./public/javascripts/logging/log');

process.on('unhandledRejection', (reason, p) => {
  logger.error('Unhandled Rejection at:', p, 'reason:', reason);
  // application specific logging, throwing an error, or other logic here
});

// catch the uncaught errors that weren't wrapped in a domain or try catch statement
// do not use this in modules, but only in applications, as otherwise we could have multiple of these bound
process.on('uncaughtException', function(err) {
	// handle the error safely
	logger.error("uncaughtException : " + err);
});

require('./routes')(app);

module.exports = app;