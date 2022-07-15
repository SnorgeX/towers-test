using UnityEditor;
using UnityEngine;

public class CannonModel
{

    public readonly GameObject projectile;
    public readonly float maxShootPower;
    public readonly float fullChargeTime;
    public readonly float angleSpeed;
    public readonly float maxAngle;
    public readonly float cooldownInSeconds;
    public CannonModel (CannonConfiguration cannonConfiguration)
    {
        projectile = cannonConfiguration.Projectile;
        maxShootPower = cannonConfiguration.MaxShootPower;
        fullChargeTime = cannonConfiguration.FullChargeTime;
        angleSpeed = cannonConfiguration.AngleSpeed;
        cooldownInSeconds = cannonConfiguration.CooldownInSecond;
        maxAngle = cannonConfiguration.MaxAngle;
    }
}
