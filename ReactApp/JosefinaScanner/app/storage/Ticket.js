export default class Ticket {
    constructor(id, qrCode, code, name, email, vs, checked) {
        this.id = id;
        this.qrCode = qrCode;
        this.code = code;
        this.name = name;
        this.email = email;
        this.vs = vs;
        this.checked = checked;
    };
}