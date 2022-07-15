using System.Collections;
using UnityEngine;


public class CorutineShieldController : MonoBehaviour
{
    [SerializeField] int MaxDelay;
    [SerializeField] int MinDelay;
    public ShieldView Shield;

    public void Init(ShieldView shield, int minDelay, int maxDelay) 
    {
        MaxDelay = maxDelay;
        MinDelay = minDelay;
        Shield = shield;
        Shield.ButtonAvalibleChanged += TryActivate;
        TryActivate(true);
    }

    private void TryActivate(bool avalible)
    {
        if (avalible)
            StartCoroutine(ShieldActivationCoroutine());
    }
    private IEnumerator ShieldActivationCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(MinDelay, MaxDelay));
        Shield.Activate();
    }
}
