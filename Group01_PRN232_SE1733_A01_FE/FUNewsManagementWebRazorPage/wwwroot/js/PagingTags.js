document.addEventListener("DOMContentLoaded", function () {
    let tableBody = document.getElementById("tableTag");
    let rows = Array.from(tableBody.getElementsByTagName("tr"));
    let itemsPerPage = 3
    let currentPage = 1;

    function renderTable(page) {
        let start = (page - 1) * itemsPerPage;
        let end = start + itemsPerPage;

        rows.forEach((row, index) => {
            row.style.display = (index >= start && index < end) ? "table-row" : "none";
        });

        updatePagination();
    }

    function updatePagination() {
        let paginationContainer = document.getElementById("pagination");
        paginationContainer.innerHTML = "";

        let totalPages = Math.ceil(rows.length / itemsPerPage);

        for (let i = 1; i <= totalPages; i++) {
            let btn = document.createElement("button");
            btn.innerText = i;
            btn.className = "btn btn-sm " + (i === currentPage ? "btn-primary" : "btn-light");
            btn.onclick = function () {
                currentPage = i;
                renderTable(currentPage);
            };
            paginationContainer.appendChild(btn);
        }
    }

    let paginationDiv = document.createElement("div");
    paginationDiv.id = "pagination";
    paginationDiv.className = "mt-3 d-flex justify-content-center";

    document.querySelector("table").after(paginationDiv);

    renderTable(currentPage);
});