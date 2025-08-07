
function getChartColorsArray(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        const colors = JSON.parse(element.getAttribute("data-colors"));
        return colors.map(color => {
            const cssVar = getComputedStyle(document.documentElement).getPropertyValue(color);
            return cssVar || color;
        });
    } else {
        console.warn(`Element with ID '${elementId}' not found.`);
        return [];
    }
}

// Lấy idExam từ URL
const currentURL = window.location.href; // URL hiện tại
const idExam = currentURL.split("/").pop(); // Lấy giá trị idExam từ phần cuối của URL

// Thiết lập dữ liệu và render biểu đồ cột
const colors = getChartColorsArray("audiences_metrics_charts");
if (colors) {

    // Lấy dữ liệu từ server qua API (nếu có)
    fetch("/Admin/Exam/ListStudentOfTakingExam/" + idExam)
        .then(response => response.json())
        .then(data => {
            const options = {
                series: [
                    {
                        name: "Scores",
                        data: data.scores, // Dữ liệu từ server
                    },
                ],
                chart: { type: "bar", height: 350, toolbar: { show: false } },
                plotOptions: {
                    bar: { horizontal: false, columnWidth: "50%", borderRadius: 5 },
                },
                xaxis: {
                    categories: ["<= 1", "<= 2", "<= 3", "<= 4", " <=5", "<= 6", "<= 7", "<= 8", "<= 9", "<= 10"], // Trục x từ 1 đến 10
                },
                colors: colors,
                dataLabels: { enabled: true },
                yaxis: { title: { text: "Số lượng học sinh" } },
                grid: { borderColor: "#f1f1f1" },
            };

            const chart = new ApexCharts(document.querySelector("#audiences_metrics_charts"), options);
            chart.render();
        })
        .catch(error => console.error("Lỗi khi tải dữ liệu:", error));
}







