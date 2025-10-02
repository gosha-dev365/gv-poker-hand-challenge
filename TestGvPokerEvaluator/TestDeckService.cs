using GvPokerEvaluator.Models;
using GvPokerEvaluator.Services;

namespace TestGvPokerEvaluator;

public class TestDeckService
{
    [Fact(DisplayName = "Should not fail to initialize")]
    public void SanityCheck()
    {
        // shouldn't throw
        var deck = new DeckServiceService();
    }

    [Fact(DisplayName = "Should contain 52 unique cards")]
    public void ShouldBeAStandardDeck()
    {
        var service = new DeckServiceService();
        var cards = new List<Card>();
        
        while (service.Draw() is { } card )
        {
            cards.Add(card);
        }
        
        Assert.Equal(52, cards.Count);
        Assert.Equal(52, cards.Distinct().Count());
    }
}
