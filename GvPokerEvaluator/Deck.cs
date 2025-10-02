namespace GvPokerEvaluator;

public class Deck
{
    private readonly List<Card> _cards;

    public Deck()
    {
        _cards = Enumerable.Range(0, 13)
            .SelectMany(rank => Enumerable.Range(0,4)
                .Select(suit => new Card((Suit)suit, (Rank)rank)))
            .ToList();
    }

    public Card Draw()
    {
        if (_cards.Count == 0)
        {
            throw new Exception("Out of cards");
        }
        
        var cardIndex = new Random().Next(0, _cards.Count-1);
        
        var card = _cards[cardIndex];
        
        _cards.RemoveAt(cardIndex);
        
        return card;
    } 
}