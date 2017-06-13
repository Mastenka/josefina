import { Alert } from 'react-native';

import Ticket from './Ticket';

let _tickets = [];
let _ticketIdsToBeUpdated = [];

class TicketsStorage {

    constructor() {
        this._loadTickets();
    };

    _loadTickets() {
        _tickets = [new Ticket("1","QR1", "CODE1", "name1", "email1", 1, true), new Ticket("2", "QR2", "CODE2", "name2", "email2", 2, false)];
    }

    getTickets = () => {
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

    checkTicket = (ticketToCheck) => {
        _tickets.forEach(function(ticket) {
            if(ticket.id === ticketToCheck.id){
                ticket.checked = true;
            }
        }, this);

        _ticketIdsToBeUpdated.push(ticketToCheck.id);
    }
}

export default new TicketsStorage();