import CPLoader from '/js/utils/cp/loader.js';

class ProductInventory {
    constructor() {
        this.loader = new CPLoader();
        this.template = document.querySelector('#inventoryRow');
        this.productId = document.querySelector('[data-productid]').value;
        this.tbody = document.querySelector('[data-inventory-table] tbody');
        this.addButton = document.querySelector('[data-add-row]');

        this.init();
    }

    init() {
        this.tbody.addEventListener('click', (e) => {
            const row = e.target.closest('[data-row]');
            if (!row) {
                return;
            }

            if (e.target.matches('[data-inventory-save]')) {
                this.handleSave(row);
            }
            else if (e.target.matches('[data-inventory-remove]')) {
                this.handleRemove(row);
            }
        });

        this.addButton.addEventListener('click', () => this.addRow());
    }

    async handleSave(row) {
        const idInput = row.querySelector('[data-id]');
        const id = parseInt(idInput?.value || 0);

        const data = {
            id: id,
            productId: parseInt(this.productId),
            productSizeId: parseInt(row.querySelector('[data-size], [data-inventory-size]').value),
            productColorId: parseInt(row.querySelector('[data-color], [data-inventory-color]').value),
            productOfficeId: parseInt(row.querySelector('[data-office], [data-inventory-office]').value),
            quantity: parseInt(row.querySelector('[data-quantity], [data-inventory-quantity]').value),
            isVisible: row.querySelector('[data-visibility], [data-inventory-visibility]').value === 'true',
            threshold: null
        };

        if (id > 0) {
            await this.updateItem(data);
        }
        else {
            await this.addItem(row, data);
        }
    }

    async addItem(row, data) {
        this.loader.open();
        try {
            const response = await fetch('/api/apiinventory/save', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                const result = await response.json();
                this.finalizeNewRow(row, result.id);
                
            }
            else {
                this.loader.update('Failed to add item.');
                this.loader.open();
            }
        } catch (err) {
            this.loader.update('Add error:', err);
            this.loader.open();
        }
        finally {
            this.loader.close();
        }
    }

    async updateItem(data) {
        this.loader.open();

        try {
            const response = await fetch(`/api/apiinventory/update/${data.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                console.log('Item updated successfully');
            }
            else {
                this.loader.update('Failed to update item.');
                this.loader.open();
            }
        }
        catch (err) {
            this.loader.update('Update error:', err);
            this.loader.open();
        }
        finally {
            this.loader.close();
        }
    }

    async handleRemove(row) {
        const idInput = row.querySelector('[data-id]');
        const id = parseInt(idInput?.value || 0);

        if (id === 0) {
            row.remove();
            return;
        }

        if (confirm('Are you sure you want to delete this inventory record?')) {
            this.loader.open();
            try {
                const response = await fetch(`/api/apiinventory/delete/${id}`, { method: 'DELETE' });
                if (response.ok) {
                    row.remove();
                }
                else {
                    this.loader.update('Failed to delete item.');
                    this.loader.open();
                }
            }
            catch (err) {
                this.loader.update('Delete error:', err);
                this.loader.open();
            }
            finally {
                this.loader.close();
            }
        }
    }

    addRow() {
        const clone = this.template.content.cloneNode(true);
        this.tbody.appendChild(clone);
    }

    finalizeNewRow(row, newId) {
        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.setAttribute('data-id', '');
        idInput.value = newId;
        row.appendChild(idInput);

        const saveBtn = row.querySelector('[data-inventory-save]');
        if (saveBtn) {
            saveBtn.textContent = 'Update';
            saveBtn.classList.remove('muted', 'success');
            saveBtn.classList.add('filled', 'info');
        }

        const removeBtn = row.querySelector('[data-inventory-remove]');
        if (removeBtn) {
            removeBtn.textContent = 'Delete';
            removeBtn.classList.remove('muted');
            removeBtn.classList.add('filled');
        }
    }
}

new ProductInventory();