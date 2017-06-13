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
                        aspect={Camera.constants.Aspect.fill}>
                        <Text style={styles.capture} onPress={this._btnTest.bind(this)}>[CAPTURE]</Text>
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
            if (ticket.checked) {
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
        var mock = { data: 'QR2' };
        this._onBarCodeRead(mock);
    }
}

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