import Loader from '/js/utils/cp/loader.js';
class CartDetails {
    constructor() {
        this.loader = new Loader();
        this.deleteButtons = Array.from(document.querySelectorAll('[data-delete]'));

        this.deleteButtons.forEach(btn => {
            btn.addEventListener('click', e => {
                const id = +e.target.dataset.delete;
                this.deleteItem(id);
            });
        });
    }

    deleteItem(id) {
        let message = confirm(`Are you sure you want to delete this item from your cart?`);
        if (message == true) {
            this.loader.open();
            fetch(`/api/apiproducts/cart/delete/${id}`, {
                method: 'DELETE'
            }).then(() => {
                location.reload();
            }).catch(err => {
                this.loader.close();
                console.error(`${err.message}`);
            });
            
        }
    }
}

new CartDetails();