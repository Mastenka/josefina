import React from 'react';
import { Text, View } from 'react-native';

export default class ScannerScreen extends React.Component {
  static navigationOptions = {
    title: 'Tickets scanner',
  };
  render() {
    return (
      <View>
        <Text>Scanner</Text>
      </View>
    );
  }
}