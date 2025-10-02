using GameRunner;
using GvPokerEvaluator.Services;
using Microsoft.Extensions.Logging;


// initializing services here, in the real project this will be done by IoC container
var deckService = new DeckServiceService();
var pokerHandComparer = new PokerHandComparer();
var handEvaluatorService = new PokerHandEvaluatorService(pokerHandComparer);
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information); 
});

var cardGame = new CardGame(deckService, handEvaluatorService, loggerFactory.CreateLogger("ConsoleApp"));

var winner = cardGame.CreateAndRunGame(
    new List<string>
    {
        { "Joe" },
        { "Bob" },
        { "Sally" }
    });
Console.WriteLine($"{winner} Wins!");