using GvPokerEvaluator.Models;
using GvPokerEvaluator.Services;

namespace TestGvPokerEvaluator;

public class TestPokerHandService
{
    [Fact(DisplayName = "Should not fail to initialize")]
    public void SanityCheck()
    {
        // shouldn't throw
        var service = new PokerHandEvaluatorService(new PokerHandComparer());
    }

    [Fact(DisplayName = "Should throw if hand is empty")]
    public void ShouldThrowIfHandIsInvalid()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        Assert.Throws<ArgumentException>(() => service.GetHandRank(new SortedSet<Card>()));
    }

    [Theory(DisplayName = "Should evaluate correct hand rank")]
    [InlineData(
        HandRank.OnePair,
        Suit.Hearts, Rank.Ten,
        Suit.Spades, Rank.Ten,
        Suit.Clubs, Rank.Five,
        Suit.Diamonds, Rank.Seven,
        Suit.Hearts, Rank.Two)]
    // Test that two pairs evaluates to one pair
    [InlineData(
        HandRank.OnePair,
        Suit.Hearts, Rank.Ten,
        Suit.Spades, Rank.Ten,
        Suit.Clubs, Rank.Five,
        Suit.Diamonds, Rank.Five,
        Suit.Hearts, Rank.Two)]
    [InlineData(
        HandRank.ThreeOfAKind,
        Suit.Hearts, Rank.Four,
        Suit.Clubs, Rank.Four,
        Suit.Diamonds, Rank.Four,
        Suit.Spades, Rank.Ten,
        Suit.Hearts, Rank.Ace)]
    // Test that tree of a kind takes precedence over one pair
    [InlineData(
        HandRank.ThreeOfAKind,
        Suit.Hearts, Rank.Four,
        Suit.Clubs, Rank.Four,
        Suit.Diamonds, Rank.Ten,
        Suit.Spades, Rank.Ten,
        Suit.Hearts, Rank.Ten)]
    // Test that 4 of a kind evaluates to 3 of a kind
    [InlineData(
        HandRank.ThreeOfAKind,
        Suit.Hearts, Rank.Four,
        Suit.Clubs, Rank.Four,
        Suit.Diamonds, Rank.Four,
        Suit.Spades, Rank.Four,
        Suit.Hearts, Rank.Ace)]
    [InlineData(
        HandRank.HighCard,
        Suit.Hearts, Rank.Four,
        Suit.Clubs, Rank.Five,
        Suit.Diamonds, Rank.Six,
        Suit.Spades, Rank.Seven,
        Suit.Hearts, Rank.Ace)]
    [InlineData(
        HandRank.Flush,
        Suit.Hearts, Rank.Four,
        Suit.Hearts, Rank.Five,
        Suit.Hearts, Rank.Six,
        Suit.Hearts, Rank.Seven,
        Suit.Hearts, Rank.Ace)]
    public void ShouldEvaluateCorrectRank(
        HandRank expected,
        Suit s1, Rank r1,
        Suit s2, Rank r2,
        Suit s3, Rank r3,
        Suit s4, Rank r4,
        Suit s5, Rank r5)
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var cards = new List<Card> {
            new(s1, r1), 
            new(s2, r2),
            new(s3, r3), 
            new(s4, r4), 
            new(s5, r5)};
        
        var actual = service.GetHandRank(new SortedSet<Card>(cards));
        
        Assert.Equal(expected, actual);
    }
    
    [Fact(DisplayName = "Throws when less than two players")]
    public void ShouldThrowWhenLessThanTwoHands()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer()); 

        Assert.Throws<ArgumentException>(() => 
            service.GetWinningHands(
                new List<PokerHand>
                {
                    new(new SortedSet<Card>(), HandRank.Flush, "")  
                }));
    }
    
    [Fact(DisplayName = "Throws when less player have different number of cards")]
    public void ShouldThrowWhenPlayersAreCheating()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer()); 

        Assert.Throws<ArgumentException>(() => 
            service.GetWinningHands(
                new List<PokerHand>
                {
                    new(new SortedSet<Card>(new []{ new Card(Suit.Clubs, Rank.Ace) }), 
                        HandRank.Flush, 
                        "P1"),  
                    new(new SortedSet<Card>(new []{ new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Spades, Rank.Ace) }), 
                        HandRank.Flush,
                        "P2")  
                }));
    }

    [Fact(DisplayName = "Should pass example 1 (modified to joe wins), Joe wins with high pair")]
    public void ShouldWinWithHighPair()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());
        var joe = new SortedSet<Card>
        {
            new(Suit.Spades, Rank.Eight), 
            new(Suit.Diamonds, Rank.Eight),
            new(Suit.Diamonds, Rank.Ace),
            new(Suit.Diamonds, Rank.Queen),
            new(Suit.Hearts, Rank.Jack)
        };

        var bob = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Two), 
            new(Suit.Spades, Rank.Three), 
            new(Suit.Clubs, Rank.Five),
            new(Suit.Spades, Rank.Jack), 
            new(Suit.Spades, Rank.King)
        };

        var sally = new SortedSet<Card>
        {
            new(Suit.Spades, Rank.Four), 
            new(Suit.Hearts, Rank.Four), 
            new(Suit.Hearts, Rank.Three),
            new(Suit.Clubs, Rank.Queen),
            new(Suit.Clubs, Rank.Eight)
        }; 
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(joe, nameof(joe)),
            service.GetPokerHand(bob, nameof(bob)),
            service.GetPokerHand(sally, nameof(sally)),
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Single(winners);
        Assert.Equal(nameof(joe), winners.First().PlayerId);
    }
    
    [Fact(DisplayName = "Should pass example 2 (modified replaced duplicate cards), Bob wins with flash")]
    public void ShouldWinWithFlash()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());
        var joe = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Eight), 
            new(Suit.Diamonds, Rank.Eight), 
            new(Suit.Diamonds, Rank.Ace),
            new(Suit.Diamonds, Rank.Queen), 
            new(Suit.Hearts, Rank.Jack)
        };
        
        var bob = new SortedSet<Card>
        {
            new(Suit.Spades, Rank.Ace), 
            new(Suit.Spades, Rank.Queen), 
            new(Suit.Spades, Rank.Eight), 
            new(Suit.Spades, Rank.Six), 
            new(Suit.Spades, Rank.Five) 
        };
        
        var sally = new SortedSet<Card>
        {
            new(Suit.Spades, Rank.Four), 
            new(Suit.Hearts, Rank.Four), 
            new(Suit.Hearts, Rank.Three), 
            new(Suit.Clubs, Rank.Queen),
            new(Suit.Clubs, Rank.Eight)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(joe, nameof(joe)),
            service.GetPokerHand(bob, nameof(bob)),
            service.GetPokerHand(sally, nameof(sally))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Single(winners);
        Assert.Equal(nameof(bob), winners.First().PlayerId);
    }
    
    [Fact(DisplayName = "Should pass example 3, Jen wins with pair and a kicker")]
    public void ShouldWinWithPairAndKicker()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var joe = new SortedSet<Card>
        {
            new(Suit.Diamonds, Rank.Five), 
            new(Suit.Clubs, Rank.Nine),
            new(Suit.Diamonds, Rank.Nine),
            new(Suit.Hearts, Rank.Queen),
            // low kicker
            new(Suit.Hearts, Rank.Three)
        };
        
        var jen = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Five),
            new(Suit.Hearts, Rank.Nine),
            new(Suit.Spades, Rank.Nine), 
            new(Suit.Spades, Rank.Queen),
            // high kicker
            new(Suit.Diamonds, Rank.Seven)
        };

        var bob = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Two), new(Suit.Clubs, Rank.Two), new(Suit.Spades, Rank.Five),
            new(Suit.Clubs, Rank.Ten), new(Suit.Hearts, Rank.Ace)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(joe, nameof(joe)),
            service.GetPokerHand(bob, nameof(bob)),
            service.GetPokerHand(jen, nameof(jen))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Single(winners);
        Assert.Contains(winners, w => w.PlayerId == nameof(jen));
    }
    
    [Fact(DisplayName = "Should have 2 winners with high card")]
    public void ShouldHaveTwoWinnersWithHighCard()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var highSameKicker1 = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Four),
            new(Suit.Diamonds, Rank.Five),
            new(Suit.Clubs, Rank.Nine),
            new(Suit.Clubs, Rank.Eight),
            new(Suit.Hearts, Rank.Queen)
        };
        var highSameKicker2 = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Four), 
            new(Suit.Clubs, Rank.Five),
            new(Suit.Hearts, Rank.Nine),
            new(Suit.Spades, Rank.Eight),
            new(Suit.Spades, Rank.Queen)
        };

        var highCardLowKicker = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Two), 
            new(Suit.Clubs, Rank.Three), 
            new(Suit.Spades, Rank.Five),
            new(Suit.Diamonds, Rank.Four), 
            new(Suit.Hearts, Rank.Six)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(highSameKicker1, nameof(highSameKicker1)),
            service.GetPokerHand(highCardLowKicker, nameof(highCardLowKicker)),
            service.GetPokerHand(highSameKicker2, nameof(highSameKicker2))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Equal(2, winners.Count);
        Assert.Contains(winners, w => w.PlayerId == nameof(highSameKicker1));
        Assert.Contains(winners, w => w.PlayerId == nameof(highSameKicker2));
    } 
    
    [Fact(DisplayName = "Should have a 2 winners with a pair")]
    public void ShouldHaveTwoWinnersWithAPair()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var pair1 = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Four),
            new(Suit.Hearts, Rank.Five),
            new(Suit.Hearts, Rank.Nine),
            new(Suit.Hearts, Rank.Eight),
            new(Suit.Diamonds, Rank.Four)
        };
        var pair2 = new SortedSet<Card>
        {
            new(Suit.Spades, Rank.Four), 
            new(Suit.Spades, Rank.Five),
            new(Suit.Spades, Rank.Nine),
            new(Suit.Spades, Rank.Eight),
            new(Suit.Clubs, Rank.Four)
        };

        var highCard = new SortedSet<Card>
        {
            new(Suit.Diamonds, Rank.Two), 
            new(Suit.Diamonds, Rank.Three), 
            new(Suit.Diamonds, Rank.Five),
            new(Suit.Diamonds, Rank.Queen), 
            new(Suit.Clubs, Rank.Six)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(pair1, nameof(pair1)),
            service.GetPokerHand(highCard, nameof(highCard)),
            service.GetPokerHand(pair2, nameof(pair2))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Equal(2, winners.Count);
        Assert.Contains(winners, w => w.PlayerId == nameof(pair2));
        Assert.Contains(winners, w => w.PlayerId == nameof(pair1));
    }
    
    [Fact(DisplayName = "Should have a 2 winners with a flash")]
    public void ShouldHaveTwoWinnersWithFlash()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var flash1 = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Four),
            new(Suit.Hearts, Rank.Five),
            new(Suit.Hearts, Rank.Six),
            new(Suit.Hearts, Rank.Seven),
            new(Suit.Hearts, Rank.Eight)
        };
        var flash2 = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Four), 
            new(Suit.Clubs, Rank.Five),
            new(Suit.Clubs, Rank.Six),
            new(Suit.Clubs, Rank.Seven),
            new(Suit.Clubs, Rank.Eight)
        };

        var flashLowCard = new SortedSet<Card>
        {
            new(Suit.Spades, Rank.Two), 
            new(Suit.Spades, Rank.Three), 
            new(Suit.Spades, Rank.Five),
            new(Suit.Spades, Rank.Four), 
            new(Suit.Spades, Rank.Six)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(flash1, nameof(flash1)),
            service.GetPokerHand(flashLowCard, nameof(flashLowCard)),
            service.GetPokerHand(flash2, nameof(flash2))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Equal(2, winners.Count);
        Assert.Contains(winners, w => w.PlayerId == nameof(flash2));
        Assert.Contains(winners, w => w.PlayerId == nameof(flash1));
    }
    
    [Fact(DisplayName = "Should have a 1 winner with a tripple, 2 with tripple is impossible")]
    public void ShouldHaveOneWinnersWithTripple()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var lowTripple = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Four),
            new(Suit.Diamonds, Rank.Four),
            new(Suit.Clubs, Rank.Four),
            new(Suit.Diamonds, Rank.Eight),
            new(Suit.Hearts, Rank.Queen)
        };
        var highTripple = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Five), 
            new(Suit.Diamonds, Rank.Five),
            new(Suit.Hearts, Rank.Five),
            new(Suit.Spades, Rank.Eight),
            new(Suit.Spades, Rank.Queen)
        };

        var randomSet = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Jack), 
            new(Suit.Diamonds, Rank.Queen), 
            new(Suit.Hearts, Rank.King),
            new(Suit.Hearts, Rank.Ace), 
            new(Suit.Diamonds, Rank.Ace)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(randomSet, nameof(randomSet)),
            service.GetPokerHand(highTripple, nameof(highTripple)),
            service.GetPokerHand(lowTripple, nameof(lowTripple)),
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Single(winners);
        Assert.Contains(winners, w => w.PlayerId == nameof(highTripple));
    }
    
    [Fact(DisplayName = "Should have a 1 winner with a double")]
    public void ShouldHaveOneWinnersWithADouble()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var lowDouble = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Four),
            new(Suit.Diamonds, Rank.Four),
            new(Suit.Clubs, Rank.Three),
            new(Suit.Diamonds, Rank.Eight),
            new(Suit.Hearts, Rank.Queen)
        };
        var highDouble = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Five), 
            new(Suit.Diamonds, Rank.Five),
            new(Suit.Hearts, Rank.Three),
            new(Suit.Spades, Rank.Eight),
            new(Suit.Spades, Rank.Queen)
        };

        var highCard = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Jack), 
            new(Suit.Diamonds, Rank.Queen), 
            new(Suit.Hearts, Rank.King),
            new(Suit.Hearts, Rank.Ten), 
            new(Suit.Diamonds, Rank.Ace)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(lowDouble, nameof(lowDouble)),
            service.GetPokerHand(highCard, nameof(highCard)),
            service.GetPokerHand(highDouble, nameof(highDouble))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Single(winners);
        Assert.Contains(winners, w => w.PlayerId == nameof(highDouble));
    }
    
    [Fact(DisplayName = "Should have a 1 winner with a flush with high card")]
    public void ShouldHaveOneWinnerWithFlashAndHighCard()
    {
        var service = new PokerHandEvaluatorService(new PokerHandComparer());

        var flashLow = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Two),
            new(Suit.Hearts, Rank.Three),
            new(Suit.Hearts, Rank.Four),
            new(Suit.Hearts, Rank.Seven),
            new(Suit.Hearts, Rank.Queen)
        };
        var flashHigh = new SortedSet<Card>
        {
            new(Suit.Clubs, Rank.Four), 
            new(Suit.Clubs, Rank.Five),
            new(Suit.Clubs, Rank.Six),
            new(Suit.Clubs, Rank.Seven),
            new(Suit.Clubs, Rank.Queen)
        };

        var highCard = new SortedSet<Card>
        {
            new(Suit.Diamonds, Rank.Two), 
            new(Suit.Diamonds, Rank.Three), 
            new(Suit.Diamonds, Rank.Five),
            new(Suit.Diamonds, Rank.Four), 
            new(Suit.Spades, Rank.Six)
        };
        
        var hands = new List<PokerHand>
        {
            service.GetPokerHand(flashLow, nameof(flashLow)),
            service.GetPokerHand(highCard, nameof(highCard)),
            service.GetPokerHand(flashHigh, nameof(flashHigh))
        };
        var winners = service.GetWinningHands(hands).ToList();
        
        Assert.Single(winners);
        Assert.Contains(winners, w => w.PlayerId == nameof(flashHigh));
    }    
}
