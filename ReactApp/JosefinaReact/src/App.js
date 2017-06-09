'use strict';
import React, { Component } from 'react';
import {
  AppRegistry,
  Dimensions,
  StyleSheet,
  Text,
  TouchableHighlight,
  View
} from 'react-native';
import Camera from 'react-native-camera';

class JosefinaReact extends Component {
  render() {
    return (
      <View style={styles.container}>
        <Camera
          ref={(cam) => {
            this.camera = cam;
          }}
          style={styles.preview}
          aspect={Camera.constants.Aspect.fill}>
          <Text style={styles.capture} onPress={this.takePicture.bind(this)}>[CAPTURE]</Text>
        </Camera>
      </View>
    );
  }

  takePicture() {
    const options = {};
    //options.location = ...
    this.camera.capture({metadata: options})
      .then((data) => console.log(data))
      .catch(err => console.error(err));
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

AppRegistry.registerComponent('JosefinaReact', () => JosefinaReact);


/*
import React from 'react';
import {
  AppRegistry,
  Text, View, Button
} from 'react-native';
import { StackNavigator } from 'react-navigation';
import ScannerScreen from './components/ScannerScreen';
import ManualSearchScreen from './components/ManualSearchScreen';

class HomeScreen extends React.Component {
  static navigationOptions = {
    title: 'Josefina tickets',
  };
  render() {
    const { navigate } = this.props.navigation;
    return (
      <View>
        <Button 
          onPress={() => navigate('Scanner')}
          title="QR Scanner"
        />
        <Button
          onPress={() => navigate('ManualSearch')}
          title="Manual search"
        />         
      </View>
    );
  }
}

const JosefinaReact = StackNavigator({
  Home: { screen: HomeScreen }, 
  ManualSearch: { screen: ManualSearchScreen }, 
  Scanner: { screen: ScannerScreen }
});

AppRegistry.registerComponent('JosefinaReact', () => JosefinaReact);*/