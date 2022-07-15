using System;
using UnityEngine;

public class ShieldView : MonoBehaviour, IDamagable
{

    public event Action<bool> ButtonAvalibleChanged;

    [SerializeField] private int _health;
    [SerializeField] private float _activationTime;
    [SerializeField] private float _lifeTime;

    [SerializeField] float _targetScale;
    [SerializeField] float _currentState;

    private bool _isScaling;
    private bool _isActive;
    private bool _isEnabled;

    private int _currentHealth;
    private float _currentLifetime;

    public float _recoveryTime;
    public float CurrentRecovery;

    private void Start()
    {
        _currentHealth = _health;
        _isActive = true;
    }

    public void ApplyDamage(int damage)
    {
        _currentHealth--;
        if (_currentHealth <= 0)
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        _currentHealth = _health;
        _currentState = 0f;
        _currentLifetime = _lifeTime;
        CurrentRecovery = 0f;
        _targetScale = 2.5f;

        _isScaling = true;
        _isActive = true;
        _isEnabled = false;
        ButtonAvalibleChanged?.Invoke(_isEnabled);
    }

    public void Deactivate()
    {
        _isScaling = true;
        _targetScale = 0;
        _currentState = 0;
        _isActive = false;
    }
    private void ChangeSize()
    {
        _currentState += Time.deltaTime / _activationTime;
        if (_currentState >= 1)
        {
            _currentState = 1;
            _isScaling = false;
        }
        var scale = new Vector3(2.5f, Mathf.Lerp(transform.localScale.y, _targetScale, _currentState), 2.5f);
        transform.localScale = scale;
    }

    private void Recovery()
    {
        CurrentRecovery += Time.deltaTime;
        if (CurrentRecovery >= _recoveryTime)
        {
            _isEnabled = true;
            ButtonAvalibleChanged?.Invoke(_isEnabled);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!_isEnabled)
        {
            Recovery();
        }
        if (_isScaling)
        {
            ChangeSize();
            return;
        }
        if (!_isActive)
        {
            return;
        }
        if (_currentLifetime <= _lifeTime)
        {
            _currentLifetime += Time.deltaTime;
            return;
        }
        _currentLifetime = 0;
        Deactivate();
    }
}
