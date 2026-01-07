// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    var sidebarEl = document.getElementById('studentsSidebar');
    if (!sidebarEl) return;

    // Bootstrap collapse emits events on the element
    // we listen for show/hide to toggle body class
    sidebarEl.addEventListener('show.bs.collapse', function () {
        document.body.classList.add('sidebar-open');
    });
    sidebarEl.addEventListener('shown.bs.collapse', function () {
        // focus first focusable element optionally
        var btn = sidebarEl.querySelector('a,button,input,select,textarea');
        if (btn) btn.focus();
    });
    sidebarEl.addEventListener('hide.bs.collapse', function () {
        document.body.classList.remove('sidebar-open');
    });
    sidebarEl.addEventListener('hidden.bs.collapse', function () {
        document.body.classList.remove('sidebar-open');
    });

    // clicking backdrop area (body) when sidebar open should close it
    document.addEventListener('click', function (e) {
        if (!document.body.classList.contains('sidebar-open')) return;

        // if click outside sidebar, close it
        var inside = sidebarEl.contains(e.target);
        var toggleBtn = document.querySelector('[data-bs-target="#studentsSidebar"]');
        if (!inside && toggleBtn) {
            // use Bootstrap Collapse API to hide
            var bsCollapse = bootstrap.Collapse.getOrCreateInstance(sidebarEl);
            bsCollapse.hide();
        }
    }, true);
});
