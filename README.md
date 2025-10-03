# Poker Hand Showdown

A library and console application to evaluate who are the winner(s) among several **5-card poker hands**.  
This implementation supports a **subset** of standard poker hands:

- **Flush**
- **Three of a Kind**
- **One Pair**
- **High Card**

Multiple winners are possible in case of exact ties.

---

## Input / Output

**Input**  
- A collection of players, each with:
  - Player Name
  - 5 Cards (rank + suit)

**Output**  
- A collection of winning players (one or more if tied)  
- All output is printed to the **Console window**

---

## Rules and Tie Breakers

### Flush
- 5 cards of the same suit
- Highest card wins
- If equal, compare the second-highest, and so on
- If all 5 are identical, result is a tie

### Three of a Kind
- 3 cards of the same rank
- Higher three wins
- If tied, use the 4th and 5th cards as kickers

### One Pair
- 2 cards of the same rank
- Higher pair wins
- If tied, compare remaining 3 cards as kickers (highest to lowest)

### High Card
- No pair, trips, or flush
- Highest card wins
- If tied, compare second, third, fourth, fifth
- If all 5 match, result is a tie

> **Note:** In poker, suits never break ties. Only ranks are used for comparisons.

For extended poker rules see:  
[Adda52 Poker Rules – Tie Breakers](https://www.adda52.com/poker/poker-rules/cash-game-rules/tie-breaker-rules)

---

## Technical Notes
- Language: **C# (.NET)** or **Spring Boot (Java)**  
- No GUI required — console output is sufficient
- Validations:
  - At least 2 players
  - All hands must be the same size (5 cards)
  - No duplicate cards across players

# Implementation

A simplified .NET 9 poker evaluator that builds a standard deck, deals five-card hands, ranks them, and reports the winner. The solution is split into a reusable hand-evaluation library, a console runner, and xUnit tests.

## Solution Structure
- `GvPokerEvaluator/` core models (`Card`, `PokerHand`) plus services for shuffling, ranking, and comparing hands.
- `GameRunner/` console host that wires up services, deals hands to named players, and logs round results.
- `TestGvPokerEvaluator/`  xUnit test suite covering deck integrity, hand ranking, and winner resolution rules.

## Getting Started
1. Install the .NET 9 SDK.
2. Restore and build the solution:
   ```
   dotnet restore
   dotnet build
   ```

### Run the Sample Game
```
dotnet run --project GameRunner
```

### Run the Tests
```
dotnet test
```

## Implementation Notes and Assumptions
- Supported rankings: `HighCard`, `OnePair`, `ThreeOfAKind`, and `Flush`.
- Duplicated cards and uneven hand sizes are rejected (thows exception) before ranking winners.
- The exmple unit test 1 and 2 are corrected: cards in the hands are not unique (example 2:  Bob and Sally both have 4S, these hands would be rejected)
- Logging uses `Microsoft.Extensions.Logging` and writes details to the console.
- we don't care if 4 of kind could lose to a higher 3 of a kind
- Poker hand comparer: we won't validate if hands are always same length, also won't validate if hand is invalid like 5 same cards of the same rank and suit (will validate in caller and in the deck but not in the comparer)




