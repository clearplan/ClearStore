import { Product } from './common.js';
import Loader from '/js/utils/cp/loader.js';

class ProductEdit extends Product {
    constructor() {
        super();

        // delete the product
        this.deleteProductButton = document.querySelector('[data-delete]');

        this.deleteProductButton.addEventListener('click', (e) => {
            const id = +e.target.dataset.delete;
            this.deleteProduct(id);
        });

        // delete images
        this.deleteImageButtons = Array.from(document.querySelectorAll('[data-delete-image]'));

        if (this.deleteImageButtons.length > 0) {
            this.deleteImageButtons.forEach(btn => {
                const parent = btn.closest('.cp-frame');
                const id = +btn.dataset.deleteImage;
                btn.addEventListener('click', (e) => {
                    this.deleteImage(id, parent);
                })
            })
        }
    }

    deleteProduct(id) {
        let message = confirm(`Are you sure you want to delete this product?\nThis action cannot be reversed.`);
        if (message == true) {
            this.loader.open();

            fetch(`/api/apiproducts/product/delete/${id}`, { method: 'DELETE' }).then((res) => {
                if (!res.ok) {
                    this.loader.close();
                    throw new Error('Error: There was an error deleting this product');
                }
                return res.json();
            }).then(data => {
                if (data.success == true) {
                    location.reload();
                }
            }).catch(err => {
                this.loader.close();
                // show error popover
            });
        }
    }

    deleteImage(id, parent) {
        let message = confirm(`Are you sure you want to delete this image?\nThis action cannot be reversed.`);
        if (message == true) {
            this.loader.open();

            fetch(`/api/apiproducts/image/delete/${id}`, { method: 'DELETE' }).then((res) => {
                if (!res.ok) {
                    this.loader.close();
                    throw new Error('Error: There was an error deleting this product order');
                }
                return res.json();
            }).then(data => {
                if (data.success == true) {
                    // delete the image parent container
                    if (parent) {
                        this.loader.close();
                        parent.remove();
                    }
                }
            }).catch(err => {
                this.loader.close();
                // show error popover
            });
        }
    }
}

new ProductEdit();