namespace Sabacc.Domain;

public class PlayersCircularList : LinkedList<Player>
{
    public LinkedListNode<Player> CurrentDealer { get; private set; } = null!;
    public LinkedListNode<Player> CurrentTurn { get; private set; } = null!;

    public void Join(Guid id, string name)
    {
        if (Count == 0)
        {
            AddFirst(new Player(id, name));
        }
        else
        {
            AddLast(new Player(id, name));
        }
    }

    public IEnumerable<Player> WithoutJunkedOut() => this.Where(p => p.State.JunkedOut == false);

    public bool InPhase1() => this.Any(p => p.State.PhaseOne.Completed == false);
    public bool InPhase2() => this.Any(p => !InPhase1() && p.State.PhaseTwo.Completed == false);
    public bool InPhase3() => this.Any(p => !InPhase1() && !InPhase2() && p.State.PhaseThree.Completed == false);

    public void Leave(Guid playerId)
    {
        foreach (var player in this)
        {
            if (player.Id == playerId)
            {
                Remove(player);
            }
        }
    }

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

        if (CurrentTurn.Value.State.JunkedOut)
            ShiftTurnToNext();
    }

    public void ResetPhaseCompletions(bool forNextRound = false)
    {
        foreach (var player in this.Where(p => !p.State.JunkedOut))
        {
            if (forNextRound)
            {
                // Players only Junk out per Round
                player.State.JunkedOut = false;
            }

            player.State.PhaseOne.Reset();
            player.State.PhaseTwo.Reset();
            player.State.PhaseThree.Reset();
        }
    }
}