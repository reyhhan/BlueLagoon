$(document).ready(function () {
    loadBookingLineChart();
});

function loadBookingLineChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetMemberAndBookingLineBarChart",
        type: 'GET',
        dataType: 'json',
        success: function (data) {          
            console.log(data);

            loadLineChart("newMemberAndBookingLineChart", data)
            $(".chart-spinner").hide();
        }
    });
}

function loadLineChart(id, data) {
   
    let chartColors = getChartColorsArray(id);


    var options = {
        colors: chartColors,
        series: data.series,

        chart: {
            height: 350,
            type: 'line',
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: [5, 7, 5],
            curve: 'smooth',
            dashArray: [0, 8, 5]
        },
        markers: {
            size: 6,
            strokeWidth: 0,
            hover: {
                size: 7
            }
        },
        xaxis: {
            categories: data.categories,
            labels: {
                style: {
                    colors: "#fff",
                },
            },
        },
        yaxis: {
            labels: {
                style: {
                    colors: "#fff",
                },
            },
        },

        grid: {
            borderColor: '#f1f1f1',
        },
        legend: {
            labels: {
                colors: "#fff",
            },
        },
        tooltip: {
            theme: 'dark'
        }

    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();

}