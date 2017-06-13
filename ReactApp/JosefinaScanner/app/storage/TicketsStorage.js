import { Alert } from 'react-native';

import Ticket from './Ticket';

let _tickets = [];
let _ticketIdsToBeUpdated = [];
let _ticketExportIdsToBeUpdated = [];

const josefinaGetTicketsUrl = 'http://pepicka.cz/api/project/react/GetTickets/asd';

let _josefinaViewModel = {};

class TicketsStorage {

    constructor() {
        this._loadTicketsFromJosefina();
    };

    _loadTicketsFromJosefina() {
        fetch(josefinaGetTicketsUrl)
            .then((response) => response.json())
            .then((responseJSON) => {
                this._processLoadedTickets(responseJSON);
            })
            .catch((error) => {
                console.warn(error);
            });
    };

    _processLoadedTickets(ticketsViewModel) {
        _josefinaViewModel = ticketsViewModel;
    };

    getTicketByQRCode = (qrCode) => {
        var ticket = {};
        var loaded = false;

        var ticketArray = _josefinaViewModel.Tickets.filter((ticket) => ticket.qrCode === qrCode);

        if (ticketArray.length === 1) {
            ticket = ticketArray[0];
            ticket.type = 'T';

            var headerArray = _josefinaViewModel.Headers.filter((header) => header.id === ticket.CtgID);
            ticket.category = headerArray[0].name;
            loaded = true;
        } else {
            ticketArray = _josefinaViewModel.TicketExports.filter((ticketExport) => ticketExport.qrCode === qrCode);
            if (ticketArray.length === 1) {
                ticket = ticketArray[0];
                ticket.type = 'E';

                var headerArray = _josefinaViewModel.ExportHeaders.filter((exportHeader) => exportHeader.id === ticket.CtgID);
                ticket.category = headerArray[0].name;
                loaded = true;
            }
        }

        if (loaded) {
            return ticket;
        } else {
            return undefined;
        }
    };

    checkTicket = (ticketToCheck) => {

        if (ticketToCheck.type === 'T') {   
            _josefinaViewModel.Tickets.forEach(function (ticket) {
                if (ticket.id === ticketToCheck.id) {
                    ticket.chck = true;
                }
            }, this);
            _ticketIdsToBeUpdated.push(ticketToCheck.id);

        } else {
            _josefinaViewModel.TicketExports.forEach(function (ticketExport) {
                if (ticketExport.id === ticketToCheck.id) {
                    ticketExport.chck = true;
                }
            }, this);
            _ticketExportIdsToBeUpdated.push(ticketToCheck.id);

        }
    }
}

export default new TicketsStorage();