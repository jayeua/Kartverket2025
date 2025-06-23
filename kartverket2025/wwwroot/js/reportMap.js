// Main map logic: drawing, updating fields, connecting everything

document.addEventListener("DOMContentLoaded", function () {
    // Map setup
    var map = L.map('map').setView([58.1467, 7.9956], 13);

    // Init base layer manager
    setupLayerSwitcher(map, 'tileLayerSelect', 'tileLayerInput');

    // Drawing tools
    var shapesGroup = new L.FeatureGroup();
    map.addLayer(shapesGroup);

    var drawingTools = new L.Control.Draw({
        draw: {
            polygon: true,
            polyline: true,
            marker: true,
            rectangle: true,
            circle: false,
            circlemarker: false
        },
        edit: {
            featureGroup: shapesGroup,
            remove: true
        }
    });

    map.addControl(drawingTools);

    map.on(L.Draw.Event.CREATED, function (evt) {
        shapesGroup.clearLayers();
        shapesGroup.addLayer(evt.layer);

        var geojson = evt.layer.toGeoJSON();
        var geojsonStr = JSON.stringify(geojson);
        document.getElementById('geoJsonInput').value = geojsonStr;

        var centerPt = turf.centroid(geojson);
        var longitude = centerPt.geometry.coordinates[0];
        var latitude = centerPt.geometry.coordinates[1];

        document.getElementById('latitude').value = latitude;
        document.getElementById('longitude').value = longitude;

        lookupMunicipalityCounty(latitude, longitude,
            function (kommunenavn, fylkesnavn) {
                document.getElementById('municipalityInput').value = kommunenavn;
                document.getElementById('countryInput').value = fylkesnavn;
            },
            function () {
                document.getElementById('municipalityInput').value = '';
                document.getElementById('countryInput').value = '';
            }
        );
    });

    map.on('draw:deleted draw:edited', function () {
        document.getElementById('geoJsonInput').value = '';
        document.getElementById('latitude').value = '';
        document.getElementById('longitude').value = '';
        document.getElementById('municipalityInput').value = '';
        document.getElementById('countryInput').value = '';
    });
});