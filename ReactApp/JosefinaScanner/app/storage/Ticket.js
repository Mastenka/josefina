export default class Ticket {
    constructor(id, qrCode, code, name, email, chck) {
        this.id = id;
        this.qrCode = qrCode;
        this.code = code;
        this.name = name;
        this.email = email;
        this.chck = chck;
        this.category = '';
    };
}