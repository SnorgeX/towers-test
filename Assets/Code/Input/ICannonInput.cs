using System;
using UnityEngine;
using System.Collections.Generic;
public interface ICannonInput 
{
    public event Action<Quaternion> TargetRotationUpdated;
    public event Action<float> ShootPowerUpdated;
    public event Action ShootingStarted;
    public CannonView View { get; set; }
    public bool isShooting { get; set; }
    public void InputController();
}
