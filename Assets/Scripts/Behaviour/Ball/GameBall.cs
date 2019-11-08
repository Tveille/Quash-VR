using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBall : MonoBehaviour, IBall
{
    [SerializeField] float m_hitforceMin = 6;
    [SerializeField] float m_hitforceMax = 12;
    [SerializeField] float m_bounceForce = 6;
    [SerializeField] Rigidbody m_rigidbody;

    [SerializeField] Vector3 m_direction;

    void Start()
    {
        if (!m_rigidbody) m_rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        m_direction = m_rigidbody.velocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public void HitBall()
    {
        //m_rigidbody.velocity = m_hitforceMax
    }

    public void Bounce(ContactPoint p_contactPoint)
    {

    }
}
