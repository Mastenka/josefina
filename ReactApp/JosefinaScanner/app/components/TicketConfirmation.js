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

    // constructor(){
    //     super();
    //     this.state = 
    // }

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
                            <Text>Email</Text>
                            <Text>{this.props.ticket.email}</Text>
                            <Text>Name</Text>
                            <Text>{this.props.ticket.name}</Text>
                            <Text>Code</Text>
                            <Text>{this.props.ticket.code}</Text>
                            <Text>VS</Text>
                            <Text>{this.props.ticket.vs}</Text>
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
        borderWidth: 12
    },
    textCenter: {
        textAlign: 'center',

    }
});