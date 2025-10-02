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

# Assumptions 

- cards in the hands should be unique (in example 2 Bob and Sally both have 4S, these hands would be rejected)
- we are not going to validate correct hands (only if empty), such as 5 same cards, we are assuming deck is valid
- we don't care if 4 of kind could lose to a higher 3 of a kind
- Poker hand comparer: hands are always same length (will validate in caller but not in the comparer)

