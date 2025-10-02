using GvPokerEvaluator.Models;

namespace GvPokerEvaluator.Services;

public interface IHandEvaluatorService
{
    /// <summary>
    /// Evaluates hand strength
    /// </summary>
    /// <param name="cards"></param>
    /// <returns>HandRank</returns>
    /// <exception cref="ArgumentException">throws if collection is empty</exception>
    HandRank GetHandRank(SortedSet<Card> cards);

    /// <summary>
    /// Helper method to quickly initialize poker hand
    /// </summary>
    /// <param name="cards">cards for player</param>
    /// <param name="playerId">player name for tracking</param>
    /// <returns>poker hand</returns>
    PokerHand GetPokerHand(SortedSet<Card> cards, string playerId);

    IEnumerable<PokerHand> GetWinningHands(List<PokerHand> pokerHands);
}