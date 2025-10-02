using GvPokerEvaluator;
using GvPokerEvaluator.Models;

namespace TestGvPokerEvaluator;

public class TestPokerHand
{
    [Fact]
    public void CompareTo_OtherIsNull_ReturnsGreaterThanZero()
    {
        var hand = new PokerHand(
            new SortedSet<Card> { new (Suit.Hearts, Rank.Four) },
            HandRank.HighCard,
            "Player1");

        var result = hand.CompareTo(null);

        Assert.True(result > 0);
    }

    [Fact]
    public void CompareTo_DifferentHandRanks_HigherRankWins()
    {
        var lowHand = new PokerHand(
            new SortedSet<Card> { new (Suit.Hearts, Rank.Ten) },
            HandRank.OnePair,
            "Low");

        var highHand = new PokerHand(
            new SortedSet<Card> { new (Suit.Clubs, Rank.Two) },
            HandRank.Flush,
            "High");

        Assert.True(highHand.CompareTo(lowHand) > 0);
        Assert.True(lowHand.CompareTo(highHand) < 0);
    }

    [Fact]
    public void CompareTo_SameRank_HighCardWins()
    {
        var hand1 = new PokerHand(
            new SortedSet<Card>
            {
                new (Suit.Hearts, Rank.Ace),
                new(Suit.Clubs, Rank.Ten),
                new(Suit.Diamonds, Rank.Seven),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.Two)
            },
            HandRank.Flush,
            "Player1");

        var hand2 = new PokerHand(
            new SortedSet<Card>
            {
                new(Suit.Hearts, Rank.King), 
                new(Suit.Clubs, Rank.Ten),
                new(Suit.Diamonds, Rank.Seven),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.Two)
            },
            HandRank.Flush,
            "Player2");

        Assert.True(hand1.CompareTo(hand2) > 0);
        Assert.True(hand2.CompareTo(hand1) < 0);
    }

    [Fact]
    public void CompareTo_SameRankAndSameCards_ReturnsZero()
    {
        var cards = new SortedSet<Card>
        {
            new(Suit.Hearts, Rank.Ten),
            new(Suit.Clubs, Rank.Eight),
            new(Suit.Diamonds, Rank.Six),
            new(Suit.Spades, Rank.Four),
            new(Suit.Hearts, Rank.Two)
        };

        var hand1 = new PokerHand(cards, HandRank.HighCard, "P1");
        var hand2 = new PokerHand(cards, HandRank.HighCard, "P2");

        var result = hand1.CompareTo(hand2);

        Assert.Equal(0, result);
    }
    
    [Theory(DisplayName = "Validate that hand rank works")]
    [InlineData(HandRank.HighCard, HandRank.Flush, -1)]   
    [InlineData(HandRank.ThreeOfAKind, HandRank.OnePair, 1)] // Trips beats Pair
    [InlineData(HandRank.Flush, HandRank.Flush, 0)]      // Same ranks → depends on cards
    public void Test(HandRank rank1, HandRank rank2, int expected)
    {
        var hand1 = new PokerHand(Create((Rank.Two, Suit.Hearts)), rank1, "P1");
        var hand2 = new PokerHand(Create((Rank.Three, Suit.Clubs)), rank2, "P2");

        int result = hand1.CompareTo(hand2);

        if (expected == 0)
            Assert.Equal(0, result);
        else
            Assert.Equal(expected > 0, result > 0);
    }
    
    [Theory(DisplayName = "Validate that hand rank works as expected")]
    [InlineData(HandRank.Flush, HandRank.Flush, 0)]   
    [InlineData(HandRank.HighCard, HandRank.Flush, -1)]   
    [InlineData(HandRank.ThreeOfAKind, HandRank.Flush, -1)]   
    [InlineData(HandRank.OnePair, HandRank.Flush, -1)]   
    [InlineData(HandRank.ThreeOfAKind, HandRank.ThreeOfAKind, 0)]   
    [InlineData(HandRank.HighCard, HandRank.ThreeOfAKind, -1)]   
    [InlineData(HandRank.OnePair, HandRank.OnePair, 0)] 
    [InlineData(HandRank.HighCard, HandRank.OnePair, -1)]      // Same ranks → depends on cards
    public void HandRankIsValid(HandRank rank1, HandRank rank2, int expected)
    {
        var hand1 = new PokerHand(Create((Rank.Two, Suit.Hearts)), rank1, "P1");
        var hand2 = new PokerHand(Create((Rank.Three, Suit.Clubs)), rank2, "P2");

        int result = hand1.CompareTo(hand2);

        if (expected == 0)
            Assert.Equal(0, result);
        else
            Assert.Equal(expected > 0, result > 0);
    }

        public static IEnumerable<object[]> HighCardData()
        {
            yield return new object[]
            {
                Create(
                    (Rank.Ace, Suit.Hearts),
                    (Rank.Ten, Suit.Spades),
                    (Rank.Five, Suit.Clubs),
                    (Rank.Four, Suit.Diamonds),
                    (Rank.Two, Suit.Hearts)
                ),
                Create(
                    (Rank.King, Suit.Hearts),
                    (Rank.Ten, Suit.Clubs),
                    (Rank.Five, Suit.Diamonds),
                    (Rank.Four, Suit.Spades),
                    (Rank.Two, Suit.Clubs)
                ),
                1 // Ace beats King
            };

            yield return new object[]
            {
                Create(
                    (Rank.Ten, Suit.Hearts),
                    (Rank.Nine, Suit.Spades),
                    (Rank.Eight, Suit.Clubs),
                    (Rank.Seven, Suit.Diamonds),
                    (Rank.Six, Suit.Hearts)
                ),
                Create(
                    (Rank.Ten, Suit.Clubs),
                    (Rank.Nine, Suit.Hearts),
                    (Rank.Eight, Suit.Spades),
                    (Rank.Seven, Suit.Diamonds),
                    (Rank.Six, Suit.Clubs)
                ),
                0 // Exact tie
            };
        }

        [Theory]
        [MemberData(nameof(HighCardData))]
        public void CompareTo_ByHighCard(SortedSet<Card> cards1, SortedSet<Card> cards2, int expected)
        {
            var hand1 = new PokerHand(cards1, HandRank.Flush, "P1");
            var hand2 = new PokerHand(cards2, HandRank.Flush, "P2");

            int result = hand1.CompareTo(hand2);

            if (expected == 0)
                Assert.Equal(0, result);
            else
                Assert.Equal(expected > 0, result > 0);
        }
        
        private static SortedSet<Card> Create(params (Rank rank, Suit suit)[] cards) =>
            new(cards.Select(c => new Card(c.suit, c.rank)));
    
}
