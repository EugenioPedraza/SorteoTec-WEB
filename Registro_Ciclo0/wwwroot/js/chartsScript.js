
function createChart(chartId, chartData, chartType, options) {
    const ctx = document.getElementById(chartId).getContext('2d');
    const defaultOptions = {
        legend: {
            display: false,
            onClick: function (e) {
                e.stopPropagation();
            }
        },

    };


    const finalOptions = Object.assign({}, defaultOptions, options);
    return new Chart(ctx, {
        type: chartType,
        data: chartData,
        options: finalOptions
    });
}

function prepareBarChartData(data) {

    if (typeof data === 'string') {
        data = JSON.parse(data);
    }

    return {
        labels: Object.keys(data),
        datasets: [{
            label: 'Data',
            data: Object.values(data),
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)'
            ],
            borderWidth: 1
        }]
    };
}



function initCharts() {

    const commonOptions = {
        legend: {
            display: false  
        },
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        }
    };


    if (typeof window.genderChartData !== 'undefined' && window.genderChartData !== null) {
        createChart('genderChart', prepareBarChartData(window.genderChartData), 'bar', commonOptions);
    }

    if (typeof window.ageChartData !== 'undefined' && window.ageChartData !== null) {
        createChart('ageChart', prepareBarChartData(window.ageChartData), 'bar', commonOptions);
    }
}

document.addEventListener('DOMContentLoaded', initCharts);
