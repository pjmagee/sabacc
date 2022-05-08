namespace Sabacc.Domain;

public interface IWinnerCalculator
{
    Player Calculate(PlayersCircularList players);
}