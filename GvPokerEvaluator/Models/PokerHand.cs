namespace GvPokerEvaluator.Models;

public enum HandRank
{
    HighCard,
    OnePair,
    ThreeOfAKind,
    Flush
}

public record PokerHand (SortedSet<Card> Cards, HandRank HandRank, string PlayerId) 
{
    public override string ToString() => $"{PlayerId}: {string.Join(", ", Cards)}.  Hand Rank: {HandRank}";
}