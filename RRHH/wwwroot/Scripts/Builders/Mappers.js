
const Mappers = function () {

    // -- MAPPERS DE NÚMEROS
    // Convertir un número a currency
    const NumberToCurrency = function (number) {
        return parseInt(number).toLocaleString('es-CL', { style: 'decimal', maximumFractionDigits: 0 })
    }

    return {
        FormatCurrency: function (number) {
            return NumberToCurrency(number);
        }
    }
}();
