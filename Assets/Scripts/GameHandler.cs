﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    //STATICs HERE:
    private static int[] startValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13};
    private static string[] suits = {"D", "H", "C", "S" };

    private static List<Card> playerDeck; // Player's deck
    private static List<Card> compDeck; // Computer's deck
    private static List<Card> gameDeck; // Game's deck (all cards that players put on field)


    private static List<Card> startGameDeck; //Unshuffled deck of 52 cards


  //  private static Text[] playersTexts; 
    private static Text[] deckPlayerTexts; // Array of texts for players's field cards
    private static Text[] deckCompTexts; // Array of texts for computer's field cards

    private static Text whosTurnText;

    private static Text[] counters; //// Array of texts for counters

    private static bool isGameStarted;
    private static int curTurn; //whos turn is it (0 for player, 1 for Comp)

    private static bool waitingForCoroutine;


    //Initializig 
    private void Awake() {
        isGameStarted = false;
        startGameDeck = new List<Card>();
        playerDeck = new List<Card>();
        compDeck = new List<Card>();
        gameDeck = new List<Card>();
      //  playersTexts = new Text[2]; // In future - Set this as the amount of players
        deckPlayerTexts = new Text[4]; // player: max cards on deck -> {card1, card2, card3, card4}
        deckCompTexts = new Text[4]; //  computer: max cards on deck -> {card1, card2, card3, card4}
        curTurn = 0;

        counters = new Text[3]; //{Player, Computer, GameDeck}
        waitingForCoroutine = false;
    }


    
    //Shuffle a deck
    private static List<Card> ShuffleDeck(List<Card> deck) {
       // Random random = new Random();
        
        int n = deck.Count;

        for (int i = deck.Count - 1; i > 1; i--) {
            int rnd = Random.Range(0,i + 1);

            Card value = deck[rnd];
            deck[rnd] = deck[i];
            deck[i] = value;
        }

        return deck;
    }


    //Building a new Deck
    private static void BuildStartingDeck() {
        int curPlace = 0;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 13; j++) {
                Card card = new Card(startValues[j], suits[i]);
                startGameDeck.Add(card);
                curPlace++;
            }
        }
    }

    // On game Start
    // Shuffle main deck and then split to hands and gets all Text GameObjects
    public static void StartGame() {
        BuildStartingDeck();

        List<Card> shuffeledGameDeck;
        shuffeledGameDeck = ShuffleDeck(startGameDeck);

        // Split deck into two decks (player and comp decks)
        for (int i = 0; i < 52; i++) {

            if (i < 26) {
                playerDeck.Add(shuffeledGameDeck[i]);
            }
            else {
                compDeck.Add(shuffeledGameDeck[i]);
            }
        }

        ////Player and Computer decks
        //playersTexts[0] = GameObject.Find("PlayerText").GetComponent<Text>();
        //playersTexts[1] = GameObject.Find("CompText").GetComponent<Text>();


        //Player PLAYGROUND TEXT
        deckPlayerTexts[0] = GameObject.Find("CurText1").GetComponent<Text>();
        deckPlayerTexts[1] = GameObject.Find("CurText2").GetComponent<Text>();
        deckPlayerTexts[2] = GameObject.Find("CurText3").GetComponent<Text>();
        deckPlayerTexts[3] = GameObject.Find("CurText4").GetComponent<Text>();


        // COMPUTER PLAYGROUND TEXT
        deckCompTexts[0] = GameObject.Find("CompCurText1").GetComponent<Text>();
        deckCompTexts[1] = GameObject.Find("CompCurText2").GetComponent<Text>();
        deckCompTexts[2] = GameObject.Find("CompCurText3").GetComponent<Text>();
        deckCompTexts[3] = GameObject.Find("CompCurText4").GetComponent<Text>();

        //COUNTERS TEXT
        counters[0] = GameObject.Find("PlayerCounter").GetComponent<Text>();
        counters[1] = GameObject.Find("CompCounter").GetComponent<Text>();
        counters[2] = GameObject.Find("GameDeckCounter").GetComponent<Text>();


        //WHOS TURN TEXT
        whosTurnText = GameObject.Find("WhosTurn").GetComponent<Text>();

        ClearTexts();

    }


    // Clears Player's and Computer's playground
    private static void ClearTexts() {
        deckPlayerTexts[0].text = "";
        deckPlayerTexts[1].text = "";
        deckPlayerTexts[2].text = "";
        deckPlayerTexts[3].text = "";

        deckCompTexts[0].text = "";
        deckCompTexts[1].text = "";
        deckCompTexts[2].text = "";
        deckCompTexts[3].text = "";
       
    }



    //When called whoGetsIt gets the whole game deck
    public static void TakeGameDeck(int whoGetsIt) {
        if (whoGetsIt == 1) { //player gets deck
            while(gameDeck.Count > 0) {
                playerDeck.Add(gameDeck[0]);
                gameDeck.RemoveAt(0);
            }
            playerDeck = ShuffleDeck(playerDeck);
        }

        if (whoGetsIt == 2) { //player gets deck
            while (gameDeck.Count > 0) {
                compDeck.Add(gameDeck[0]);
                gameDeck.RemoveAt(0);
            }
            compDeck = ShuffleDeck(compDeck);
        }
        ClearTexts();
    }



    /// One turn
    /// 
    /// <param name="playerNum"> player number </param>
    /// <param name="lastCardVal"> value of last card used </param>
    public void GameTurn(int playerNum, int lastCardVal) {
        if (playerNum == 1) { // Player turn
            bool tookDeck = false;
            //Took Jocker
            if (lastCardVal == 11) {
                UseCard(1);

                deckPlayerTexts[0].text = gameDeck[0].ToString();
                deckPlayerTexts[1].text = "";
                deckPlayerTexts[2].text = "";
                deckPlayerTexts[3].text = "";

                StartCoroutine(SpecialCardCoroutine(1, 2, 1));

            }

            //Took Ace
            else if (lastCardVal == 1) {
                UseCard(1);
                UseCard(1);
                UseCard(1);
                UseCard(1);

                deckPlayerTexts[0].text = gameDeck[0].ToString();
                deckPlayerTexts[1].text = gameDeck[1].ToString();
                deckPlayerTexts[2].text = gameDeck[2].ToString();
                deckPlayerTexts[3].text = gameDeck[3].ToString();

                
                StartCoroutine(SpecialCardCoroutine(1, 2, 4));
            }


            //Took Queen
            else if (lastCardVal == 12) {
                UseCard(1);
                UseCard(1);

                deckPlayerTexts[0].text = gameDeck[0].ToString();
                deckPlayerTexts[1].text = gameDeck[1].ToString();
                deckPlayerTexts[2].text = "";
                deckPlayerTexts[3].text = "";

                StartCoroutine(SpecialCardCoroutine(1, 2, 2));
            }
            //Took King
            else if (lastCardVal == 13) {
                UseCard(1);
                UseCard(1);
                UseCard(1);

                deckPlayerTexts[0].text = gameDeck[0].ToString();
                deckPlayerTexts[1].text = gameDeck[1].ToString();
                deckPlayerTexts[2].text = gameDeck[2].ToString();
                deckPlayerTexts[3].text = "";

                StartCoroutine(SpecialCardCoroutine(1, 2, 3));
            }

            // Took card between 2 and 10
            else { 
                UseCard(1);

                deckPlayerTexts[0].text = gameDeck[0].ToString();
                deckPlayerTexts[1].text = "";
                deckPlayerTexts[2].text = "";
                deckPlayerTexts[3].text = "";
            }
        }

        else if ((playerNum == 2)) { //Computer turn
            bool tookDeck = false;

            //Took Jocker
            if (lastCardVal == 11) {
                UseCard(2);

                deckCompTexts[0].text = gameDeck[0].ToString();
                deckCompTexts[1].text = "";
                deckCompTexts[2].text = "";
                deckCompTexts[3].text = "";

                StartCoroutine(SpecialCardCoroutine(2, 1, 1));

            }

            //Took Ace
            else if (lastCardVal == 1) {
                UseCard(2);
                UseCard(2);
                UseCard(2);
                UseCard(2);

                deckCompTexts[0].text = gameDeck[0].ToString();
                deckCompTexts[1].text = gameDeck[1].ToString();
                deckCompTexts[2].text = gameDeck[2].ToString();
                deckCompTexts[3].text = gameDeck[3].ToString();

                StartCoroutine(SpecialCardCoroutine(2, 1, 4));

            }

            //Took Queen
            else if (lastCardVal == 12) {
                UseCard(2);
                UseCard(2);

                deckCompTexts[0].text = gameDeck[0].ToString();
                deckCompTexts[1].text = gameDeck[1].ToString();
                deckCompTexts[2].text = "";
                deckCompTexts[3].text = "";


                StartCoroutine(SpecialCardCoroutine(2, 1, 2));
            }

            //Took King
            else if (lastCardVal == 13) {
                UseCard(2);
                UseCard(2);
                UseCard(2);

                deckCompTexts[0].text = gameDeck[0].ToString();
                deckCompTexts[1].text = gameDeck[1].ToString();
                deckCompTexts[2].text = gameDeck[2].ToString();
                deckCompTexts[3].text = "";

                StartCoroutine(SpecialCardCoroutine(2, 1, 3));
            }

            // Took card between 2 and 10
            else { 
                UseCard(2);

                deckCompTexts[0].text = gameDeck[0].ToString();
                deckCompTexts[1].text = "";
                deckCompTexts[2].text = "";
                deckCompTexts[3].text = "";
            }
        }


        //Setting counters texts
        UpdateCounters();


        //Whos next turn
        if (curTurn == 0) {
            whosTurnText.text = "Computer";
        }
        else if (curTurn == 1) {
            whosTurnText.text = "Player";
        }

    }

    //Updating counters
    private void UpdateCounters() {
        counters[0].text = playerDeck.Count.ToString();
        counters[1].text = compDeck.Count.ToString();
        counters[2].text = gameDeck.Count.ToString();
    }

    private void UseCard(int playerNum) {
        if (playerNum == 1) {
            gameDeck.Insert(0, playerDeck[0]);
            Debug.Log("Player Used: " + playerDeck[0].ToString());
            playerDeck.RemoveAt(0);

        }
        if (playerNum == 2) {
            gameDeck.Insert(0, compDeck[0]);
            Debug.Log("Computer Used: " + compDeck[0].ToString());
            compDeck.RemoveAt(0);
        }
        CheckDuplicate();
        
    }


    private void CheckDuplicate() {
        int thisCard = 0;
        int prevCard = 1;

        if (gameDeck.Count > 1) {
            thisCard = gameDeck[0].GetValue();
            prevCard = gameDeck[1].GetValue();
        }


       
        // Two same cards! Someone will take the Deck
        if (thisCard == prevCard) {
            StartCoroutine(DuplicationCoroutine());
        }
    }



    private void Update() {
        if (waitingForCoroutine == false) { // Checking if waiting for coroutine, if so-> don't get in
            if (Input.GetKeyDown(KeyCode.Space)) {
                StartGame();
                isGameStarted = true;
            }

            if (Input.GetKeyDown(KeyCode.A) && (curTurn == 0)) {
                //Shuffling Player Deck
                playerDeck = ShuffleDeck(playerDeck);

                int lastCard;
                if (gameDeck.Count > 0) {
                    lastCard = gameDeck[0].GetValue();
                }
                else {
                    lastCard = 0; //Null card
                }

                //Debug.Log("1: last card: " + lastCard);

                GameTurn(1, lastCard);

                curTurn = 1;
            }


            if (Input.GetKeyDown(KeyCode.S) && (curTurn == 1)) {
                //Shuffling Player Deck
                compDeck = ShuffleDeck(compDeck);

                int lastCard;
                if (gameDeck.Count > 0) {
                    lastCard = gameDeck[0].GetValue();
                }
                else {
                    lastCard = 0; //Null card
                }

                GameTurn(2, lastCard);

                curTurn = 0;
            }
        }
        if (isGameStarted) {
            UpdateCounters(); //Updating counters

            //Check win condition
            if (playerDeck.Count == 0 || compDeck.Count == 0) {
                SceneManager.LoadScene("WinScene");
            }
        }  
    }



    private IEnumerator SpecialCardCoroutine(int curPlayer, int otherPlayer, int numOfCardsToCheck) {
        waitingForCoroutine = true;
        Debug.Log("Start courtime: ");

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);

        bool tookDeck = false;
        //Who will take the deck?
        for (int i = 0; i < numOfCardsToCheck; i++) {
            Debug.Log("Checking exception: " + gameDeck[i].ToString());
            if ((gameDeck[i].GetValue() >= 11) && (gameDeck.Count > 0) && (i < gameDeck.Count)) {
                TakeGameDeck(curPlayer); // Player 1 gets the Deck
                tookDeck = true;
                break;
            }
        }
        if (tookDeck == false) {
            TakeGameDeck(otherPlayer); // Player 2 gets the Deck
        }
        Debug.Log("End courtime: ");
        waitingForCoroutine = false;
    }

    //If called, Duplication were found
    private IEnumerator DuplicationCoroutine() {
        waitingForCoroutine = true;
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);

        
        Debug.Log("Found Duplication!!!");
        int rnd = Random.Range(0, 2);
        Debug.Log("Random Number between 0 to 1: " + rnd.ToString());

        //Player takes the deck
        if (rnd == 0) {
            TakeGameDeck(1);
        }
        //Computer takes the deck
        else {
            TakeGameDeck(2);
        }
        waitingForCoroutine = false;
    }

}