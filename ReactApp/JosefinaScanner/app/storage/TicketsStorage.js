import { Alert } from 'react-native';

import Ticket from './Ticket';

let _tickets = [];
let _ticketIdsToBeUpdated = [];
let _ticketExportIdsToBeUpdated = [];

let _authGUID = '3deca158-9668-42d2-8619-1135e487685a';

const josefinaGetTicketsUrl = 'http://pepicka.cz/api/project/react/GetTickets/3deca158-9668-42d2-8619-1135e487685a';
const josefinaPostTicketsUrl = 'http://pepicka.cz/api/project/react/sync';
// const josefinaPostTicketsUrl = 'https://requestb.in/12n52k51';

let _josefinaViewModel = {};

class TicketsStorage {

    constructor() {
        this.syncWithJosefina(false);
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

    syncWithJosefina(showAlert) {
        fetch(josefinaPostTicketsUrl, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                tickets: _ticketIdsToBeUpdated,
                exports: _ticketExportIdsToBeUpdated,
                guid: _authGUID
            })
        })
            .then((response) => response.json())
            .then((responseJSON) => {
                this._processLoadedTickets(responseJSON);
                if (showAlert) {
                    this._showSyncAlert();
                }
            })
            .catch((error) => {
                console.warn(error);
            });
    };

    _showSyncAlert() {
        Alert.alert(
            'Success',
            'Synchronized with server!',
            [
                { text: 'OK' }
            ],
            { cancelable: false }
        )
    };


    _processLoadedTickets(ticketsViewModel) {
        _josefinaViewModel = ticketsViewModel;
        _ticketIdsToBeUpdated = [];
        _ticketExportIdsToBeUpdated = [];
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