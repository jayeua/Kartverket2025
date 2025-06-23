// Handles initializing the Leaflet map for report preview

function setPreviewMap(options) {
    // Extract options with defaults
    const {
        mapDivId = "previewMap",
        center = [58.1467, 7.9956],
        zoom = 13,
        tileLayerId = 1,
        reportAreaJson = null
    } = options;

    // Base layers and mapping
    const baseLayers = {
        "Topofarge": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Topogråtone": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Turkart": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Sjøkart": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Carto Light": L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', { attribution: '&copy; OpenStreetMap & CartoDB' }),
        "Carto Dark": L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', { attribution: '&copy; OpenStreetMap & CartoDB' })
    };
    const idToName = {
        1: "Topofarge",
        2: "Topogråtone",
        3: "Turkart",
        4: "Sjøkart",
        5: "Carto Light",
        6: "Carto Dark"
    };

    // Set up map and layer
    const map = L.map(mapDivId).setView(center, zoom);
    const layerName = idToName[tileLayerId] || "Topofarge";
    baseLayers[layerName].addTo(map);

    // Display GeoJSON if provided
    if (reportAreaJson && reportAreaJson !== "null" && reportAreaJson !== "") {
        try {
            let geoJsonObj = typeof reportAreaJson === 'string'
                ? JSON.parse(reportAreaJson)
                : reportAreaJson;
            if (geoJsonObj.type === "Feature") {
                geoJsonObj = {
                    type: "FeatureCollection",
                    features: [geoJsonObj]
                };
            }
            const geoLayer = L.geoJSON(geoJsonObj).addTo(map);
            if (geoLayer.getBounds && geoLayer.getBounds().isValid()) {
                map.fitBounds(geoLayer.getBounds(), { maxZoom: 30 });
            } else if (
                geoJsonObj.features &&
                geoJsonObj.features.length > 0 &&
                geoJsonObj.features[0].geometry.type === "Point"
            ) {
                const coords = geoJsonObj.features[0].geometry.coordinates;
                map.setView([coords[1], coords[0]], 30);
            }
        } catch (err) {
            console.error("GeoJSON parse error:", err, reportAreaJson);
        }
    }
}