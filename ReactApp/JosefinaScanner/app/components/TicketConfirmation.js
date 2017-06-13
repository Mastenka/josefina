import React, { Component } from 'react';
import {
    StyleSheet,
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
                    onRequestClose={() => { this.props.modalClosed(false) }}>
                    <View style={styles.container}>
                        <View>
                            <Text>Category:</Text>
                            <Text style={styles.boldText}>{this.props.ticket.category}</Text>
                            <Text>Email:</Text>
                            <Text style={styles.boldText}>{this.props.ticket.email}</Text>
                            <Text>Name:</Text>
                            <Text style={styles.boldText}>{this.props.ticket.name}</Text>
                            <Text>Code:</Text>
                            <Text style={styles.boldText}>{this.props.ticket.code}</Text>
                        </View>
                        <Button title='Check' onPress={this._onCheck.bind(this)}/>
                        <Button title='Back' onPress={this._onBack.bind(this)}/>
                    </View>
                </Modal>
            </View>
        );
    };

    _onBack(event){
        this.props.modalClosed(false);
    };

    _onCheck(event){
        this.props.modalClosed(true); 
    };
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
        borderWidth: 12,
        borderColor: 'green'
    },
    boldText: {
        fontWeight: 'bold',
        fontSize: 18
    },
    textCenter: {
        textAlign: 'center',
    }
});