import { Alert } from 'react-native';

import Ticket from './Ticket';

let _tickets = [];

class TicketsStorage {

    constructor() {
        this._loadTickets();
    };

    _loadTickets() {
        _tickets = [new Ticket("QR1", "CODE1", "name1", "email1", 1, true), new Ticket("QR2", "CODE2", "name2", "email2", 2, false)];
        // Alert.alert('asd');
    }

    getTickets = () => {
        // return "Ahoj";
        return _tickets;
    };

    getTicketByQRCode = (qrCode) => {
        var ticket = _tickets.filter((ticket) => ticket.qrCode === qrCode);

        if (ticket.length === 1) {
            var nevim = ticket[0];
            return nevim;
        } else {
            return undefined;
        }
    };
}

export default new TicketsStorage();