const path = require('path');

module.exports = {
    entry:'./bin/www',
    output: {
      filename: 'bundle.js',
      path: path.resolve(__dirname, 'dist')
    },
  };