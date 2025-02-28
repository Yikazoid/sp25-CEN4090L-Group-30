using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f; //amount of force added when the player jumps.
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f; //how much to smooth out the movement
    [SerializeField] private bool m_AirControl = false; //whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround; //a mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck; //a position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck; //a position marking where to check for ceilings

    const float k_GroundedRadius = .2f; //radius of the overlap circle to determine if grounded
    private bool m_Grounded; //whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    [System.Obsolete]
    public void Move(float move, bool jump, bool v, bool jump1)
    {

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        }
        //if player should jump
        if (m_Grounded && jump)
        {
            //adding verticle force to plater
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }
}
