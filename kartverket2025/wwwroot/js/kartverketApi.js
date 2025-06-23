// Looks up municipality and county via Kartverket API for a given coordinate

function lookupMunicipalityCounty(lat, lon, onSuccess, onFail) {
    const endpoint = `https://api.kartverket.no/kommuneinfo/v1/punkt?nord=${lat}&ost=${lon}&koordsys=4258`;
    fetch(endpoint)
        .then(response => response.json())
        .then(data => {
            if (data && data.kommunenavn && data.fylkesnavn) {
                onSuccess(data.kommunenavn, data.fylkesnavn);
            } else {
                onFail();
            }
        })
        .catch(onFail);
}