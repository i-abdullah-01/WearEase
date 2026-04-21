$(document).ready(function () {
    let timeout = null;
    let lastSearchTerm = "";

    $("#searchInput").on("keyup", function (e) {
        clearTimeout(timeout);

        let query = $(this).val().trim();

        // If Enter key is pressed, navigate to search results page
        if (e.key === "Enter" && query.length >= 2) {
            window.location.href = `/Product/Search?term=${encodeURIComponent(query)}`;
            return;
        }

        if (query.length < 2) {
            $("#searchResults").addClass("d-none").empty();
            return;
        }

        timeout = setTimeout(function () {
            // Store the search term for potential page navigation
            lastSearchTerm = query;

            $.ajax({
                url: "/Product/Search",
                type: "GET",
                data: { term: query },
                success: function (data) {
                    let resultBox = $("#searchResults");
                    resultBox.empty();

                    if (data.length === 0) {
                        resultBox
                            .append(`<div class="list-group-item text-muted">No products found</div>`)
                            .removeClass("d-none");
                        return;
                    }

                    // Add "View All Results" link at the top
                    if (data.length > 0) {
                        resultBox.append(`
                            <a href="/Product/Search?term=${encodeURIComponent(query)}" 
                               class="list-group-item list-group-item-warning fw-bold d-flex justify-content-between align-items-center">
                                View All Results (${data.length})
                                <i class="bi bi-arrow-right"></i>
                            </a>
                        `);
                    }

                    // Add individual product results
                    data.forEach(p => {
                        resultBox.append(`
                            <a href="/Product/Details/${p.id}"
                               class="list-group-item list-group-item-action d-flex align-items-center">
                                <img src="${p.imageUrl}" width="40" height="40" class="me-2 rounded object-fit-cover">
                                <div class="flex-grow-1">
                                    <div class="fw-semibold">${p.name}</div>
                                    <small class="text-muted">$ ${p.price}</small>
                                </div>
                            </a>
                        `);
                    });

                    resultBox.removeClass("d-none");
                }
            });
        }, 300); // debounce
    });

    // Close dropdown when clicking outside
    $(document).click(function (e) {
        if (!$(e.target).closest("#searchInput, #searchResults").length) {
            $("#searchResults").addClass("d-none");
        }
    });
});