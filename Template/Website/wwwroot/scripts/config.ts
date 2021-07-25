declare var requirejs: any;
requirejs.config({
    baseUrl: '/',
    paths: {
        "app": "lib/olive.microservices.hubjs/dist/bundle-built",
    },
    bundles: {
        "app": [
            "lib/olive.microservices.hubjs/dist/bundle-built"
        ]
    },
    shim: {
        "scripts/appPage": ["app"]
    }
});

requirejs(["scripts/appPage"], function (hub) { })
