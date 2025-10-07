import Loader from '/js/utils/cp/loader.js';
class Order {
    constructor(opts = {}) {
        this.loader = new Loader();

        // delete items from cart
        this.deleteItemButtons = Array.from(document.querySelectorAll('[data-item]'));
        this.deleteItemButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                const id = +btn.dataset.item;
                this.deleteItem(id);
            });
        });

        // delete the order
        this.deleteOrderButton = document.querySelector('[data-delete]');
        this.deleteOrderButton.addEventListener('click', e => {
            const id = +e.target.dataset.delete;
            this.deleteOrder(id);
        });
    }

    deleteItem(id) {
        let message = confirm(`Are you sure you want to delete this item?\nThis operation cannot be reversed.`);
        if (message == true) {
            this.loader.open();
            fetch(`/api/apiproducts/order-item/delete/${id}`, { method: 'DELETE' }).then((res) => {
                if (!res.ok) {
                    this.loader.close();
                    throw new Error('Error: There was an error deleting this item');
                }
                return res.json();
            }).then(data => {
                if (data.success == true) {
                    location.reload();
                }
            }).catch(err => {
                this.loader.close();
                console.error(err.message)
            });
        }
    }

    deleteOrder(id) {
        let message = confirm(`Are you sure you want to delete this order?\nThis operation cannot be reversed.`);
        if (message == true) {
            this.loader.open();
            fetch(`/api/apiproducts/orders/delete/${id}`, { method: 'DELETE' }).then((res) => {
                if (!res.ok) {
                    this.loader.close();
                    throw new Error('Error: There was an error deleting this product order');
                }
                return res.json();
            }).then(data => {
                if (data.success == true) {
                    location.reload();
                }
            }).catch(err => {
                this.loader.close();
                console.error(err.message)
            });
        }
    }
}

new Order();