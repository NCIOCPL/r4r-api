function fn() {
    var config = {
        apiHost: 'http://localhost:5000',
        esHost: 'http://localhost:9200'
    };
    if (java.lang.System.getenv('KARATE_APIHOST')) {
        config.apiHost = java.lang.System.getenv('KARATE_APIHOST');
    }
    if (java.lang.System.getenv('KARATE_ESHOST')) {
        config.esHost = java.lang.System.getenv('KARATE_ESHOST');
    }
    return config;
}