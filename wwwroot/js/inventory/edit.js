import Loader from '/js/utils/cp/loader.js';

class Inventory {
    constructor() {
        this.loader = new Loader();
        this.template = document.querySelector('#inventoryRow');
        this.tbody = document.querySelector('[data-inventory-table] tbody');
        this.rows = Array.from(this.tbody.querySelectorAll('[data-row]'));

        this.addButton = document.querySelector('[data-add]');
        this.addButton.addEventListener('click', this.addItem.bind(this));
    }

    addItem() {
        let newRow;
        if (this.rows.length > 0) {
            newRow = this.rows[this.rows.length - 1].cloneNode(true);
        }
        else {
            newRow = this.template.content.firstElementChild.cloneNode(true);
        }

        const lastCell = newRow.querySelector('td:last-child');
        const prevDeleteButton = lastCell.querySelector('[data-delete]');
        if (prevDeleteButton) prevDeleteButton.remove();

        const index = this.rows.length;

        lastCell.insertAdjacentHTML('afterbegin', `
            <div class="istack p-1 bg-red br-1" data-delete="${index}">
                <i class="fi fi-delete-regular fs-1 text-white"></i>
            </div>`);

        this.setFieldAttributes(newRow, index, true);

        this.tbody.append(newRow);
        this.rows.push(newRow);

        const deleteButton = newRow.querySelector('[data-delete]');
        deleteButton.addEventListener('click', () => this.deleteRow(newRow));
    }

    deleteRow(row) {
        row.remove();
        this.rows = this.rows.filter(r => r != row);
        this.resetIds();
    }

    resetIds() {
        this.rows.forEach((row, index) => {
            this.setFieldAttributes(row, index, false);
            const deleteButton = row.querySelector('[data-delete]');
            if (deleteButton) {
                deleteButton.setAttribute('data-delete', index);
            }
        });
    }

    setFieldAttributes(row, index, resetValues) {
        let id = row.querySelector('[data-id]');
        let productId = row.querySelector('[data-productid]');
        let threshold = row.querySelector('[data-threshold]');
        let size = row.querySelector('[data-size]');
        let color = row.querySelector('[data-color]');
        let office = row.querySelector('[data-office]');
        let quantity = row.querySelector('[data-quantity]');
        let visibility = row.querySelector('[data-visibility]');

        if (id) {
            id.id = `ProductInventoryDto_ProductInventory_${index}__Id`;
            id.name = `ProductInventoryDto.ProductInventory[${index}].Id`;
            if (resetValues) {
                id.value = null;
            }
        }
        if (productId) {
            productId.id = `ProductInventoryDto_ProductInventory_${index}__ProductId`;
            productId.name = `ProductInventoryDto.ProductInventory[${index}].ProductId`;
            if (resetValues) {
                productId.value = null;
            }
        }
        if (threshold) {
            threshold.id = `ProductInventoryDto_ProductInventory_${index}__Threshold`;
            threshold.name = `ProductInventoryDto.ProductInventory[${index}].Threshold`;
            if (resetValues) {
                threshold.value = null;
            }
        }
        if (size) {
            size.id = `ProductInventoryDto_ProductInventory_${index}__ProductSizeId`;
            size.name = `ProductInventoryDto.ProductInventory[${index}].ProductSizeId`;
            if (resetValues) {
                size.selectedIndex = 0;
            }
        }
        if (color) {
            color.id = `ProductInventoryDto_ProductInventory_${index}__ProductColorId`;
            color.name = `ProductInventoryDto.ProductInventory[${index}].ProductColorId`;
            if (resetValues) {
                color.selectedIndex = 0;
            }
        }
        if (office) {
            office.id = `ProductInventoryDto_ProductInventory_${index}__ProductOfficeId`;
            office.name = `ProductInventoryDto.ProductInventory[${index}].ProductOfficeId`;
            if (resetValues) {
                office.selectedIndex = 0;
            }
        }
        if (quantity) {
            quantity.id = `ProductInventoryDto_ProductInventory_${index}__Quantity`;
            quantity.name = `ProductInventoryDto.ProductInventory[${index}].Quantity`;
            if (resetValues) {
                quantity.value = null;
            }
        }
        if (visibility) {
            visibility.id = `ProductInventoryDto_ProductInventory_${index}__IsVisible`;
            visibility.name = `ProductInventoryDto.ProductInventory[${index}].IsVisible`;
            if (resetValues) {
                visibility.selectedIndex = 0;
            }
        }
    }
}

new Inventory();
