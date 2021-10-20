const path = require('path');

module.exports = {
    entry: {
        admin: './Assets/Js/index.ts',
    },
    mode: 'development',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
        alias: {
            vue$: 'vue/dist/vue.esm.js',
        },
    },
    externals: {
        bootstrap: 'bootstrap',
        jquery: 'jQuery',
        vue: 'Vue',
    },
    output: {
        filename: 'admin.js',
        path: path.resolve(__dirname, './wwwroot/Scripts/'),
    }
};