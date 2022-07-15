using System.Collections;
using UnityEngine;


public class CannonView : MonoBehaviour
{
    [SerializeField] private CannonConfiguration _startCannon;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] public Transform CannonPivot;
    [SerializeField] private ICannonInput _input;

    private CannonModel _cannonModel;

    private Quaternion _targetRotation;
    private float _targetPower;

    private float _shootPowerPercent;
    private float _cooldownElapsedTime;

    private bool _isShootPowerUp;
    private bool _canShoot;

    void Start()
    {
        _cannonModel = new CannonModel(_startCannon);
        _input.TargetRotationUpdated += UpdateTargetRotation;
        _input.ShootingStarted += StartShootPowerUp;
        _input.ShootPowerUpdated += SetTargetPower;
    }
    public void SetInput(ICannonInput input)
    {
        _input = input;
    }

    void FixedUpdate()
    {
        if (!_canShoot)
        {
            CoolDown();
            return;
        }

        _input.InputController();

        if (CannonPivot.rotation != _targetRotation)
        {
            CannonPivot.rotation = Quaternion.RotateTowards(CannonPivot.rotation, _targetRotation, _cannonModel.angleSpeed);
        }

        if (_input.isShooting)
        {
            PowerActive();
        }
    }
    public void Init(CannonModel cannonModel, ICannonInput input)
    {
        _cannonModel = cannonModel;
        _input = input;
    }

    public void SetTargetRotation(Quaternion targetRotation)
    {
        _targetRotation = targetRotation;
    }

    public void SetTargetPower(float targerPower)
    {
        if (_cannonModel.maxShootPower > targerPower)
            _targetPower = targerPower / _cannonModel.maxShootPower;
        else
        _targetPower = 1;
    }

    private void PowerActive()
    {
        _shootPowerPercent += Time.deltaTime * _cannonModel.fullChargeTime;
        CannonPivot.transform.localScale = new Vector3(1-_shootPowerPercent / 4,1 + _shootPowerPercent / 4, 1f);
        if (_shootPowerPercent >= _targetPower)
        {
            Shoot();
            EndShootPowerUp();
        }
    }

    private void StartCoolDown()
    {
        _canShoot = false;
        _cooldownElapsedTime = 0f;
    }

    private void CoolDown() 
    {
        CannonPivot.transform.localScale = Vector3.Lerp(CannonPivot.transform.localScale, Vector3.one, _cooldownElapsedTime / _cannonModel.cooldownInSeconds);
        _cooldownElapsedTime += Time.deltaTime;
        if (_cooldownElapsedTime >= _cannonModel.cooldownInSeconds)
        {
            _canShoot = true;
            _shootPowerPercent = 0f;
        }
    }

    public void StartShootPowerUp()
    {
        if (_isShootPowerUp || !_canShoot)
            return;

        _isShootPowerUp = true;
        _shootPowerPercent = 0f;
    }

    public void EndShootPowerUp()
    {
        _input.isShooting = false;
        _isShootPowerUp = false;
        _targetPower = _shootPowerPercent;
    }

    public void Shoot()
    {
        var projectile = ObjectPooler.Instance.GetObject(ObjectPooler.ObjectType.Projectile,_shootPosition.position,_shootPosition.rotation);
        projectile.GetComponent<Rigidbody2D>().velocity = CannonPivot.right * _cannonModel.maxShootPower * _shootPowerPercent;
        StartCoolDown();
    }
    public void UpdateRotation(Vector3 targetPos)
    {
        var targetRotation = Quaternion.LookRotation(Vector3.forward, CannonPivot.position - targetPos) * Quaternion.Euler(0, 0, -90f);
        if (targetRotation.eulerAngles.z > _cannonModel.maxAngle)
            return;
        UpdateTargetRotation(targetRotation);
    }
    public void UpdateTargetRotation(Quaternion targetRotation)
    {
        _targetRotation = targetRotation;
    }

    private void OnDestroy()
    {
        _input.TargetRotationUpdated -= UpdateTargetRotation;
        _input.ShootingStarted -= StartShootPowerUp;
        _input.ShootPowerUpdated -= SetTargetPower;
    }
}
