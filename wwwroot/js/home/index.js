import Loader from '/js/utils/cp/loader.js';

class Home {
    constructor() {
        this.loader = new Loader();
        this.loader.open();

        // products
        this.products = Array.from(document.querySelectorAll('[data-product]'));

        // apparel filters
        this.genders = Array.from(document.querySelectorAll('[data-gender]'));

        if (this.genders) {
            this.genders.forEach(checkbox => {
                checkbox.addEventListener('change', () => {
                    this.filterProducts();
                });
            });
        }

        // item has been successfully added from details
        this.itemAdded = document.querySelector('[data-item-added]');

        if (this.itemAdded) {
            this.itemAdded.showPopover();
        }

        // order has been completed
        this.orderComplete = document.querySelector('[data-order-complete]');

        if (this.orderComplete) {
            this.orderComplete.showModal();
        }

        window.addEventListener('load', this.loader.close());
    }

    filterProducts() {
        const selectedGenderTypes = this.genders
            .filter(d => d.checked)
            .map(d => +d.value);

        this.products.forEach(product => {
            const genderType = +product.getAttribute('data-gender-type');

            if (!selectedGenderTypes.includes(genderType)) {
                product.style.display = 'none';
            }
            else {
                product.style.display = 'block';
            }
        });
    }

    async getData(id) {
        try {
            const response = await fetch(`/api/apiproducts/product/${id}`, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }

            const product = await response.json();
            this.productDetails = product;

            this.loadPreview.call(this);
        }
        catch (error) {
            console.error(`Error: ${error.message}`);
        }
    }
}

new Home();