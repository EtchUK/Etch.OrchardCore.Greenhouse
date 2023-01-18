const RemoveEmptyScriptsPlugin = require('webpack-remove-empty-scripts');
const ESLintPlugin = require('eslint-webpack-plugin');
const path = require('path');

module.exports = [
    {
        entry: {
            greenhouse: path.join(process.cwd(), 'Assets/Scripts/index.ts'),
        },
        output: {
            path: path.join(process.cwd(), 'wwwroot/js'),
            filename: '[name].js',
        },
        module: {
            rules: [
                {
                    test: /(\.ts(x?)$)/,
                    exclude: /node_modules/,
                    use: [
                        {
                            loader: 'babel-loader',
                            options: {
                                presets: [
                                    [
                                        '@babel/preset-env',
                                        {
                                            corejs: {
                                                version: 3,
                                                proposals: true,
                                            },
                                            useBuiltIns: 'entry',
                                        },
                                    ],
                                ],
                            },
                        },
                        {
                            loader: 'ts-loader',
                            options: {
                                configFile: path.join(
                                    process.cwd(),
                                    './tsconfig.json'
                                ),
                            },
                        },
                    ],
                },
                {
                    test: /(\.js(x?)$)/,
                    exclude: /node_modules/,
                    use: ['babel-loader'],
                },
            ],
        },
        plugins: [
            new ESLintPlugin({
                extensions: ['ts'],
            }),
            new RemoveEmptyScriptsPlugin(),
        ],
        resolve: {
            extensions: ['.tsx', '.ts', '.js'],
        },
    },
];
