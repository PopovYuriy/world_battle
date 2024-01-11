/**
 * Import function triggers from their respective submodules:
 *
 * const {onCall} = require("firebase-functions/v2/https");
 * const {onDocumentWritten} = require("firebase-functions/v2/firestore");
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

'use strict';

const functions = require('firebase-functions/v2');
const admin = require('firebase-admin');
admin.initializeApp();
console.log('App initialized');

exports.touch = functions.database.onValueUpdated({
        ref: '/games/{gameID}/LastTurnPlayerId',
        region: 'europe-west1'
    },
    (event) => {
        const lastTurnPlayerUid = event.data.after.val();
        const gameId = event.params.gameID;
        
        getGameData(gameId)
            .then((gameData) => {
                getPlayerData(gameData.Players, lastTurnPlayerUid)
                    .then((player) => {
                        if (player.Token === null || player.Token === undefined || player.Token.trim() === '') {
                            console.log('Player has no token, player : ', player);
                            return;
                        }
                        sendLastTurnNotification(player, gameData.Uid);
                    });
            });
    }
)

function sendLastTurnNotification(player, gameId) {
    const payload = {
        token: player.Token,
        notification: {
            title: `${player.Name} зробив свій хід`,
            body: `Зробіть хід у відповідь`
        },
        data: {
            type: "OpenGame",
            game_id: gameId,
        }
    };
    
    sendNotification(payload);
}

function sendNotification(payload) {
    admin.messaging().send(payload).then((response) => {
        console.log('Successfully sent message:', response);
        return {success: true};
    }).catch((error) => {
        console.log('Error during sent message:', error);
        return {error: error.code};
    });
}

async function getPlayerData(players, opposedPlayerId) {
    try
    {
        const targetPlayerGameData = players.find((p) => p.Uid !== opposedPlayerId);
        
        const targetPlayerSnapshot = await admin.database().ref('/users')
            .orderByChild('Uid')
            .equalTo(targetPlayerGameData.Uid)
            .once('value');

        let targetPlayer = null;

        targetPlayerSnapshot.forEach((child) => {
            targetPlayer = child.val();
            return true;
        });
        
        return targetPlayer;
    }
    catch (e) {
        console.error('Помилка отримання даних:', error);
        throw error;
    }
}

async function getGameData(gameDataId){
    const gameDataSnapshot = await admin.database().ref(`/games/${gameDataId}`).once('value');
    return gameDataSnapshot.val();
}
