using System.Collections.ObjectModel;

namespace GvPokerEvaluator;

public class HandEvaluator
{
    /// <summary>
    /// Evaluates hand strength
    /// </summary>
    /// <param name="cards"></param>
    /// <returns>HandRank</returns>
    /// <exception cref="ArgumentException">throws if collection is empty</exception>
    public HandRank GetHandRank(SortedSet<Card> cards)
    {
        if (cards.Count == 0)
            throw new ArgumentException("Can't play with an empty hand", nameof(cards));

        // Check for flash first, then we don't have to continue with grouping
        if (cards.All(card => card.Suit == cards.First().Suit))
            return HandRank.Flush;

        var cardGroups = cards.GroupBy(card => card.Rank).ToList();

        // Using equals or greater then to catch 4 of a kind, hopefully we don't have 5 of a kind :)
        // but deck verification should be done on the deck.
        // Also, assumption: we don't care if 4 of kind could lose to a higher 3 of a kind
        if (cardGroups.Any(cg => cg.Count() >= 3))
            return HandRank.ThreeOfAKind;

        if (cardGroups.Any(cg => cg.Count() == 2))
            return HandRank.OnePair;
        
        return HandRank.HighCard;
    }

    public IEnumerable<PokerHand> GetWinningHand(IEnumerable<PokerHand> pokerHands)
    {
        var groupedHands = 
            pokerHands
                .GroupBy(p => p.HandRank)
                .OrderByDescending(p => p.Key)
                .ToList();

        var winningHands = groupedHands.First();
        
        if (winningHands.Count() == 1)
            return winningHands;
        
        // deal with potential ties
        var bestHand = winningHands.Max();

        return winningHands.Where(wh => wh.CompareTo(bestHand) == 0);
    }
}