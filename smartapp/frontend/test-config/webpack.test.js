var webpack = require('webpack');
var path = require('path');
var ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

module.exports = {
  devtool: 'inline-source-map',

  resolve: {
    extensions: ['.ts', '.js']
  },

  module: {
    rules: [{
      test: /\.ts$/,
      loaders: [{
        loader: 'ts-loader',
        options: {
            transpileOnly: true
        }
      }, 'angular2-template-loader']
    },
    {
      test: /\.html$/,
      loader: 'html-loader?attrs=false'
    },
    {
      test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
      loader: 'null-loader'
    }
    ]
  },

  plugins: [
    new webpack.ContextReplacementPlugin(
      /(ionic-angular)|(angular(\\|\/)core(\\|\/)@angular)/,
      root('../src/'),
      {}
    ),
    new ForkTsCheckerWebpackPlugin()
  ]
};

function root(localPath) {
  return path.resolve(__dirname, localPath);
}
