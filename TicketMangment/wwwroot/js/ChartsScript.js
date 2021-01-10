function LoadData() {
    $.ajax({

        url: '/ticket/PopulationChart',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (data) {
            PopulationChart(data);
            return false;
        }
    });
    return false;
}

function PopulationChart(data) {
    var ctx = document.getElementById('ticketsChart').getContext('2d');
    //console.log(data);
    //var array = data.Labels;
    //console.log(data.labels);
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [{
                label: 'Solved Tickets',
                data: data.solvedTicketsValues,
                backgroundColor: '#fff',
                borderColor: "transparent",
                pointRadius: "0",
                borderWidth: 3
            }, {
                label: 'All Tickets',
                data: data.allTicketsValues,
                backgroundColor: "rgba(255, 255, 255, 0.25)",
                borderColor: "transparent",
                pointRadius: "0",
                borderWidth: 1
            }]
        },
        options: {
            maintainAspectRatio: false,
            legend: {
                display: false,
                labels: {
                    fontColor: '#ddd',
                    boxWidth: 40
                }
            },
            tooltips: {
                displayColors: false
            },
            scales: {
                xAxes: [{
                    ticks: {
                        beginAtZero: true,
                        fontColor: '#ddd'
                    },
                    gridLines: {
                        display: true,
                        color: "rgba(221, 221, 221, 0.08)"
                    },
                }],
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        fontColor: '#ddd'
                    },
                    gridLines: {
                        display: true,
                        color: "rgba(221, 221, 221, 0.08)"
                    },
                }]
            }

        }
    });
}

// chart 2

function LoadDataChart2() {
    $.ajax({

        url: '/ticket/FeedChart',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (data) {
            PopulationChart2(data);
            return false;
        }
    });
    return false;
}
function PopulationChart2(data) {
    var ctx = document.getElementById("departmentChart").getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: data.labels,
            datasets: [{
                backgroundColor: [
                    "#ffffff",
                    "rgba(255, 255, 255, 0.70)",
                    "rgba(255, 255, 255, 0.50)",
                    "rgba(255, 255, 255, 0.20)"
                ],
                data: data.ticketsCount,
                borderWidth: [0, 0, 0, 0]
            }]
        },
        options: {
            maintainAspectRatio: false,
            legend: {
                position: "bottom",
                display: false,
                labels: {
                    fontColor: '#ddd',
                    boxWidth: 15
                }
            }
            ,
            tooltips: {
                displayColors: false
            }
        }
    });
}