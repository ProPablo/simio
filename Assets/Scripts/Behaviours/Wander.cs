using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : BehaviourComponent
{
    public override bool OnTick()
    {
        base.OnTick();
        ticks++;
        if (ticks > ticksPerAction)
        {
            OnAction();
            return true;
        }
        return false;
    }
    public override void OnAction()
    {
        base.OnAction();
        var (neighbourTile, tileDir) = actor.currentTile.SelectRandomNeighbor(actor.walkable);
        if (neighbourTile == null) return;
        actor.ChangeState(new MoveState(actor, tileDir, neighbourTile));
    }
}
