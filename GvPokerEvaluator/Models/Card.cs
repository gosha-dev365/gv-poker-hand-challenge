namespace GvPokerEvaluator.Models;

public enum Suit { Spades, Clubs, Diamonds, Hearts }

public enum Rank
{
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,      
    Queen,     
    King,
    Ace,
}

public record Card (Suit Suit, Rank Rank) : IComparable<Card> {
    
    public override string ToString() => $"{Rank} of {Suit}";

    // This is fairly simple comparison so keeping in model instead of adding comparer service
    public int CompareTo(Card? other)
    {
        if (other is null)
            return 1;

        return (Rank == other.Rank)
            ? Suit.CompareTo(other.Suit)
            : Rank.CompareTo(other.Rank);
    }
}