using GvPokerEvaluator.Models;

namespace GvPokerEvaluator.Services;

public interface IDeckService
{
    /// <summary>
    /// Sets the deck to new 52 cards
    /// </summary>
    public void Reset();

    /// <summary>
    /// Draws card from the deck
    /// </summary>
    /// <returns>Card, null if the deck is empty</returns>
    public Card? Draw();
}
