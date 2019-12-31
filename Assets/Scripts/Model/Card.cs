
public class Card {

    public enum Locations {
        Deck,
        Player,
        Playground
    }

    public int value;
    public string suit;

    public int IndexInPlayground;
    public int OwnerPlayerID;

    public Locations Location;

    public Card(int value, string suit) {
        this.suit = suit;
        this.value = value;
        this.IndexInPlayground = -1;
        this.OwnerPlayerID = -1;
        this.Location = Locations.Deck;
    }

    public override string ToString() {
        return this.value.ToString() + " " + this.suit;
    }
}
