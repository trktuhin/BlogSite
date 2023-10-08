function initializeTooltip() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        const tooltip = new bootstrap.Tooltip(tooltipTriggerEl, {
            trigger: "manual",
        });

        let tooltipTimeout;
        let currentToolTip;

        tooltipTriggerEl.addEventListener("mouseenter", function () {
            let toolTipID;

            // Clear Set Timeout
            clearTimeout(tooltipTimeout);

            // Show Tooltip
            tooltip.show();

            // Assign current tooltip ID to toolTipID variable
            toolTipID = tooltipTriggerEl.getAttribute("aria-describedby");

            // Assign current tooltip to currentToolTip variable
            currentToolTip = document.querySelector(`#${toolTipID}`);

            // Hide tooltip on tooltip mouse leave
            currentToolTip.addEventListener("mouseleave", function () {
                tooltip.hide();
            });
        });


        tooltipTriggerEl.addEventListener("mouseleave", function () {
            // SetTimeout before tooltip disappears
            tooltipTimeout = setTimeout(function () {
                // Hide tooltip if not hovered.
                if (!currentToolTip.matches(":hover")) {
                    tooltip.hide();
                }
            }, 100);
        });

        return tooltip;
    });
}

function setActiveSideNav(id) {
    // Remove "active" class from all elements with class name "nav-link"
    var navLinks = document.querySelectorAll('#sidebar a.nav-link');
    for (var i = 0; i < navLinks.length; i++) {
        navLinks[i].classList.remove("active");
    }

    // Add "active" class to the element with the specified ID
    var activeLink = document.getElementById(id);
    if (activeLink) {
        activeLink.classList.add("active");
    }
}

function showModal(id) {
    var myModal = new bootstrap.Modal(document.getElementById(id), {});
    myModal.show();
}

function hideModal(id) {
    var myModalEl = document.getElementById(id);
    var modal = bootstrap.Modal.getInstance(myModalEl)
    modal.hide();
}