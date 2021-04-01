export class InvoicedAmount {
    requestId: number;
    amount: number;

    constructor(init?:Partial<InvoicedAmount>) {
        Object.assign(this, init);
    }
}