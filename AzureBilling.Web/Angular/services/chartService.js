var app = angular.module('app');
app.factory('chartService', function () {

    var _drawHalfPieChart = function (name,chartData) {
        Highcharts.chart(name, {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: 0,
                plotShadow: false
            },
            title: {
                text: chartData.title,
                align: 'left',
                verticalAlign: 'top',
            },
            subtitle: {
                text: chartData.totalCostSummary,
                align: 'center',
                verticalAlign: 'middle',
                y: 40
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    dataLabels: {
                        enabled: false,
                        distance: -50,
                        style: {
                            fontWeight: 'bold',
                            color: 'white',
                            textShadow: '0px 1px 2px black'
                        }
                    },
                    startAngle: -90,
                    endAngle: 90,
                    center: ['50%', '75%']
                }
            },
            series: [{
                type: 'pie',
                name: 'Total Expense',
                innerSize: '50%',
                data: chartData.data
            }],
            exporting: { enabled: false }
        });
    };
    var _drawlineChart = function (name,data) {
            Highcharts.chart(name, {
                chart: {
                    type: 'line'
                },
                title: {
                    text: data.title
                },
                xAxis: {
                    categories: data.categories

                },
                yAxis: {
                    labels: {
                        format: '$ {value}'
                    }
                },
                legend: {
                    layout: 'vertical',
                    floating: true,
                    backgroundColor: '#FFFFFF',
                    align: 'left',
                    verticalAlign: 'top',
                    y: 60,
                    x: 90
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.series.name + '</b><br/>' +
                            this.x + ': ' + this.y;
                    }
                },
                
                series:data.series,
                exporting: { enabled: false }
            });
        
    }

    var _drawPieChart = function (name, chartData) {
        Highcharts.chart(name, {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: chartData.title
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                        style: {
                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                        }
                    }
                }
            },
            series: [{
                name: 'Brands',
                colorByPoint: true,
                data: chartData.data
            }],
            exporting: { enabled: false }
        });
    }

    var _drawBarChart = function (name,chartData) {
        Highcharts.chart(name, {
            chart: {
                type: 'column'
            },
            title: {
                text: chartData.title
            },
            xAxis: {
                type: 'category'
            },
            yAxis: {
                title: {
                    text: 'Total expense'
                }

            },
            legend: {
                enabled: false
            },
            plotOptions: {
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '${point.y:.2f}'
                    }
                }
            },

            tooltip: {
                headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> of total<br/>'
            },

            series: [{
                name: 'Brands',
                colorByPoint: true,
                data: chartData.data
            }],
            exporting: { enabled: false }
        });
    }

    var _drawMapChart = function(name,chartData)
    {
        var mapGeoJSON = Highcharts.maps[''],
                data = [],
                parent,
                match;
        Highcharts.chart(name, {
            chart: {
                type: 'Map'
            },

            title: {
                    text: null
            },

            mapNavigation: {
                enabled: true
            },

            colorAxis: {
                min: 0,
                stops: [
                    [0, '#EFEFFF'],
                    [0.5, Highcharts.getOptions().colors[0]],
                    [1, Highcharts.Color(Highcharts.getOptions().colors[0]).brighten(-0.5).get()]
                ]
            },

            legend: {
                layout: 'vertical',
                align: 'left',
                verticalAlign: 'bottom'
            },

            series: [{
                data: data,
                mapData: Highcharts.maps['custom/world'],
                joinBy: ['hc-key', 'key'],
                name: 'Random data',
                states: {
                    hover: {
                        color: Highcharts.getOptions().colors[2]
                    }
                },
                dataLabels: {
                    enabled: showDataLabels,
                    formatter: function () {
                        return mapKey === 'custom/world' || mapKey === 'countries/us/us-all' ?
                                (this.point.properties && this.point.properties['hc-a2']) :
                                this.point.name;
                    }
                },
                point: {
                    events: {
                        // On click, look for a detailed map
                        click: function () {
                            var key = this.key;
                            $('#mapDropdown option').each(function () {
                                if (this.value === 'countries/' + key.substr(0, 2) + '/' + key + '-all.js') {
                                    $('#mapDropdown').val(this.value).change();
                                }
                            });
                        }
                    }
                }
            }, {
                type: 'mapline',
                name: "Separators",
                data: Highcharts.geojson(mapGeoJSON, 'mapline'),
                nullColor: 'gray',
                showInLegend: false,
                enableMouseTracking: false
            }]
        });
    }
    return { drawLineChart: _drawlineChart ,
        drawHalfPieChart:_drawHalfPieChart,
        drawPieChart: _drawPieChart,
        drawBarChart: _drawBarChart
    };

});