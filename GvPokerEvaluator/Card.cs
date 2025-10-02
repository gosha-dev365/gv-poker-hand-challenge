using System.Reflection;

namespace GvPokerEvaluator;

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

    public int CompareTo(Card? other)
    {
        if (other is null)
            return 1;

        return (Rank == other.Rank)
            ? Suit.CompareTo(other.Suit)
            : Rank.CompareTo(other.Rank);
    }
}