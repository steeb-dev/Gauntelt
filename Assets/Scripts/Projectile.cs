using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float m_MinDamage;
    public float m_MaxDamage;
    public float m_LifeTime;
    private float m_TimeAlive;

    private Rigidbody m_RigidBody;

    public void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_TimeAlive += Time.deltaTime;
        if(m_TimeAlive > m_LifeTime)
        { Destroy(this.gameObject); }
    }

    public void Init(Vector3 trajectory)
    {
        m_RigidBody.AddForce(trajectory  * speed * m_RigidBody.mass * 10, ForceMode.Impulse);
    }
       
    public float GetDamage()
    {
        return Random.Range(m_MinDamage, m_MaxDamage);
    }
}

