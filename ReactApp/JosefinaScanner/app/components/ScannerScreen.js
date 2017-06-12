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



    render() {
        return (
            <TicketConfirmation/>
        );
    };

    _onBarCodeRead(event) {
        Alert.alert(event.data);
        // Alert.alert(TicketsStorage.getTickets());
    };

    // Hack
    _btnTest(event) {
        // Works on both iOS and Android
        Alert.alert(
            'Alert Title',
            'My Alert Msg',
            [
                { text: 'Ask me later', onPress: () => console.log('Ask me later pressed') },
                { text: 'Cancel', onPress: () => console.log('Cancel Pressed'), style: 'cancel' },
                { text: 'OK', onPress: () => console.log('OK Pressed') },
            ],
            { cancelable: false }
        );
        var mock = { data: 'QR1' };
        // this._onBarCodeRead(mock);
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