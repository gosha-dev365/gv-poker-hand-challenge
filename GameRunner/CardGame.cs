using GvPokerEvaluator.Models;
using GvPokerEvaluator.Services;
using Microsoft.Extensions.Logging;

namespace GameRunner;

/// <summary>
/// Game runner, ideally can run different types of games and should/could be moved to a separate prj 
/// </summary>
public class CardGame
{
    private readonly IDeckService _deckService;
    private readonly IHandEvaluatorService _handEvaluatorService;
    private readonly ILogger _logger;

    public CardGame(IDeckService deckService, IHandEvaluatorService handEvaluatorService, ILogger logger)
    {
        _deckService = deckService;
        _handEvaluatorService = handEvaluatorService;
        _logger = logger;
    }

    /// <summary>
    /// Creates and runs game for set of players, in the future this can be refactored to a granular steps,
    /// create hands, return winning hand(s) etc.
    /// </summary>
    /// <param name="players"></param>
    /// <returns>Name(s) of the winning player</returns>
    public string CreateAndRunGame(IEnumerable<string> players)
    {
        try
        {
            var playerHands = new List<PokerHand>();

            foreach (var player in players)
            {
                var cards = new SortedSet<Card?>(Enumerable.Range(0, 5).Select(_ => _deckService.Draw()));

                if (cards.Any(card => card is null))
                    throw new Exception("deck is invalid, has null cards or not full");
                
                var playerHand = _handEvaluatorService.GetPokerHand(cards!, player);

                playerHands.Add(playerHand);

                _logger.LogInformation(playerHand.ToString());
            }

            var winningHands = _handEvaluatorService.GetWinningHands(playerHands).ToList();
            
            return winningHands.Count == 1 ? winningHands.First().PlayerId : string.Join(", ", winningHands);
        }
        catch (Exception e)
        {
            // Log detailed error here
            _logger.LogError(e, "Unexpected error");
            throw;
        }
    }
}