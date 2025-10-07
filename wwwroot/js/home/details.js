import Loader from '/js/utils/cp/loader.js';

class Details {
    constructor() {
        this.productId = +document.querySelector('[data-product-id]').dataset.productId;
        this.submitButton = document.querySelector('[data-submit]');

        // toggle thumbnails
        this.thumbnails = Array.from(document.querySelectorAll('[data-thumbnail-src]'));
        this.thumbnailFrames = Array.from(document.querySelectorAll('[data-thumbnail-frame]'));
        this.target = document.querySelector('[data-thumbnail-target]');

        this.thumbnailFrames.forEach((f, i) => {
            if (i == 0) f.classList.add('active');
        });

        this.thumbnails.forEach(thumb => {
            thumb.addEventListener('click', () => {
                this.thumbnailFrames.forEach(frame => frame.classList.remove('active'));
                const frame = thumb.closest('[data-thumbnail-frame]');

                if (frame) {
                    frame.classList.add('active');
                }

                if (this.target && thumb instanceof HTMLImageElement) {
                    this.target.src = thumb.src;
                }
            });
        });

        // color -> size -> quantity updates
        this.colors = Array.from(document.querySelectorAll('[data-color]'));
        this.sizes = Array.from(document.querySelectorAll('[data-size]'));
        this.quantity = document.querySelector('[data-quantity]');
        this.currentSizes = [];

        if (this.colors.length) {
            this.colors.forEach(color => {
                color.addEventListener('change', e => {
                    this.currentSizes = JSON.parse(e.target.dataset.sizes || '[]');
                    this.updateSizes();
                    this.sizeId = null;
                    this.quantity.value = null;
                    this.quantity.removeAttribute('max');
                });
            });
        }

        if (this.sizes.length) {
            this.sizes.forEach(size => {
                size.addEventListener('change', e => {
                    this.sizeId = +e.target.dataset.size;
                    this.updateQuantity();
                });
            });
        }

        if (this.quantity) {
            this.quantity.addEventListener('input', () => {
                const max = parseInt(this.quantity.max, 10);
                if (this.quantity.value && max) {
                    let val = parseInt(this.quantity.value, 10);
                    if (val > max) {
                        this.quantity.value = max;
                    }
                    if (val < 1) {
                        this.quantity.value = 1;
                    }
                }
                this.toggleSubmit();
            });
        }

        this.init();
        this.toggleSubmit();
    }

    init() {
        if (!this.colors.length && !this.sizes.length) {
            this.quantity.disabled = false;
            this.quantity.value = null;
            this.quantity.placeholder = 'Enter quantity';
            return;
        }

        if (this.colors.length) {
            const checkedColor = this.colors.find(c => c.checked);
            if (checkedColor) {
                this.currentSizes = JSON.parse(checkedColor.dataset.sizes || '[]');
                this.updateSizes();
            }
        } else if (this.sizes.length) {
            this.currentSizes = this.sizes.map(s => ({
                sizeId: +s.dataset.size,
                quantity: +s.dataset.sizeQuantity || 0
            }));
        }

        //const checkedColor = this.colors.find(c => c.checked);
        //if (checkedColor) {
        //    this.currentSizes = JSON.parse(checkedColor.dataset.sizes || '[]');
        //    this.updateSizes();
        //}

        const checkedSize = this.sizes.find(s => s.checked && !s.disabled);
        if (checkedSize) {
            this.sizeId = +checkedSize.dataset.size;
            this.updateQuantity();
        }
    }

    updateSizes() {
        const sizeMap = this.currentSizes.reduce((acc, s) => {
            acc[s.sizeId] = s.quantity;
            return acc;
        }, {});

        this.sizes.forEach(size => {
            const id = +size.dataset.size;
            const available = (sizeMap[id] ?? 0) > 0;
            size.disabled = !available;
            if (!available) {
                size.checked = false;
            }
        });
    }

    updateQuantity() {
        const match = this.currentSizes.find(s => s.sizeId == this.sizeId);

        if (match && match.quantity > 0) {
            this.quantity.disabled = false;
            this.quantity.max = match.quantity;
            this.quantity.placeholder = `Max available: ${match.quantity}`;
            this.quantity.value = null;
        }
        else {
            this.quantity.disabled = true;
            this.quantity.value = null;
            this.quantity.removeAttribute('max');
        }

        this.toggleSubmit();
    }

    toggleSubmit() {
        if (this.quantity && !this.quantity.disabled && this.quantity.value && parseInt(this.quantity.value, 10) > 0) {
            this.submitButton.disabled = false;
        }
        else {
            this.submitButton.disabled = true;
        }
    }

}

new Details();
