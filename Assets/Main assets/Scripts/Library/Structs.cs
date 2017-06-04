using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structs
{
    public class MovingControl
    {
        public bool FreezeMode = false;
        public bool JumpMode = false;
        public bool Landing = true;
        public double Speed = 0;
        public double MaxSpeed = 10;
        public double DeltaSpeed = 0.35;
        private bool MoveToLeftMode = false;
        private bool MoveToRightMode = false;

        public MovingControl(double spd, double deltaSpd)
        {
            MaxSpeed = spd;
            DeltaSpeed = deltaSpd;
            FreezeMode = false;
            JumpMode = false;
            Landing = true;
            MoveToLeftMode = false;
            MoveToRightMode = false;
        }

        public void SetMoveToLeftState(bool flag)
        {
            MoveToLeftMode = flag;
            MoveToRightMode = false;
        }

        public void SetMoveToRightState(bool flag)
        {
            MoveToRightMode = flag;
            MoveToLeftMode = false;
        }

        public void StopMoving()
        {
            MoveToRightMode = false;
            MoveToLeftMode = false;
        }

        public bool GetMoveToLeftState()
        {
            return MoveToLeftMode;
        }

        public bool GetMoveToRightState()
        {
            return MoveToRightMode;
        }
    }
}
