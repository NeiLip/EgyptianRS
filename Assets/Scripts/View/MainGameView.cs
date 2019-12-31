using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameView : MonoBehaviour {
    public PlayersPlaygroundContainer[] PlayersPlaygroundContainers;

    [Serializable]
    public class PlayersPlaygroundContainer {
        public Text[] PlayersPlaygroundTexts;
    }

    public Text WhosTurnText;
    public Text[] PlayersCardsCountersTexts; // Array of texts for counters
    public Text GameDeckCardsCounterText;

    public Text[] PlayersWinsCountersTexts;

    public Action EPlayTurnPressed;
    public Action ERestartPressed;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            PlayTurnButton();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            RestartGameButton();
        }
    }

    public void PlayTurnButton() {
        EPlayTurnPressed();
    }

    public void RestartGameButton() {
        ERestartPressed();
    }

    public void UpdateView(GameData gameData) {
        UpdateCounters(gameData);
        UpdatePlayground(gameData);
    }

    private void UpdatePlayground(GameData gameData) {
        for (int i = 0; i < PlayersPlaygroundContainers.Length; i++) {
            for (int j = 0; j < PlayersPlaygroundContainers[i].PlayersPlaygroundTexts.Length; j++) {
                PlayersPlaygroundContainers[i].PlayersPlaygroundTexts[j].text = "";
            }
        }

        for (int i = 0; i < gameData.DeckCards.Count; i++) {
            if (gameData.DeckCards[i].Location == Card.Locations.Playground) {
                PlayersPlaygroundContainers[gameData.DeckCards[i].OwnerPlayerID].PlayersPlaygroundTexts[gameData.DeckCards[i].IndexInPlayground].text = gameData.DeckCards[i].ToString();
            }
        }
    }

    private void UpdateCounters(GameData gameData) {
        for (int i = 0; i < PlayersCardsCountersTexts.Length; i++) {
            PlayersCardsCountersTexts[i].text = gameData.PlayersCards[i].Count.ToString();
        }
        //Debug.Log("Length: " + PlayersWinsCountersTexts.Length);

        //Updateing winning counters
        //for (int i = 0; i < PlayersWinsCountersTexts.Length; i++) {
        //    PlayersWinsCountersTexts[i].text = GameStats.WinningCounters[i].ToString();
        //}

        GameDeckCardsCounterText.text = gameData.DeckCards.Count.ToString();

        WhosTurnText.text = "Player " + gameData.CurrentTurnPlayer;
    }
}
