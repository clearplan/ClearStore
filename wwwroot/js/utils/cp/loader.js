export default class CPLoader {
    constructor(opts = {}) {
        const {
            text = 'Please wait...',
            blockEscape = false,
            className = ''
        } = opts;

        const dialog = document.createElement('dialog');
        dialog.className = `cp-dialog ${className}`.trim();
        dialog.role = 'dialog';
        dialog.setAttribute('aria-busy', 'true');

        const box = document.createElement('div');
        box.className = 'hstack gap';

        const span = document.createElement('span');
        span.className = '';
        span.textContent = text;

        const img = document.createElement('img');
        img.src = '/img/loader.gif';
        img.width = 24;
        img.height = 24;

        box.append(span);
        box.append(img);
        dialog.append(box);
        document.body.append(dialog);

        if (blockEscape) {
            dialog.addEventListener('cancel', (e) => e.preventDefault());
        }

        dialog.addEventListener('close', () => this._restoreScroll());

        this.dialog = dialog;
        this._span = span;
        this._openPromise = null; // reused if open() called again while already open
    }

    open() {
        if (this.dialog.open) {
            return this._openPromise ?? Promise.resolve(this);
        }

        this._lockScroll();
        this.dialog.showModal();

        // Resolve after paint so .then(fn) runs when the dialog is actually visible.
        this._openPromise = new Promise((resolve) => {
            requestAnimationFrame(() => {
                requestAnimationFrame(() => resolve(this));
            });
        });

        return this._openPromise;
    }

    close() {
        if (this.dialog.open) {
            this.dialog.close();
        }
        this._openPromise = null;
    }

    update(text) {
        if (typeof text === 'string') {
            this._span.textContent = text;
        }
    }

    destroy() {
        this.close();
        if (this.dialog.isConnected) {
            this.dialog.remove();
        }
    }

    _lockScroll() {
        if (this._scrollLocked) {
            return;
        }
        this._scrollLocked = true;
        this._prevOverflow = document.documentElement.style.overflow;
        document.documentElement.style.overflow = 'hidden';
    }

    _restoreScroll() {
        if (!this._scrollLocked) {
            return;
        }
        document.documentElement.style.overflow = this._prevOverflow || '';
        this._scrollLocked = false;
    }
}


