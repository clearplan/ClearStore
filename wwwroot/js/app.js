import CP from '../js/utils/cp/cp.js';

class App {
    constructor() {
        this.cp = new CP();
        this.cp.postGuard();

        // layout
        this.layout = document.querySelector('[data-layout]');
        this.layoutHeader = document.querySelector('[data-layout-header]');
        this.layoutViewport = document.querySelector('[data-layout-viewport]');
        this.layoutSidebar = document.querySelector('[data-layout-sidebar]')

        // controllers
        this.toggleLayoutSidebarButton = document.querySelector('[data-toggle-layout-sidebar]');

        this.init();
    }

    init() {
        this.toggleLayoutSidebarButton.addEventListener('click', () => {
            this.layout.classList.toggle('collapsed');
        });
    }
}

new App();