using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameHandler : MonoBehaviour
{
    //STATICs HERE:
    //private static int[] startValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13};
    private string[] _suits = { "D", "H", "C", "S" };
    private const int CARDS_PER_SUIT = 13;
    private int TotalCards {
        get {
            return _suits.Length * CARDS_PER_SUIT;
        }
    }
    private Card LastCard {
        get {
            Card retVal = null;
            if(_gameDeck.Count > 0) {
                retVal = _gameDeck[0];
            }
            return retVal;
        }
    }
    private int LastCardVal {
        get {
            int retVal = 0;
            if(LastCard != null) {
                retVal = LastCard.GetValue();
            }
            return retVal;
        }
    }

    private List<List<Card>> _playersCardsList;
    private List<Card> _gameDeck; // Game's deck (all cards that players put on field)

    private List<Card> _startGameDeck; //Unshuffled deck of 52 cards

    //  private static Text[] playersTexts;
    public PlayersPlaygroundContainer[] PlayersPlaygroundContainers;

    [Serializable]
    public class PlayersPlaygroundContainer {
        public Text[] PlayersPlaygroundTexts;
    }

    public Text WhosTurnText;
    public Text[] PlayersCardsCountersTexts; // Array of texts for counters
    public Text GameDeckCardsCounterText;

    private bool _isGameStarted;
    private int _whosTurnIsIt; //Whos turn is it (0 for player, 1 for Comp)

    private bool _waitingForCoroutine; //If true, everything is disabled. If false, player can play a turn

    private string _winningPlayer;

    private bool _isGameRestarted;


    //Initializig variables
    private void Awake() {
        _waitingForCoroutine = false;
        InitializeStats();
    }


    private void InitializeStats() {
        _isGameRestarted = true;
        _isGameStarted = false;
        _startGameDeck = new List<Card>();
        _playersCardsList = new List<List<Card>>();
        _playersCardsList.Add(new List<Card>());
        _playersCardsList.Add(new List<Card>());
        _gameDeck = new List<Card>();


        _whosTurnIsIt = 0;
        _winningPlayer = "Player 1";
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


    //Building a new Deck
    private void BuildStartingDeck() {
        _startGameDeck.Clear();
        for (int i = 0; i < _suits.Length; i++) { //For every type of suit (D\H\C\S)
            for (int j = 1; j < 14; j++) { //For each value (1-13)
                Card card = new Card(j, _suits[i]); //Creating card
                _startGameDeck.Add(card); //Adding card to startDeck
            }
        }
    }

    // On game Start
    // Shuffle main deck and then split to hands and gets all Text GameObjects
    public void StartGame() {
        BuildStartingDeck();
        List<Card> shuffeledGameDeck;
        shuffeledGameDeck = ShuffleDeck(_startGameDeck);

        // Split deck into two decks (player and comp decks)
        int cardsPerPlayer = Mathf.FloorToInt(TotalCards / _playersCardsList.Count);

        for (int i = 0; i < _playersCardsList.Count; i++) {
            for (int j = 0; j < cardsPerPlayer; j++) {
                _playersCardsList[i].Add(shuffeledGameDeck[j + (cardsPerPlayer * i)]);
            }
        }

        _isGameStarted = true;
        ClearAllPlayGroundTexts();
    }


    // Clears Player's and Computer's playground
    private void ClearAllPlayGroundTexts() {
        for (int i = 0; i < PlayersPlaygroundContainers.Length; i++) {
            for (int j = 0; j < PlayersPlaygroundContainers[i].PlayersPlaygroundTexts.Length; j++) {
                PlayersPlaygroundContainers[i].PlayersPlaygroundTexts[j].text = "";
            }
        }
    }
    /// Clears specific texts
    /// <param name="player"> player to clear his playground texts </param>
    private void ClearTexts(int player) {
        for (int i = 0; i < PlayersPlaygroundContainers[player].PlayersPlaygroundTexts.Length; i++) {
            PlayersPlaygroundContainers[player].PlayersPlaygroundTexts[i].text = "";
        }
    }


    //When called whoGetsIt gets the whole game deck
    public void TakeGameDeck(int whoGetsIt) {
        _playersCardsList[whoGetsIt].AddRange(_gameDeck);
        _gameDeck.Clear();
        
        _whosTurnIsIt = whoGetsIt;
        ClearAllPlayGroundTexts();
        UpdateCounters();
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
            GameOver();
            return;
        }


        ClearTexts(playerNum);
        int nextPlayer = (playerNum + 1) % _playersCardsList.Count;
        int prevPlayer = (playerNum - 1) < 0 ? _playersCardsList.Count - 1 : playerNum - 1;
        if (LastCardVal == 1 || LastCardVal > 10) {
            StartCoroutine(SpecialCardCoroutine(playerNum, prevPlayer, SpecialCardTurnsByCard(LastCardVal)));
        } else {
            UseCard(playerNum, 0);
            _whosTurnIsIt = nextPlayer;
        }
    }

    //Updating counters
    private void UpdateCounters() {
        for (int i = 0; i < PlayersCardsCountersTexts.Length; i++) {
            PlayersCardsCountersTexts[i].text = _playersCardsList[i].Count.ToString();
        }
        GameDeckCardsCounterText.text = _gameDeck.Count.ToString();

        WhosTurnText.text = "Player " + _whosTurnIsIt;
    }

    //Using one card (adding it to gameDeck and removing it from player's deck)
    private void UseCard(int playerNum, int cardSeriesNum) {
        if (playerNum > _playersCardsList.Count - 1) {
            Debug.LogWarning("There is no such player: " + playerNum);
            return;
        }
        _gameDeck.Insert(0, _playersCardsList[playerNum][0]);
        Debug.Log(string.Format("Player {0} Used: {1}", playerNum, _playersCardsList[playerNum][0].ToString()));
        _playersCardsList[playerNum].RemoveAt(0);
        PlayersPlaygroundContainers[playerNum].PlayersPlaygroundTexts[cardSeriesNum].text = LastCard.ToString();
        UpdateCounters();

        if (CheckDuplicate()) {
            StartCoroutine(DuplicationCoroutine());
        }
    }

    //Check if two same cards used 
    private bool CheckDuplicate() {
        if(_gameDeck.Count < 2) {
            return false;
        }
       
        return LastCardVal == _gameDeck[1].GetValue();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            PlayTurnBtn();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            RestartGameBtn();
        }
    }

    //If player playerNum lost
    private bool IsPlayerLost(int playerNum) {
        return (_whosTurnIsIt == playerNum && _playersCardsList[playerNum].Count == 0);
    }

    private void GameOver() {
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
                GameOver();
                break;
            }
            UseCard(curPlayer, i);
            if(CheckDuplicate()) {
                break;
            }

            if (_gameDeck[0].GetValue() >= 11) {

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
            int rnd = UnityEngine.Random.Range(0, _playersCardsList.Count);
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
                _playersCardsList[_whosTurnIsIt] = ShuffleDeck(_playersCardsList[_whosTurnIsIt]);

                GameTurn(_whosTurnIsIt);
            }
        }
 
    }

    public void RestartGameBtn() {
        InitializeStats();
        ClearAllPlayGroundTexts();
        if (!_waitingForCoroutine) {
            _isGameRestarted = false;
        }
        StartGame();
        UpdateCounters();

        Debug.Log("GAME RESTARTED");
    }

}
