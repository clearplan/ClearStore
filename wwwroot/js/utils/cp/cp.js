export default class CP {
    constructor(cancelPostGuard = false) {
        this.cancelPostGuard = cancelPostGuard;
    }

    setHeaders() {
        return {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        };
    }

    create(node, html, css, attrs) {
        node = document.createElement(node);
        if (html != null || html != undefined) {
            node.innerHTML = html;
        }
        if (css != null && css != undefined && css != '') {
            node.className = css;
        }
        else {
            node.removeAttribute('class');
        }
        if (attrs) {
            for (let key in attrs) {
                if (attrs.hasOwnProperty(key)) {
                    node.setAttribute(key, attrs[key]);
                }
            }
        }
        return node;
    }

    clamp(value, precision = 2) {
        if (value.includes('.')) {
            const [whole, fraction] = value.split('.');
            if (fraction.length > precision) {
                value = `${whole}.${fraction.substring(0, precision)}`;
            }
        }
        return value;
    }

    // new
    postGuard() {
        if (!this.cancelPostGuard) {
            this.weakSet = new WeakSet();

            function createDialog(message = 'Please wait...') {
                const dialog = document.createElement('dialog');
                dialog.setAttribute('data-postguard-dialog', 'true');

                const hstack = document.createElement('div');
                hstack.className = 'hstack';

                const span = document.createElement('span');
                span.innerText = message;

                const img = document.createElement('img');
                img.src = `/img/loader.gif`;
                img.width = 20;
                img.height = 20;
                img.alt = 'Loading icon';

                hstack.append(span, img);
                dialog.append(hstack);

                document.body.append(dialog);
                dialog.addEventListener('cancel', (e) => e.preventDefault());

                return dialog;
            }

            const forms = Array.from(document.querySelectorAll('form[method="post"]'));

            forms.forEach(form => {
                if (this.weakSet.has(form)) {
                    return;
                }

                this.weakSet.add(form);

                const dialog = createDialog();

                form.addEventListener('submit', (e) => {
                    if (!form.checkValidity()) {
                        return;
                    }

                    if (form.dataset.submitted == 'true') {
                        e.preventDefault();
                        return;
                    }

                    form.dataset.submitted = 'true';

                    form.querySelectorAll('button[type="submit"], button[data-submit], input[type="submit"]').forEach(field => {
                        field.disabled = true;
                        field.setAttribute('aria-disabled', true);
                    });

                    dialog.showModal();
                }, { capture: true });
            })
        }
    }

    // original
    //preventDoublePostSubmission() {
    //    if (!this.cancelDoublePostSubmission) {
    //        const dp = {
    //            body: document.body,
    //            hstack: this.create('div', '', 'hstack'),
    //            span: this.create('span', 'Please wait.', ''),
    //            loader: this.create('img', '', ''),
    //            dialog: this.create('dialog', '', 'dialog', { 'data-submit-dialog': true }),
    //            form: document.querySelector('.app-viewport form[method="post"]')
    //        };

    //        const content = setContent();

    //        if (content) {
    //            if (dp.form) {
    //                dp.submitButton = dp.form.querySelector('button[type="submit"]');
    //                dp.submitButton.addEventListener('click', (e) => {
    //                    if (dp.form.checkValidity()) {
    //                        e.stopPropagation();
    //                        dp.form.submit();
    //                        dp.submitButton.setAttribute('disabled', 'disabled');
    //                        dp.dialog.showModal();
    //                    }
    //                });
    //            }
    //        }

    //        function setContent() {
    //            dp.loader.src = `/img/loader.gif`;
    //            dp.loader.width = 20;
    //            dp.loader.height = 20;
    //            dp.loader.alt = `Loading spinner`;
    //            dp.loader.style.display = 'inline-block';
    //            dp.loader.style.verticalAlign = 'middle';

    //            dp.hstack.append(dp.span);
    //            dp.hstack.append(dp.loader);
    //            dp.dialog.append(dp.hstack);

    //            if (!dp.form) {
    //                return false;
    //            }

    //            dp.body.append(dp.dialog);
    //            return dp;
    //        }
    //    }
    //}

    toCurrency(num) {
        let dollar = new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        });
        return dollar.format(num);
    }

    toDecimal(value, digits = 2) {
        let number = new Intl.NumberFormat('en-US', { style: 'decimal', maximumFractionDigits: digits }).format(value);
        return parseFloat(number);
    }

    toDecimalWithoutRounding(value, digits = 2) {
        const factor = 10 ** digits;
        const truncated = Math.trunc(value * factor) / factor;
        return truncated.toLocaleString(undefined, {
            minimumFractionDigits: digits,
            maximumFractionDigits: digits
        });
    }

    toDouble(value, digits = 2) {
        return parseFloat(value).toFixed(digits);
    }

    toBoolean(value) {
        let type = Object.prototype.toString.call(value);
        return type;
    }

    toDate(date, year = 2) {
        const yearFormat = year == 2 ? '2-digit' : 'numeric';
        const newDate = new Date(date).toLocaleDateString('en-US', {
            year: yearFormat,
            month: '2-digit',
            day: '2-digit',
            hour12: true
        });
        return newDate;
    }
}