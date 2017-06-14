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
        this.state = { text: 'Useless Placeholder' };
    }

    static navigationOptions = {
        title: 'Josefina tickets',
    };

    render() {
        const { navigate } = this.props.navigation;
        return (
            <View style={styles.container}>
                <Button
                    onPress={() => navigate('Scanner')}
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
            </View>
        );
    }

    _btnSync(event) {
        TicketsStorage.syncWithJosefina(true);
    }
}

var styles = StyleSheet.create({
    container: {
        paddingTop: 30,
        paddingBottom: 30,
        paddingRight: 30,
        paddingLeft: 30,
        flexDirection: 'column',
        justifyContent: 'space-between',
        flex:1
    },
    textCenter: {
        textAlign: 'center',

    }
});

const JosefinaScanner = StackNavigator({
    Home: { screen: HomeScreen },
    Scanner: { screen: ScannerScreen },
    ManualSearch: { screen: ManualSearchScreen },
});

AppRegistry.registerComponent('JosefinaScanner', () => JosefinaScanner);