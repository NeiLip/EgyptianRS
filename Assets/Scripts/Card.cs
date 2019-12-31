using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {


    private int value;
    private string suit;


    //Creating 'Null' card
    //!@!
    public Card() {
        this.value = 0;
        this.suit = "NULL CARD";
    }

    //Creating Card 
    public Card(int value, string suit) {
        this.suit = suit;
        this.value = value;
    }

    public void SetValue(int value) {
         this.value = value;
    }

    public void SetSuit(string suit) {
        this.suit = suit;
    }

    public int GetValue() {
        return this.value;
    }

    public string GetSuit() {
        return this.suit;
    }
    //!@!
    public string ToString() {
        return this.value.ToString() + " " + this.suit;
    }
}
