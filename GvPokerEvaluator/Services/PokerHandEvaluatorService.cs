using GvPokerEvaluator.Models;

namespace GvPokerEvaluator.Services;

public class PokerHandEvaluatorService : IHandEvaluatorService
{
    private readonly PokerHandComparer _pokerHandComparer;

    public PokerHandEvaluatorService(PokerHandComparer pokerHandComparer)
    {
        _pokerHandComparer = pokerHandComparer;
    }

    /// <summary>
    /// Evaluates hand strength
    /// ASSUMPTIONS: we are not going to validate correct hands (only if empty), such as 5 same cards, we are assuming deck is valid
    /// </summary>
    /// <param name="cards"></param>
    /// <returns>HandRank</returns>
    /// <exception cref="ArgumentException">throws if cards is empty</exception>
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
        // Also, ASSUMPTION: we don't care if 4 of kind could lose to a higher 3 of a kind
        if (cardGroups.Any(cg => cg.Count() >= 3))
            return HandRank.ThreeOfAKind;

        if (cardGroups.Any(cg => cg.Count() == 2))
            return HandRank.OnePair;
        
        return HandRank.HighCard;
    }

    public PokerHand GetPokerHand(SortedSet<Card> cards, string playerId)
    {
        var evaluation = GetHandRank(cards);

        return new PokerHand(cards!, evaluation, playerId);
    }

    /// <summary>
    /// Compares poker hands
    /// </summary>
    /// <param name="pokerHands"></param>
    /// <returns>set of winning hands</returns>
    /// <exception cref="ArgumentException">Expects more then 1 hand</exception>
    public IEnumerable<PokerHand> GetWinningHands(List<PokerHand> pokerHands)
    {
        if (pokerHands.Count < 2)
            throw new ArgumentException("need at least 2 players", nameof(pokerHands));
        
        var handSize = pokerHands.First().Cards.Count;

        if (pokerHands.Any(ph => ph.Cards.Count != handSize))
            throw new ArgumentException("poker hands should be of equal size", nameof(pokerHands));

        if (!pokerHands.SelectMany(ph => ph.Cards).All(new HashSet<Card>().Add))
            throw new ArgumentException("the deck should not have duplicate cards", nameof(pokerHands));
        
        var groupedHands = 
            pokerHands
                .GroupBy(p => p.HandRank)
                .OrderByDescending(p => p.Key)
                .ToList();

        var winningHands = groupedHands.First();
        
        if (winningHands.Count() == 1)
            return winningHands;
        
        // deal with potential ties by checking if there are multiple of winning hand
        // and if there are compare each for high kicker/high flush card
        var bestHand = winningHands.Max(_pokerHandComparer);
        
        return winningHands.Where(wh => _pokerHandComparer.Compare(wh, bestHand) == 0);
    }
}