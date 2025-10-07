import Loader from '/js/utils/cp/loader.js';
class Order {
    constructor(opts = {}) {
        this.loader = new Loader();

        this.zipcode = document.querySelector('[data-zipcode]');
        this.phoneNumber = document.querySelector('[data-phonenumber]');

        // clamp the zip code to 5 digits
        this.zipcode.addEventListener('input', () => {
            this.zipcode.value = this.zipcode.value.replace(/\D/g, '').slice(0, 5);
        });

        // clamp the phone number to 9 digits
        this.phoneNumber.addEventListener('input', () => {
            this.phoneNumber.value = this.phoneNumber.value.replace(/\D/g, '').slice(0, 10);
        });

        this.deleteButtons = Array.from(document.querySelectorAll('[data-delete]'));

        this.deleteButtons.forEach(btn => {
            btn.addEventListener('click', e => {
                const id = +e.target.dataset.delete;
                this.deleteItem(id);
            });
        });
    }

    deleteItem(id) {
        let message = confirm(`Are you sure you want to delete this item?\nThis action cannot be reversed.`);
        if (message == true) {
            this.loader.open();
            fetch(`/api/apiproducts/cart/delete/${id}`, { method: 'DELETE' })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        if (data.totalCartItems == 0) {
                            location.reload();
                        }
                        else {
                            const cartItem = document.querySelector(`[data-item="${id}"]`);
                            cartItem?.remove();
                            this.loader.close();
                        }
                    }
                    else {
                        console.error('An error occurred');
                    }
                });
        }
    }
}

new Order();