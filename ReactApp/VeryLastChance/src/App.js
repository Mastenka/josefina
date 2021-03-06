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
import TicketsStorage from './storage/TicketsStorage'; //HACK

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
        {/*<TextInput
          style={{borderColor: 'gray', borderWidth: 1, }}
          onChange Text={(text) => this.setState({ text })}
          value={this.state.text}
        />*/}
        <Button
          onPress={() => navigate('Scanner')}
          title="QR Scanner"
        />
        <Button
          onPress={() => navigate('ManualSearch')}
          title="Manual search"
        />
        <Button
          onPress={this._btnDownload}
          title="Download"
        />
        <Button
          onPress={this._btnUpload}
          title="Upload"
        />

        <Button
          onPress={this._btnTest}
          title="Test"
        />

      </View>
    );
  }

  _btnDownload(event) {
    Alert.alert("_btnDownload");
  }

  _btnUpload(event) {
    Alert.alert("_btnUpload");
  }

  // Hack
  _btnTest(event) {

    var neco = TicketsStorage.getTickets();

    Alert.alert(JSON.stringify(neco[0]));
  }
  // HACK

}

var styles = StyleSheet.create({
  container: {
    paddingTop: 30,
    paddingBottom: 10,
    flexDirection: 'column'
  },
  textCenter: {
    textAlign: 'center',

  }
});

const VeryLastChance = StackNavigator({
  Home: { screen: HomeScreen },
  Scanner: { screen: ScannerScreen },
  ManualSearch: { screen: ManualSearchScreen },
});

AppRegistry.registerComponent('VeryLastChance', () => VeryLastChance);