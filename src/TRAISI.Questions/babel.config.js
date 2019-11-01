module.exports = function (api) {
	api.cache(true);

	const presets = [
		[
			'@babel/preset-typescript',
			{
				exclude: ['node_modules/mapbox-gl/*'],
				targets: {
					browsers: [ "> 1%","last 2 versions", "IE 11", "not dead"]
				}
			}
		]
	];
	const plugins = [
		"babel-plugin-transform-typescript-metadata",
		[
			"@babel/plugin-proposal-decorators",
			{
				"legacy": true,
			}
		],
		"@babel/plugin-transform-block-scoping",
		["@babel/plugin-proposal-class-properties", { "loose": true }]
	];

	return {
		presets,
		plugins
	};
};
