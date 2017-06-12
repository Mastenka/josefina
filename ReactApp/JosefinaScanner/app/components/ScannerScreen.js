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
import Camera from 'react-native-camera';
import TicketsStorage from '../storage/TicketsStorage';

export default class ScannerScreen extends Component {
   static navigationOptions = {
    title: 'Tickets scanner',
  };

  render() {
    return (
      <View style={styles.container}>
        <Camera
          ref={(cam) => {
            this.camera = cam;
          }}
          style={styles.preview}
          aspect={Camera.constants.Aspect.fill}
          onBarCodeRead={this._onBarCodeRead.bind(this)}>
        </Camera>
      </View>
    );
  };

  _onBarCodeRead(event){
    // Alert.alert(event.data);
    Alert.alert(TicketsStorage.getTickets());
  };
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