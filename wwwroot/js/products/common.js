import Loader from '/js/utils/cp/loader.js';

export class Product {
    constructor() {
        this.loader = new Loader();

        this.isApparel = document.querySelector('[data-isapparel]');
        this.gender = document.querySelector('[data-gender]');
        this.colorCategory = document.querySelector('[data-color-category]');

        this.genderValue = this.gender.value;
        this.colorCategoryValue = this.colorCategory.value;

        this.disabled = true;

        this.isApparel.addEventListener('change', e => {
            this.updateState(e.target.value);
        });

        if (this.isApparel) {
            this.updateState(this.isApparel.value);
        }
    }

    updateState(value) {
        if (value == 'true') {
            this.disabled = false;
            this.gender.value = this.genderValue;
            this.colorCategory.value = this.colorCategoryValue;
        }
        else {
            this.disabled = true;
            this.gender.selectedIndex = 0;
            this.colorCategory.selectedIndex = 0;
        }

        this.gender.disabled = this.disabled;
        this.colorCategory.disabled = this.disabled;
    }
}
