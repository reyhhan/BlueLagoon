function loadRadialBarChart(id, data) {
    var chartcolors = getChartColorsArray(id);

    var options = {
        fill: {
            colors: chartcolors
        },
        chart: {
            height: 90,
            width: 90,
            type: "radialBar",
            offsetY: -10,
            sparkline: {
                enabled: true
            }
        },
        series: data.series,
        plotOptions: {
            radialBar: {
                dataLabels: {
                    value: {
                        offsetY: -10
                    }
                }
            }
        },
        labels: [""]
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);

    chart.render();

}

function getChartColorsArray(id) {
    if (document.getElementById(id) !== null) {
        var colors = document.getElementById(id).getAttribute("data-colors");

        if (colors) {
            colors = JSON.parse(colors);
            return colors.map(function (value) {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) return color;
                    else return newValue;;
                }
            });
        }
    }
}