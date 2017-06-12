export default class Ticket {
    constructor(qrCode, code, name, email, vs, payed) {
        this.qrCode = qrCode;
        this.code = code;
        this.name = name;
        this.email = email;
        this.vs = vs;
        this.payed = payed;
    };
}