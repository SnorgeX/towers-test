using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : ICannonInput
{
    public bool isShooting { get ; set ; }
    public CannonView View { get ; set ; }

    public event Action<Quaternion> TargetRotationUpdated;
    public event Action<float> ShootPowerUpdated;
    public event Action ShootingStarted;

    private float _cannonAngle;
    private Vector3 _targetCoordinates;
    private float g = Physics2D.gravity.y;
    private float _aiShootCooldown;
    private bool _cooldownEnded;
    private float _missChance;
    public AIInput(Vector3 target,float missChance)
    {
        _missChance = missChance;
        _targetCoordinates = target;
        isShooting = false;
    }

    public void InputController()
    {
        if (_cooldownEnded)
        {
            
            isShooting = true;
            SetRandomAngle();
            TargetRotationUpdated?.Invoke(Quaternion.Euler(0,0,_cannonAngle));

            float velocity;
            if(UnityEngine.Random.Range(0, 100)>_missChance)
            {
                velocity = SetShootVelocity(_cannonAngle, _targetCoordinates);
            }
            else
            {
                velocity = UnityEngine.Random.Range(10, 100);
            }

            ShootPowerUpdated?.Invoke(velocity);
            ShootingStarted?.Invoke();
            StartAIShootCoolDown();
        }
        else
        {
            AIShootCoolDown();
        }
    }

    private void StartAIShootCoolDown()
    {
        _cooldownEnded = false;
        _aiShootCooldown = UnityEngine.Random.Range(1, 5);
    }
    private void AIShootCoolDown()
    {
        _aiShootCooldown -= Time.deltaTime;
        if (_aiShootCooldown <= 0)
            _cooldownEnded = true;
    }

    private void SetRandomAngle()
    {
        if(View.transform.position.x>_targetCoordinates.x)
            _cannonAngle = UnityEngine.Random.Range(180, 105);
        else
            _cannonAngle = UnityEngine.Random.Range(0, 75);
    }

    private float SetShootVelocity(float degrees, Vector3 targetPosition)
    {
        var fromTo = targetPosition - View.CannonPivot.position;
        var x = fromTo.x;
        var y = fromTo.y;
        var angleRad = degrees * Mathf.PI / 180;
        var targetVelocity = Mathf.Sqrt((g * x * x) / (2 * (y - Mathf.Tan(angleRad) * x) * Mathf.Pow(Mathf.Cos(angleRad), 2)));
        //Debug.Log(targetVelocity + " pos: " + targetPosition);
        return targetVelocity;
    }
}
