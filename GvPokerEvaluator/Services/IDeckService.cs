using GvPokerEvaluator.Models;

namespace GvPokerEvaluator.Services;

public interface IDeckService
{
    public void Reset();

    public Card? Draw();
}
