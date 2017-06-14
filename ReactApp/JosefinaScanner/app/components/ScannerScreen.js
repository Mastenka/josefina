import React, { Component } from 'react';
import {
    AppRegistry,
    Dimensions,
    StyleSheet,
    TouchableHighlight,
    Text,
    View,
    Button,
    TextInput,
    Alert,
    Modal
} from 'react-native';
import Camera from 'react-native-camera';
import TicketsStorage from '../storage/TicketsStorage';
import TicketConfirmation from './TicketConfirmation';

export default class ScannerScreen extends Component {
    static navigationOptions = {
        title: 'Tickets scanner',
    };

    constructor() {
        super();
        this.state = {
            cameraVisible: true, ticketToCheck: {}
        }
    }

    render() {
        let cameraVisible = this.state.cameraVisible;

        if (cameraVisible) {
            return (
                <View style={styles.container}>
                    <Camera
                        ref={(cam) => {
                            this.camera = cam;
                        }}
                        style={styles.preview}
                        aspect={Camera.constants.Aspect.fill}
                        onBarCodeRead={this._onBarCodeRead.bind(this)}
                    >
                        {/*<Text style={styles.capture} onPress={this._btnTest.bind(this)}>[CAPTURE]</Text>*/}
                    </Camera>
                </View>
            );
        } else {
            return (
                <View style={styles.container}>
                    <TicketConfirmation
                        modalClosed={this._modalClosed.bind(this)}
                        ticket={this.state.ticketToCheck}
                    />
                </View>
            );
        }
    };

    _modalClosed(checkTicket) {
        if (checkTicket) {
            var ticketToCheck = this.state.ticketToCheck;
            this._checkTicket(ticketToCheck);
        }

        this.setState(() => {
            return { cameraVisible: true, ticketToCheck: {} };
        });
    };

    _checkTicket(ticket) {
        TicketsStorage.checkTicket(ticket);
    };

    _onBarCodeRead(event) {
        var ticket = TicketsStorage.getTicketByQRCode(event.data);

        if (ticket !== undefined) {
            if (ticket.chck) {
                this._ticketAlreadyCheckedAlert();
            } else {
                this._showModalTicket(ticket);
            }
        } else {
            this._ticketNotFoundAlert();
        }
    };

    _showModalTicket(ticket) {
        this.setState(previousState => {
            return { cameraVisible: !previousState.cameraVisible, ticketToCheck: ticket };
        });
    }

    _ticketNotFoundAlert() {
        Alert.alert(
            'Error',
            'Ticket not found! :(',
            [
                { text: 'OK' }
            ],
            { cancelable: false }
        )
    };

    _ticketAlreadyCheckedAlert() {
        Alert.alert(
            'Error',
            'Ticket already checked!',
            [
                { text: 'OK' }
            ],
            { cancelable: false }
        )
    };

    // Hack
    _btnTest(event) {
        var mock

        if (neco === 0) {
            mock = { data: 'e0adac80-ed73-4003-909b-38b5b4a4995d' };
            neco++;
            this._onBarCodeRead(mock);
            return;
        }

        if (neco === 1) {
            mock = { data: '3b2d4f03-30ef-4a06-a0fb-9e87aa0dc995' };
            neco++;
            this._onBarCodeRead(mock);
            return;
        }
        if (neco === 2) {
            mock = { data: '332348e6-27ef-4650-9e54-d3af79cbb5de' };
            neco++;
            this._onBarCodeRead(mock);
            return;
        }
        if (neco === 3) {
            mock = { data: '23e423e2-88cb-4b2b-8a68-a633c78699f8' };
            neco++;
            this._onBarCodeRead(mock);
            return;
        }

        if (neco === 4) {
            mock = { data: '5505e348-baa7-44af-9079-780c5c2104b8' };
            neco++;
            this._onBarCodeRead(mock);
            return;
        }

        if (neco === 5) {
            mock = { data: 'c93267ef-0fa4-4414-9bea-c4db58c94779' };
            neco++;
            this._onBarCodeRead(mock);
            return;
        }
    }
}

let neco = 0;

const styles = StyleSheet.create({
    container: {
        flex: 1,
        flexDirection: 'row',
    },
    preview: {
        flex: 1,
        justifyContent: 'flex-end',
        alignItems: 'center'
    },
    capture: {
        flex: 0,
        backgroundColor: '#fff',
        borderRadius: 5,
        color: '#000',
        padding: 10,
        margin: 40
    }
});