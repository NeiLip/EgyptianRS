using System.Collections.Generic;

public class GameData  {
    public List<Card> DeckCards;
    public List<List<Card>> PlayersCards;
    public int CurrentTurnPlayer;

    public Card LastCard {
        get {
            Card retVal = null;
            if (DeckCards.Count > 0) {
                retVal = DeckCards[0];
            }
            return retVal;
        }
    }

    public int LastCardVal {
        get {
            int retVal = 0;
            if (LastCard != null) {
                retVal = LastCard.value;
            }
            return retVal;
        }
    }

    public GameData(int numOfPlayers) {
        DeckCards = new List<Card>();
        PlayersCards = new List<List<Card>>();
        for (int i = 0; i < numOfPlayers; i++) {
            PlayersCards.Add(new List<Card>());
        }
        CurrentTurnPlayer = 0;
    }
}
