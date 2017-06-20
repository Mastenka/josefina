'use strict';
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
    Alert
} from 'react-native';
import { StackNavigator } from 'react-navigation';
import ScannerScreen from './components/ScannerScreen';
import ManualSearchScreen from './components/ManualSearchScreen';
import TicketsStorage from './storage/TicketsStorage';

class HomeScreen extends React.Component {

    constructor(props) {
        super(props);
        TicketsStorage.bindCountUpdateEvent(this.updateTicketToUpdateCounter.bind(this));
        this.state = { text: 'Useless Placeholder', ticketsToUpdate: 0 };
    }

    static navigationOptions = {
        title: 'Josefina tickets',
    };

    render() {
        const { navigate } = this.props.navigation;
        return (
            <View style={styles.container}>
                <Button
                    onPress={this._btnScanner.bind(this)}
                    title="QR Scanner"
                />
                {/*<Button
                    onPress={() => navigate('ManualSearch')}
                    title="Manual search"
                />*/}
                <Button
                    onPress={this._btnSync}
                    title="Synchronize"
                />
                <Text>Tickets to synchronize: <Text style={styles.boldText}>{this.state.ticketsToUpdate}</Text></Text>


            </View>
        );
    }



    updateTicketToUpdateCounter(count) {
        this.setState(() => {
            return { ticketsToUpdate: count };
        });
    }

    _btnScanner(evnet) {

        if (TicketsStorage.isSynchronized()) {
            this.props.navigation.navigate('Scanner');
        } else {
            Alert.alert(
                'Error',
                'No tickets found, synchronize first.',
                [
                    { text: 'OK' }
                ],
                { cancelable: false }
            )
        }
    }


    _btnSync(event) {
        TicketsStorage.syncWithJosefina(true);
    }
}

var styles = StyleSheet.create({
    container: {
        paddingTop: 130,
        paddingBottom: 130,
        paddingRight: 30,
        paddingLeft: 30,
        flexDirection: 'column',
        justifyContent: 'space-between',
        flex: 1
    },
    textCenter: {
        textAlign: 'center',

    },
    boldText: {
        fontWeight: 'bold',
        fontSize: 18
    },
});

const JosefinaScanner = StackNavigator({
    Home: { screen: HomeScreen },
    Scanner: { screen: ScannerScreen },
    ManualSearch: { screen: ManualSearchScreen },
});

AppRegistry.registerComponent('JosefinaScanner', () => JosefinaScanner);