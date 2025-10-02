using GvPokerEvaluator.Models;

namespace GvPokerEvaluator.Services;

public class PokerHandComparer : IComparer<PokerHand>
{
    /// <summary>Compares this instance to a specified object and returns an indication of their relative values.
    /// Assumptions: hands are always same length
    /// </summary>
    /// <param name="x">An object to compare</param>
    /// <param name="y">An object to compare</param>
    /// <returns>return x if x is greater then y, -1 if y is greater then x and 0 if equal</returns>
    public int Compare(PokerHand? x, PokerHand? y)
    {
        if (x is null)
            return -1;
        if (y is null)
            return 1;

        if (x.HandRank != y.HandRank)
            return x.HandRank.CompareTo(y.HandRank);

        // for flush or high card we can just look for highest card
        if (x.HandRank is HandRank.Flush or HandRank.HighCard)
        {
            return x.Cards.Reverse()
                // Compare hands for high card
                .Zip(y.Cards.Reverse(), (a, b) => a.Rank.CompareTo(b.Rank))
                // if no high card found the hands will split the pot
                .FirstOrDefault(highCard => highCard != 0, 0);
        }

        // figure out what pair or three the group has and compare
        var xGroupRank = x.Cards
            .GroupBy(card => card.Rank)
            .OrderByDescending(group => group.Count())
            .First()
            .Key;
        var yGroupRank = y.Cards
            .GroupBy(card => card.Rank)
            .OrderByDescending(group => group.Count())
            .First()
            .Key;
        
        if (xGroupRank > yGroupRank) 
            return 1;
        if (yGroupRank > xGroupRank)
            return -1;

        // Groups are the same so look for high kicker
        return x.Cards.Reverse()
            // Compare hands for high card
            .Zip(y.Cards.Reverse(), (a, b) => a.Rank.CompareTo(b.Rank))
            // if no high card found the hands will split the pot
            .FirstOrDefault(highCard => highCard != 0, 0);
    }
}