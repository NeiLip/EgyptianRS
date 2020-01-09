using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameHandler : MonoBehaviour
{

    public MainGameView MainGameViewRef;
    public int PlayerAmount = 2;
    private string[] _suits = { "D", "H", "C", "S" };
    private const int CARDS_PER_SUIT = 13;
    private int TotalCards {
        get {
            return _suits.Length * CARDS_PER_SUIT;
        }
    }

    private bool _isGameStarted;
    private bool _isGameRestarted;
    private bool _waitingForCoroutine; //If true, everything is disabled. If false, player can play a turn


    private GameData _gameData;

    //Initializig variables
    private void Awake() {
        MainGameViewRef.ERestartPressed += RestartGameBtn;
        MainGameViewRef.EPlayTurnPressed += PlayTurnBtn;
        DontDestroyOnLoad(gameObject);
        _waitingForCoroutine = false;
        InitializeGameData();

      
        for (int i = 0; i < PlayerAmount; i++) {
            if (PlayerPrefs.HasKey(i.ToString())) {
                GameStats.WinningCounters[i] = PlayerPrefs.GetInt(i.ToString());
            }
        }
    }

    private void InitializeGameData() {
        _isGameRestarted = true;
        _isGameStarted = false;
        _gameData = new GameData(PlayerAmount);
        MainGameViewRef.UpdateView(_gameData);
    }
    
    /// Shuffle a deck
    /// <param name="deck"> Deck to shuffle </param>
    /// <returns>Shuffeled deck</returns>
    private List<Card> ShuffleDeck(List<Card> deck) {
        for (int i = deck.Count - 1; i > 1; i--) {
            int rnd = UnityEngine.Random.Range(0,i + 1);

            Card value = deck[rnd];
            deck[rnd] = deck[i];
            deck[i] = value;
        }
        return deck;
    }


    // On game Start
    // Shuffle main deck and then split to hands and gets all Text GameObjects
    public void StartGame() {
        //BuildStartingDeck();
        List<Card> shuffeledGameDeck = new List<Card>() ;

        for (int i = 0; i < _suits.Length; i++) { //For every type of suit (D\H\C\S)
            for (int j = 1; j < 14; j++) { //For each value (1-13)
                Card card = new Card(j, _suits[i]); //Creating card
                shuffeledGameDeck.Add(card); //Adding card to startDeck
            }
        }
        shuffeledGameDeck = ShuffleDeck(shuffeledGameDeck);

        // Split deck into two decks (player and comp decks)
        int cardsPerPlayer = Mathf.FloorToInt(TotalCards / PlayerAmount);

        for (int i = 0; i < PlayerAmount; i++) {
            for (int j = 0; j < cardsPerPlayer; j++) {
                Card card = shuffeledGameDeck[j + (cardsPerPlayer * i)];
                card.OwnerPlayerID = i;
                card.Location = Card.Locations.Player;
                _gameData.PlayersCards[i].Add(card);
            }
        }

        _isGameStarted = true;
        MainGameViewRef.UpdateView(_gameData);
    }

    //When called whoGetsIt gets the whole game deck
    public void TakeGameDeck(int whoGetsIt) {
        for (int i = 0; i < _gameData.DeckCards.Count; i++) {
            _gameData.DeckCards[i].Location = Card.Locations.Player;
            _gameData.DeckCards[i].OwnerPlayerID = whoGetsIt;
        }
        _gameData.PlayersCards[whoGetsIt].AddRange(_gameData.DeckCards);
        _gameData.DeckCards.Clear();
        
        _gameData.CurrentTurnPlayer = whoGetsIt;
        //ClearAllPlayGroundTexts();
        //   UpdateCounters();
        MainGameViewRef.UpdateView(_gameData);
    }

    private int SpecialCardTurnsByCard(int card) {
        switch (card) {
            case 12:
                return 2;
            case 13:
                return 3;
            case 1:
                return 4;
            default:
                return 1;

        }
    }

    /// One turn:
    ///  - Checking whos turn is it
    ///  - Checking previous card
    ///  - if previous card was a special card:
    ///     - SpecialCardCoroutine
    ///  - else: Use 1 card
    /// <param name="playerNum"> player number </param>
    public void GameTurn(int playerNum) {
        //Checking winning condition
        if (IsPlayerLost(playerNum)) {
            GameOver(playerNum);
            return;
        }


    //    ClearTexts(playerNum);
        int nextPlayer = (playerNum + 1) % PlayerAmount;
        int prevPlayer = (playerNum - 1) < 0 ? PlayerAmount - 1 : playerNum - 1;
        if (_gameData.LastCardVal == 1 || _gameData.LastCardVal > 10) {
            StartCoroutine(SpecialCardCoroutine(playerNum, prevPlayer, SpecialCardTurnsByCard(_gameData.LastCardVal)));
        } else {
            UseCard(playerNum, 0);
            _gameData.CurrentTurnPlayer = nextPlayer;
        }
    }

    //Using one card (adding it to gameDeck and removing it from player's deck)
    private void UseCard(int playerNum, int locationInPlayground) {
        if (playerNum > PlayerAmount - 1) {
            Debug.LogWarning("There is no such player: " + playerNum);
            return;
        }



        for (int i = 0; i < _gameData.DeckCards.Count; i++) {
            if (_gameData.DeckCards[i].OwnerPlayerID == playerNum && _gameData.DeckCards[i].Location == Card.Locations.Playground) {
                if (locationInPlayground == 0) {
                    _gameData.DeckCards[i].Location = Card.Locations.Deck;
                }
            }
        }
       


        Card card = _gameData.PlayersCards[playerNum][0];
        card.IndexInPlayground = locationInPlayground;
        card.Location = Card.Locations.Playground;
        _gameData.DeckCards.Insert(0, card);
        Debug.Log(string.Format("Player {0} Used: {1}", playerNum, _gameData.PlayersCards[playerNum][0]));
        _gameData.PlayersCards[playerNum].RemoveAt(0);
        MainGameViewRef.UpdateView(_gameData);
        if (CheckDuplicate()) {
            StartCoroutine(DuplicationCoroutine());
        }
    }

    //Check if two same cards used 
    private bool CheckDuplicate() {
        if(_gameData.DeckCards.Count < 2) {
            return false;
        }

        return _gameData.LastCardVal == _gameData.DeckCards[1].value;
    }



    //If player playerNum lost
    private bool IsPlayerLost(int playerNum) {
        return (_gameData.CurrentTurnPlayer == playerNum && _gameData.PlayersCards[playerNum].Count == 0);
    }


    //Changing level and setting Winningcounters
    private void GameOver(int playerNum) {

        int wonPlayerNum = (playerNum - 1) < 0 ? PlayerAmount - 1 : playerNum - 1;
        GameStats.WinningPlayer = wonPlayerNum.ToString();
        GameStats.WinningCounters[wonPlayerNum] += 1;
        PlayerPrefs.SetInt(wonPlayerNum.ToString(), GameStats.WinningCounters[wonPlayerNum]);
        SceneManager.LoadScene("WinScene");
    }

    // for each card;
    // - Draw a card
    // - Check if larger than 11, if so- take deck
    // - if at end of loop, didn't take deck -> other player takes it
    private IEnumerator SpecialCardCoroutine(int curPlayer, int prevPlayer, int numOfCardsToCheck) {
        _waitingForCoroutine = true;

        bool tookDeck = false;

        float startTime;

        for (int i = 0; i < numOfCardsToCheck; i++) {
            if (IsPlayerLost(curPlayer)) {
                GameOver(curPlayer);
                break;
            }
            UseCard(curPlayer, i);
            if(CheckDuplicate()) {
                break;
            }

            if (_gameData.DeckCards[0].value >= 11) {

                startTime = Time.time;
                while (!_isGameRestarted && Time.time - startTime < 1) {
                    yield return null;
                }
                if (_isGameRestarted) {
                    _waitingForCoroutine = false;
                    yield break;
                }


                TakeGameDeck(curPlayer); // Player 1 gets the Deck
                tookDeck = true;
                break;
            }

            startTime = Time.time;
            while (!_isGameRestarted && Time.time - startTime < 1) {
                yield return null;
            }
            if (_isGameRestarted) {
                _waitingForCoroutine = false;
                yield break;
            }
        }
        if (!tookDeck) {
            startTime = Time.time;
            while (!_isGameRestarted && Time.time - startTime < 1) {
                yield return null;
            }
            if (_isGameRestarted) {
                _waitingForCoroutine = false;
                yield break;
            }
            TakeGameDeck(prevPlayer); // Player 2 gets the Deck
        }

        _waitingForCoroutine = false;
    }


    //If called, Duplication were found
    private IEnumerator DuplicationCoroutine() {
            _waitingForCoroutine = true;
        //yield on a new YieldInstruction that waits for 5 seconds.
        float startTime = Time.time;
        while (!_isGameRestarted && Time.time - startTime < 1) {
            yield return null;
        }
        if(_isGameRestarted) {
            _waitingForCoroutine = false;
            yield break;
        }
 
        Debug.Log("Found Duplication!!!");
            int rnd = UnityEngine.Random.Range(0, PlayerAmount);
            Debug.Log("Random Number between 0 to 1: " + rnd.ToString());

            //Player takes the deck
            TakeGameDeck(rnd);
            //Computer takes the deck
            _waitingForCoroutine = false;
        }




    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    /*                BUTTONS HANDLER                    */

    public void StartTheGameBtn() {
        if (!_waitingForCoroutine && !_isGameStarted) {
            StartGame();
        }
    }

    public void PlayTurnBtn() {
        if (_isGameStarted) {

            if (!_waitingForCoroutine) {
                _isGameRestarted = false;
                _gameData.PlayersCards[_gameData.CurrentTurnPlayer] = ShuffleDeck(_gameData.PlayersCards[_gameData.CurrentTurnPlayer]);

                GameTurn(_gameData.CurrentTurnPlayer);
            }
        }
 
    }

    public void RestartGameBtn() {
        InitializeGameData();
        if (!_waitingForCoroutine) {
            _isGameRestarted = false;
        }
        StartGame();
        Debug.Log("GAME RESTARTED");
    }

}
