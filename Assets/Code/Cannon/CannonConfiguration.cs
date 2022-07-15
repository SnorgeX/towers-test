
using UnityEngine;


[CreateAssetMenu(fileName = "NewCannonConfiguration", menuName = "GameConfigurations/CannonConfiguration")]
public class CannonConfiguration : ScriptableObject
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _maxShootPower;
    [SerializeField] private float _fullChargeTimeSec;
    [SerializeField] private float _angleSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _cooldownInSeconds;
    public GameObject Projectile => _projectile;
    public float MaxShootPower => _maxShootPower;
    public float FullChargeTime => 1/_fullChargeTimeSec;
    public float AngleSpeed => _angleSpeed;
    public float MaxAngle => _maxAngle;
    public float CooldownInSecond => _cooldownInSeconds;

}
