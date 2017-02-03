using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    public float walkSpeed = 0.15f;                 // Default walk speed.
    public float runSpeed = 1.0f;                   // Default run speed.
    public float sprintSpeed = 2.0f;                // Default sprint speed.
    public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
    public float jumpHeight = 1.0f;                 // Default jump height.
    private AudioSource m_Source;
    private float speed;                            // Moving speed.
    private int attack1Bool;                       // Animator variable related to whether or not the player is on ground.
    protected int movingBool;
    private bool attack1 = false;
    public BatController m_Bat;
    public int m_HP;
    public int m_Score = 0;
    public bool m_Dead;
    public SkinnedMeshRenderer m_MeshRenderer;
    private ParticleSystem m_PSys;
    public Color m_DefaultColor;
    public Color m_HitColor;
    public float m_HitFlashTime;
    public float m_HitCooldown = 0.2f;
    private float m_CooldownTimer = 0f;
    public GameObject m_ProjectilePrefab;
    private bool m_Firing = false;
    public GameObject m_RagDoll;
    private float h;                                // Horizontal Axis.
    private float v;                                // Vertical Axis.
    private Rigidbody rbody;
    public float turnSmoothing = 3.0f;
    private Animator m_Anim;
    private Vector3 lastDirection;
    private bool IsMoving;
    private bool IsSprinting;
    private int speedFloat;
    private ThirdPersonOrbitCam camScript;         // Reference to the third person camera script.
    public float sprintFOV = 100f;
    public Transform playerCamera;
    public string m_PlayerPrefix;
    public GameObject[] m_KillObjects;

    // Start is always called after any Awake functions.
    void Start()
    {
        m_Source = GetComponent<AudioSource>();
        camScript = playerCamera.GetComponent<ThirdPersonOrbitCam>();
        m_Anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();
        m_PSys = GetComponent<ParticleSystem>();
        m_MeshRenderer.materials[0].color = m_DefaultColor;
        // Set up the references.
        movingBool = Animator.StringToHash("Moving");
        attack1Bool = Animator.StringToHash("Attack1");
        speedFloat  = Animator.StringToHash("Speed");
        m_Anim.SetBool(movingBool, false);
    }

    // Update is used to set features regardless the active behaviour.
    void Update()
    {
        h = Input.GetAxis(m_PlayerPrefix + "Horizontal");
        v = Input.GetAxis(m_PlayerPrefix + "Vertical");

         
        m_CooldownTimer += Time.deltaTime;
        if (!m_Dead)
        {
            if (Input.GetButtonDown(m_PlayerPrefix + "Fire1"))
            {
                if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && m_Anim.IsInTransition(0)))
                {
                    m_Anim.SetTrigger("Attack");
                    m_Bat.Attack();
                }
  
            }
            else if (Input.GetButtonDown(m_PlayerPrefix + "Fire2") && !m_Firing)
            {
                m_Firing = true;
                m_Bat.DeactivateCollider();
                if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile"))
                {
                    m_Anim.SetTrigger("Projectile");
                }
                StartCoroutine(FireProjectile());
            }
            if (h != 0 || v != 0)
            { IsMoving = true; }
            else { IsMoving = false; }
            m_Anim.SetBool(movingBool, IsMoving);

            //if (Input.GetButtonDown("Sprint"))
            //{ IsSprinting = true; }
            //else { IsSprinting = false; }


            if (IsSprinting)
            {
                camScript.SetFOV(sprintFOV);
            }
            else
            {
                camScript.ResetFOV();
            }
        }

    }

    IEnumerator FireProjectile()
    {
        WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();
        bool loop = true;
        while(loop)
        {
            if(m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile"))
            {
                loop = false;
            }
            yield return waitForFrame;
        }
        Vector3 trajectory = transform.forward;
        GameObject pObj = Instantiate(m_ProjectilePrefab);
        pObj.transform.position = this.transform.position;
        Projectile p = pObj.GetComponent<Projectile>();
        p.m_Player = this.gameObject;
        p.Init(trajectory);
        m_Firing = false;
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    void FixedUpdate()
    {
        if (!m_Dead)
        {
            // Call the basic movement manager.
            MovementManagement(h, v, true);
        }
    }

    // Deal with the basic player movement
    void MovementManagement(float horizontal, float vertical, bool running)
	{
		// Call function that deals with player orientation.
		Rotating(horizontal, vertical);

		// Set proper speed.
		if(IsMoving)
		{
			if(IsSprinting)
			{
				speed = sprintSpeed;
			}
			else if (running)
			{
				speed = runSpeed;
			}
			else
			{
				speed = walkSpeed;
			}
		}
		else
		{
			speed = 0f;
		}
		m_Anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
	}

	// Rotate the player to match correct orientation, according to camera and key pressed.
	Vector3 Rotating(float horizontal, float vertical)
	{
        Vector3 forward = playerCamera.TransformDirection(Vector3.forward);

        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
		forward = forward.normalized;

		// Calculate target direction based on camera forward and direction key.
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		Vector3 targetDirection;
		float finalTurnSmoothing;
		targetDirection = forward * vertical + right * horizontal;
        finalTurnSmoothing = turnSmoothing;

        // Lerp current direction to calculated target direction.
        if ((IsMoving && targetDirection != Vector3.zero))
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection);

			Quaternion newRotation = Quaternion.Slerp(rbody.rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);
			rbody.MoveRotation (newRotation);
			SetLastDirection(targetDirection);
		}
		// If idle, Ignore current camera facing and consider last moving direction.
		if(!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
		{
			Repositioning();
		}

		return targetDirection;
	}

    // Set the last player direction of facing.
    public void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }

    // Put the player on a standing up position based on last direction faced.
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
            rbody.MoveRotation(newRotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            if (!m_Dead)
            {
                BatController bc = other.gameObject.GetComponent<BatController>();
                if(bc.m_Player != null && bc.m_Player.gameObject.tag != "Player")
                { 
                int damage = (int)bc.GetDamage();
                    if (damage > 0)
                    {
                        bc.PlaySound();

                        m_Source.Play();
                        this.m_HP -= damage;
                        StartCoroutine(Hit());
                        ////if (m_CooldownTimer > m_HitCooldown)
                        ////{
                        ////    m_HitCooldown = Random.Range(0.4f, 2f);
                        ////    m_CooldownTimer = 0f;
                        ////    HandleReactionAnimation(bc);
                        ////}

                        //m_PSys.Emit(100);

                        if (m_HP <= 0)
                        {
                            StopAllCoroutines();
                            StartCoroutine(KillAfterDeath());
                        }
                    }
                }
            }
        }
    }


    void HandleReactionAnimation(BatController bc)
    {
        var relativePoint = transform.InverseTransformPoint(bc.m_Player.transform.position);

        string[] hitArray = new string[] { "HitLeft", "HitRight", "HitFront", "HitBack" };
        float xWeight, yWeight;
        xWeight = Mathf.Abs(relativePoint.x);
        yWeight = Mathf.Abs(relativePoint.y);
        int animIndex = 0;
        if (xWeight > yWeight)
        {
            if (relativePoint.x < 0) { animIndex = 0; }
            else { animIndex = 1; }
        }
        else
        {
            if (relativePoint.z > 0) { animIndex = 2; }
            else { animIndex = 3; }
        }

        m_Anim.SetTrigger(hitArray[animIndex]);
    }

    IEnumerator KillAfterDeath()
    {
        m_Dead = true;
        m_Bat.DeactivateCollider();
        yield return new WaitForSeconds(0.2f);
        GameObject ragdoll = Instantiate(m_RagDoll);

        ragdoll.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = m_DefaultColor;
        ragdoll.transform.position = this.transform.position;
        ragdoll.transform.rotation = this.transform.rotation;
        ragdoll.transform.parent = this.transform;

        Destroy(m_Anim);
        foreach (GameObject go in m_KillObjects)
        {
            Destroy(go);
        }
    }


    IEnumerator Hit()
    {
        float t = 0;
        m_MeshRenderer.materials[0].color = m_HitColor;
        while (t < m_HitFlashTime)
        {
            m_MeshRenderer.materials[0].color = Color.Lerp(m_HitColor, m_DefaultColor, t / m_HitFlashTime);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

}
