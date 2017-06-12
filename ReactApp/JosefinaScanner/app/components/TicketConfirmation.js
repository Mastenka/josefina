import React, { Component } from 'react';
import {
    StyleSheet,
    TouchableHighlight,
    Text,
    View,
    Button,
    Modal
} from 'react-native';

export default class TicketConfirmation extends Component {

    state = {
        modalVisible: true,
    }

    render() {
        return (
            <View style={styles.container}>
                <Modal
                    animationType={"slide"}
                    transparent={false}
                    visible={this.props.modalVisible}
                    onRequestClose={() => { alert("Modal has been closed.") }}>
                    <View style={styles.container}>
                        <View>
                            <Text>Email</Text>
                            <Text>Name</Text>
                            <Text>Code</Text>
                            <Text>VS</Text>

                            <Button
                                onPress={this._btnTest.bind(this)}
                                title="Test"
                            />
                        </View>
                    </View>
                </Modal>
            </View>
        );
    };

    _btnTest(event) {
        this.setModalVisible(!this.state.modalVisible)
    }

    setModalVisible(visible) {
        this.setState({ modalVisible: visible });
    }
}

var styles = StyleSheet.create({
    container: {
        paddingTop: 130,
        paddingBottom: 30,
        paddingRight: 30,
        paddingLeft: 30,
        flexDirection: 'column',
        justifyContent: 'space-between',
        flex: 1,
        borderWidth: 12
    },
    textCenter: {
        textAlign: 'center',

    }
});