const path = require('path');
const logger = require('winston');
var appRootDir = require('app-root-dir').get();

logger.addColors({
    debug: 'green',
    info:  'cyan',
    silly: 'magenta',
    warn:  'yellow',
    error: 'red'
});

logger.remove(logger.transports.Console);
if (process.env.NODE_ENV !== 'production') {
    logger.add(logger.transports.Console, { colorize:true, timestamp:true });
  }

logger.add(logger.transports.File, {
    filename: path.join(appRootDir, 'logs', '/logger.log'),
    maxsize: 10000000, // 10 MB
    maxFiles: 30 // Expected 1 month data to be archived
});

module.exports = logger;