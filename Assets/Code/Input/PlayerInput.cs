using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : ICannonInput
{
    private int _touchId = 0;

    public event Action<Quaternion> TargetRotationUpdated;
    public event Action<float> ShootPowerUpdated;
    public event Action ShootingStarted;

    public bool isShooting { get; set; }
    public CannonView View { get; set; }

    public void SetView(CannonView view)
    {
        View = view;
    }
    public void InputController()
    {
        TapDetection();
    }
    public void TapDetection()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _touchId = touch.fingerId;
                if (isShooting)
                    return;
                isShooting = true;

                ShootPowerUpdated?.Invoke(100f);
                ShootingStarted?.Invoke();

            }
            if (touch.fingerId == _touchId && touch.phase == TouchPhase.Moved)
            {
                var touchPosition = getTouchPosition(touch.position);
                var targetRotation = Quaternion.LookRotation(Vector3.forward, View._cannonPivot.position - (Vector3)touchPosition);
                //_cannon.UpdateTargetRotation(targetRotation * Quaternion.Euler(0, 0, -90f));
                TargetRotationUpdated?.Invoke(targetRotation * Quaternion.Euler(0, 0, -90f));
            }
            if (touch.fingerId == _touchId && touch.phase == TouchPhase.Ended)
            {
                ShootPowerUpdated?.Invoke(0f);
            }
        }
    }
    Vector2 getTouchPosition(Vector2 touchPosition)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0));
    }
}
