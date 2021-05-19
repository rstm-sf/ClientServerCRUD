const path = require("path");
const webpack = require("webpack");

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

const babelOptions = {
    presets: [
        ["@babel/preset-env", {
            "modules": false,
            "useBuiltIns": "usage",
        }]
    ]
};

module.exports = function (evn, argv) {
    const mode = argv.mode || "development";
    const isProduction = mode === "production";
    console.log("Webpack mode: " + mode);

    return {
        devtool: "source-map",
        entry: resolve('./src/Client/Client.fsproj'),
        output: {
            path: resolve('./src/Client/public'),
            filename: "bundle.js"
        },
        resolve: {
            modules: [ resolve("./node_modules")]
        },
        devServer: {
            proxy: {
                '/api/*': {
                    target: 'http://localhost:' + 5000,
                    changeOrigin: true
                }
            },
            hot: true,
            inline: true,
            contentBase: resolve('./src/Client/public'),
        },
        module: {
            rules: [
                {
                    test: /\.fs(x|proj)?$/,
                    use: "fable-loader"
                },
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: babelOptions
                    },
                }
            ]
        },
        plugins : isProduction ? [] : [
            new webpack.HotModuleReplacementPlugin(),
            new webpack.NamedModulesPlugin()
        ]
    }
};
