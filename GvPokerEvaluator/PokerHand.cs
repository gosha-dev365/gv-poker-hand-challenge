namespace GvPokerEvaluator;

public enum HandRank
{
    HighCard,
    OnePair,
    ThreeOfAKind,
    Flush
}

// Assumptions: hands are always same length 
public record PokerHand (SortedSet<Card> Cards, HandRank HandRank, string PlayerId) : IComparable<PokerHand>
{
    public int CompareTo(PokerHand? other)
    {
        if (other is null)
            return 1;

        if (HandRank != other.HandRank)
            return HandRank.CompareTo(other.HandRank);

        return Cards.Reverse()
            // Compare hands for high card
            .Zip(other.Cards.Reverse(), (a, b) => a.CompareTo(b))
            // if no high card found the hands will split the pot
            .FirstOrDefault(highCard => highCard != 0, 0);
    }

    public override string ToString() => $"{PlayerId}: {string.Join(", ", Cards)}.  Hand Rank: {HandRank}";
}