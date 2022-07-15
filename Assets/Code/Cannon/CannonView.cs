using System.Collections;
using UnityEngine;


public class CannonView : MonoBehaviour
{
    [SerializeField] private CannonConfiguration _startCannon;
    private CannonModel _cannonModel;
    private Quaternion _targetRotation;
    private float _targetPower;
    private bool _isShootPowerUp;
    private bool _canShoot;
    private float _shootPowerPercent;
    private float _cooldownElapsedTime;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] public Transform _cannonPivot;
    [SerializeField] private ICannonInput _input;
    [SerializeField] private Transform _target;
    void Start()
    {
        //_input = new AIInput(this, _target.position);
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

        if (_cannonPivot.rotation != _targetRotation)
        {
            _cannonPivot.rotation = Quaternion.RotateTowards(_cannonPivot.rotation, _targetRotation, _cannonModel.angleSpeed);
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
        _cannonPivot.transform.localScale = new Vector3(1-_shootPowerPercent / 4,1 + _shootPowerPercent / 4, 1f);
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
        _cannonPivot.transform.localScale = Vector3.Lerp(_cannonPivot.transform.localScale, Vector3.one, _cooldownElapsedTime / _cannonModel.cooldownInSeconds);
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
        //var projectile = Instantiate(_cannonModel.projectile, _shootPosition.position, _shootPosition.rotation);
        projectile.GetComponent<Rigidbody2D>().velocity = _cannonPivot.right * _cannonModel.maxShootPower * _shootPowerPercent;
        StartCoolDown();
    }
    public void UpdateRotation(Vector3 targetPos)
    {
        var targetRotation = Quaternion.LookRotation(Vector3.forward, _cannonPivot.position - targetPos) * Quaternion.Euler(0, 0, -90f);
        if (targetRotation.eulerAngles.z > _cannonModel.maxAngle)
            return;
        UpdateTargetRotation(targetRotation);

    }
    public void UpdateTargetRotation(Quaternion targetRotation)
    {
        _targetRotation = targetRotation;
    }

}
