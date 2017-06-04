using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : UnitController
{
    /// <summary>
    /// Прыжок.
    /// </summary>
    /// <param name="state"></param>
    public void Jump(string state)
    {
        if (MovingStates.Landing)
            MovingStates.JumpMode = bool.Parse(state);
        else
            MovingStates.JumpMode = false;
    }

    [PunRPC]
    public void MoveSpeed(string spd)
    {
        double speedTmp = double.Parse(spd);

      /*  if (this.gameObject.GetPhotonView())
            this.gameObject.GetPhotonView().RPC("MoveSpeed", PhotonTargets.Others, (speedTmp * 0.7).ToString());*/

        if (speedTmp > 0)
            MovingStates.SetMoveToRightState(true);
        else if (speedTmp < 0)
            MovingStates.SetMoveToLeftState(true);
        else
            MovingStates.StopMoving();
    }
}
