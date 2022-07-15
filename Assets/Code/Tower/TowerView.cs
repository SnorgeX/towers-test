using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
public class TowerView : MonoBehaviour, IDamagable
{
    private int _maxHealth;
    private int _health;
    [SerializeField] private List<Transform> _cannonPositions;
    [SerializeField] private List<CannonView> _cannons;
    [SerializeField] private CannonView cannonPrefub;
    [SerializeField] private CannonConfiguration _startCannon;

    [SerializeField] private ShieldView _shield;

    private VisualElement _bar;
    private Button _shieldButton;
    public void Init(int health, Vector3 position, VisualElement bar)
    {
        _bar = bar.Q("BarColor");
        _maxHealth = health;
        _health = health;
        transform.position = position;
    }

    public void ApplyDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Destroy(this);
            return;
        }
        _bar.style.width = Length.Percent((float)_health / _maxHealth * 100f);

    }

    public void CreateCannon(CannonModel cannonModel, ICannonInput input)
    {
        var newcannon = Instantiate(cannonPrefub, _cannonPositions[_cannons.Count]);
        newcannon.transform.rotation = Quaternion.Euler(0,0,90);
        newcannon.Init(cannonModel, input);

        _cannons.Add(newcannon);

        input.View = newcannon;
    }

    public void CreateShield(Vector3 position, Button shieldButton)
    {
        _shield = Instantiate(_shield, position, Quaternion.identity);
        _shield.Activate();

        _shieldButton = shieldButton;
        _shieldButton.parent.Q<Label>().style.visibility = Visibility.Hidden;
        _shieldButton.clicked += _shield.Activate;
        _shield.ButtonAvalibleChanged += ChangeShieldButtonView;

    }

    private void OnDestroy()
    {
        foreach (var cannon in _cannons)
        {
            Destroy(cannon);
        }
        _cannons.Clear();
        _bar.parent.parent.RemoveFromHierarchy();
        _shieldButton.parent.parent.RemoveFromHierarchy();
        Destroy(_shield.gameObject);
    }
    private void ChangeShieldButtonView(bool avalible)
    {
        _shieldButton.SetEnabled(avalible);
        if (avalible)
            _shieldButton.parent.Q<Label>().style.visibility = Visibility.Hidden;
        else
            _shieldButton.parent.Q<Label>().style.visibility = Visibility.Visible;
    }
    private void FixedUpdate()
    {
        if (_shieldButton.parent.Q<Label>().style.visibility == Visibility.Visible)
        {
            _shieldButton.parent.Q<Label>().text = ((int)(_shield._recoveryTime - _shield.CurrentRecovery)).ToString();
        }

    }
}
