using UnityEngine;
using System.Collections;

public class PlayerBehaviour : Woundable
{
    public float walkSpeed = 0.15f;                 // Default walk speed.
    public float runSpeed = 1.0f;                   // Default run speed.
    public float sprintSpeed = 2.0f;                // Default sprint speed.
    public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
    public float jumpHeight = 1.0f;                 // Default jump height.
    private float speed;                            // Moving speed.
    private int attack1Bool;                       // Animator variable related to whether or not the player is on ground.
    protected int movingBool;
    private bool attack1 = false;
    public BatController m_Bat;
    public int m_Score = 0;
    private ParticleSystem m_PSys;
     public GameObject m_ProjectilePrefab;
    private bool m_Firing = false;
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
    private Transform playerCamera;
    public string m_PlayerPrefix;
    public GameObject[] m_KillObjects;
    private GameController m_GameController;
    public int m_Keys;
    public bool m_Finish = false;
    public GameObject m_PlayerUIPrefab;

    // Start is always called after any Awake functions.
    public override void Start()
    {
        base.Start();
        GameObject UI = Instantiate(m_PlayerUIPrefab);
        UI.GetComponent<PlayerUI>().m_Player = this;
        UI.transform.parent = GameObject.Find("PlayerUIPanel").transform;
        m_GameController = FindObjectOfType<GameController>();
        camScript = FindObjectOfType<ThirdPersonOrbitCam>();
        playerCamera = camScript.transform;
        m_Anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();
        m_PSys = GetComponent<ParticleSystem>();
        // Set up the references.
        movingBool = Animator.StringToHash("Moving");
        attack1Bool = Animator.StringToHash("Attack1");
        speedFloat  = Animator.StringToHash("Speed");
        m_Anim.SetBool(movingBool, false);
    }

    public IEnumerator FinishAnim()
    {
        m_Finish = true;
        m_Invincible = true;
        m_Anim.SetBool(movingBool, false);
        rbody.ResetCenterOfMass();
        rbody.ResetInertiaTensor();
        float timer = 0;
        while (timer < m_GameController.m_ExitTime)
        {
            this.transform.RotateAroundLocal(new Vector3(0, 1, 0), 0.25f);
            this.transform.localScale -= new Vector3(0.005f, 0.005f, 0.005f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
            //Put in transform rotation here
    }

    // Update is used to set features regardless the active behaviour.
    void Update()
    {
        if(m_Finish)
        {
            h = 0;
            v = 0;
        }
        if (!m_Dead)
        {
            h = Input.GetAxis(m_PlayerPrefix + "Horizontal");
            v = Input.GetAxis(m_PlayerPrefix + "Vertical");

            if (Input.GetButtonDown(m_PlayerPrefix + "Fire1"))
            {
                if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !(m_Anim.IsInTransition(0) && m_Anim.GetNextAnimatorStateInfo(0).IsName("Attack")))
                {
                    m_Anim.SetTrigger("Attack");
                    m_Bat.Attack();
                }
            }
            else if (Input.GetButtonDown(m_PlayerPrefix + "Fire2") && !m_Firing)
            {
                m_Bat.DeactivateCollider();
                if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile") && !(m_Anim.IsInTransition(0) && m_Anim.GetNextAnimatorStateInfo(0).IsName("Projectile")))
                {
                    m_Firing = true;
                    m_Anim.SetTrigger("Projectile");
                }
             
                StartCoroutine(FireProjectile());
            }

            if (h != 0 || v != 0)
            { IsMoving = true; }
            else { IsMoving = false; }

            m_Anim.SetBool(movingBool, IsMoving);

            if (Input.GetButtonDown(m_PlayerPrefix + "Teleport"))
            {
                Vector3 teleportPos = m_GameController.m_Players[0].transform.position + new Vector3(0, 0, 0);
                this.transform.position = teleportPos;
            }
          

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
            if(m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile") || (m_Anim.GetNextAnimatorStateInfo(0).IsName("Projectile") && m_Anim.IsInTransition(0)))
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

}
