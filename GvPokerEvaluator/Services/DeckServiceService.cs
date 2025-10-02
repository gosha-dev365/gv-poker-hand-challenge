using GvPokerEvaluator.Models;

namespace GvPokerEvaluator.Services;

public class DeckServiceService : IDeckService
{
    private List<Card>? _cards;

    public DeckServiceService()
    {
        Reset();
    }

    public Card? Draw()
    {
        if (_cards?.Count == 0)
        {
            return null;
        }
        
        var cardIndex = new Random().Next(0, _cards!.Count-1);
        
        var card = _cards[cardIndex];
        
        _cards.RemoveAt(cardIndex);
        
        return card;
    }
    
    public void Reset() => _cards = Enumerable.Range(0, 13)
            .SelectMany(rank => Enumerable.Range(0,4)
                .Select(suit => new Card((Suit)suit, (Rank)rank)))
            .ToList();
    
}
