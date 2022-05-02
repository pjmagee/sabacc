namespace Sabacc.Domain;

public class PlayersCircularList : LinkedList<Player>
{
    public LinkedListNode<Player> CurrentDealer { get; private set; } = null!;
    public LinkedListNode<Player> CurrentTurn { get; private set; } = null!;

    public void SetFirstRoundDealer()
    {
        if (Count <= 1)
            throw new InvalidOperationException("Minimum of 2 players required.");

        CurrentDealer = First!;
    }

    public void SetFirstRoundTurn()
    {
        if (Count <= 1)
            throw new InvalidOperationException("Minimum of 2 players required.");

        CurrentTurn = CurrentDealer!.Next ?? First!;
    }

    public void ShiftDealerToNext()
    {
        if (Count <= 1)
            throw new InvalidOperationException("Minimum of 2 players required.");

        CurrentDealer = CurrentDealer!.Next ?? First!;
    }

    public void ShiftTurnToNext()
    {
        if (Count <= 1)
            throw new InvalidOperationException("Minimum of 2 players required.");

        CurrentTurn = CurrentTurn.Next ?? First!;
    }
}