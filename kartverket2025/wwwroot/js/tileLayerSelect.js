// Handles base layer logic and switching for the map

function getLayerDictionary() {
    return {
        "Topofarge": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Topogråtone": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Turkart": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Sjøkart": L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; Kartverket' }),
        "Carto Light": L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', { attribution: '&copy; OpenStreetMap & CartoDB' }),
        "Carto Dark": L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', { attribution: '&copy; OpenStreetMap & CartoDB' })
    };
}

function getLayerNameFromId(id) {
    const lookup = {
        1: "Topofarge",
        2: "Topogråtone",
        3: "Turkart",
        4: "Sjøkart",
        5: "Carto Light",
        6: "Carto Dark"
    };
    return lookup[parseInt(id)] || "Topofarge";
}

function setupLayerSwitcher(map, selectId, inputId) {
    const baseLayers = getLayerDictionary();
    let activeLayerName = getLayerNameFromId(document.getElementById(selectId).value);
    let activeLayer = baseLayers[activeLayerName];
    activeLayer.addTo(map);

    document.getElementById(inputId).value = document.getElementById(selectId).value;

    document.getElementById(selectId).addEventListener('change', function () {
        map.removeLayer(activeLayer);
        activeLayerName = getLayerNameFromId(this.value);
        activeLayer = baseLayers[activeLayerName];
        map.addLayer(activeLayer);
        document.getElementById(inputId).value = this.value;
    });

    // Optional: Return baseLayers object if you need to use it elsewhere
    return baseLayers;
}