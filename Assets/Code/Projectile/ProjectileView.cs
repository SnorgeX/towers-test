using UnityEngine;

public class ProjectileView : MonoBehaviour, IPooledObject
{
    

    public ObjectPooler.ObjectType Type => _type;
    [SerializeField] private ObjectPooler.ObjectType _type;
    [SerializeField] private int _damage = 1;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float despawnTime;
    private bool isDespawning;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var damagableObject = collision.collider.GetComponent(typeof(IDamagable)) as IDamagable;
        if (damagableObject != null)
            damagableObject.ApplyDamage(_damage);
        particles.Play();

        isDespawning = true;
    }
    private void FixedUpdate()
    {
        if (isDespawning)
        {
            transform.localScale -= Time.deltaTime * Vector3.one * 5;

            if (transform.localScale.x <= 0)
            {
                isDespawning = false;
                transform.localScale = Vector3.one;
                ObjectPooler.Instance.DestroyObject(this.gameObject);
            }
        }
    }
}
