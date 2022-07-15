using UnityEditor;
using UnityEngine;

public class CannonController
{
    private CannonModel _cannonModel;
    private CannonView _cannonView;
    private bool _isShootPowerUp;
    private float _ShootPowerPercent;
    private void PowerActive()
    {
        _ShootPowerPercent += Time.deltaTime;
        if (_ShootPowerPercent > 1f)
        {
            EndShootPowerUp();
            Shoot();
            StartShootPowerUp();
        }
    }
    public void StartShootPowerUp()
    {
        if (_isShootPowerUp)
            return;

        _isShootPowerUp = true;
        _ShootPowerPercent = 0f;
    }

    public void EndShootPowerUp()
    {
        _isShootPowerUp = false;
    }

    public void UpdateRotation(Vector3 targetPos)
    {
        var targetRotation = Quaternion.LookRotation(Vector3.forward, _cannonView.transform.position - targetPos);
        if (targetRotation.eulerAngles.z > _cannonModel.maxAngle)
            targetRotation = Quaternion.Euler(0, 0, _cannonModel.maxAngle);
        _cannonView.UpdateTargetRotation(targetRotation);

    }
    public void Shoot()
    {
        _cannonView.Shoot();
    }
    public CannonController(CannonConfiguration model, CannonView view)
    {
        _cannonView = view;
        _cannonModel = new CannonModel(model);
    }
}
