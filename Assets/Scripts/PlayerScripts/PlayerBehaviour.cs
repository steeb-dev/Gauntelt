using UnityEngine;
using System.Collections;

// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
public class PlayerBehaviour : GenericBehaviour
{
	public float walkSpeed = 0.15f;                 // Default walk speed.
	public float runSpeed = 1.0f;                   // Default run speed.
	public float sprintSpeed = 2.0f;                // Default sprint speed.
	public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
	public float jumpHeight = 1.0f;                 // Default jump height.

	private float speed;                            // Moving speed.
	private int jumpBool;                           // Animator variable related to jumping.
	private int groundedBool;                       // Animator variable related to whether or not the player is on ground.
    private int attack1Bool;                       // Animator variable related to whether or not the player is on ground.
 	private bool jump;                              // Boolean to determine whether or not the player started a jump.
    private bool attack1 = false;
    public BatController m_Bat;
    public int m_HP;
    private bool m_Dead;
    public SkinnedMeshRenderer m_MeshRenderer;
    private ParticleSystem m_PSys;
    public Color m_DefaultColor;
    public Color m_HitColor;
    public float m_HitFlashTime;

    public GameObject m_RagDoll;
    public UnityEngine.UI.Text m_HealthText;

    public Camera m_DeathCam;
    public Camera m_MainCam;
    // Start is always called after any Awake functions.
    void Start()
    {
        m_DeathCam.enabled = false;
        m_MainCam.enabled = true;
        m_PSys = GetComponent<ParticleSystem>();
        m_MeshRenderer.materials[0].color = m_DefaultColor;
        // Set up the references.
        jumpBool = Animator.StringToHash("Jump");
		groundedBool = Animator.StringToHash("Grounded");
        attack1Bool = Animator.StringToHash("Attack1");
        anim.SetBool (groundedBool, true);

		// Subscribe and register this behaviour as the default behaviour.
		behaviourManager.SubscribeBehaviour (this);
		behaviourManager.RegisterDefaultBehaviour (this.behaviourCode);
	}

	// Update is used to set features regardless the active behaviour.
	void Update ()
	{
        m_HealthText.text = "HP: " + m_HP.ToString();
        if (!m_Dead)
        {
            if (Input.GetButtonDown("Jump"))
                jump = true;
            if (Input.GetButtonDown("Fire1"))
            {
                anim.SetTrigger("Attack");
                m_Bat.Attack();
            }
        }
	}
    

	// LocalFixedUpdate overrides the virtual function of the base class.
	public override void LocalFixedUpdate()
    {
        if (!m_Dead)
        {
            // Call the basic movement manager.
            MovementManagement(behaviourManager.GetH, behaviourManager.GetV, true);

            // Call the jump manager.
            JumpManagement();
        }
    }

	// Execute the idle and walk/run jump movements.
	void JumpManagement()
	{
		// Already jumped, landing.
		if (anim.GetBool(jumpBool) && rbody.velocity.y < 0)
		{
			// Set jump boolean on the Animator controller.
			jump = false;
			anim.SetBool (jumpBool, false);
		}
		// Start jump.
		if (jump && !anim.GetBool(jumpBool) && IsGrounded())
		{
			// Set jump boolean on the Animator controller.
			anim.SetBool(jumpBool, true);
			if(speed > 0)
			{
				// Set jump vertical impulse when moving.
				rbody.AddForce (Vector3.up * jumpHeight * rbody.mass * 10, ForceMode.Impulse);
			}
		}
	}

    // Deal with the basic player movement
    void MovementManagement(float horizontal, float vertical, bool running)
	{
		// On ground, obey gravity.
		if (anim.GetBool(groundedBool))
			rbody.useGravity = true;

		// Call function that deals with player orientation.
		Rotating(horizontal, vertical);

		// Set proper speed.
		if(behaviourManager.IsMoving())
		{
			if(behaviourManager.isSprinting())
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
		anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
	}

	// Rotate the player to match correct orientation, according to camera and key pressed.
	Vector3 Rotating(float horizontal, float vertical)
	{
		// Get camera forward direction, without vertical component.
		Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

		// Player is moving on ground, Y component of camera facing is not relevant.
		forward.y = 0.0f;
		forward = forward.normalized;

		// Calculate target direction based on camera forward and direction key.
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		Vector3 targetDirection;
		float finalTurnSmoothing;
		targetDirection = forward * vertical + right * horizontal;
		finalTurnSmoothing = behaviourManager.turnSmoothing;

		// Lerp current direction to calculated target direction.
		if((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection);

			Quaternion newRotation = Quaternion.Slerp(rbody.rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);
			rbody.MoveRotation (newRotation);
			behaviourManager.SetLastDirection(targetDirection);
		}
		// If idle, Ignore current camera facing and consider last moving direction.
		if(!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
		{
			behaviourManager.Repositioning();
		}

		return targetDirection;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            if (!m_Dead)
            {
                BatController bc = other.gameObject.GetComponent<BatController>();
                int damage = (int)bc.GetDamage();
                if (damage > 0)
                {
                    this.m_HP -= damage;
                    StartCoroutine(Hit());
                    var relativePoint = transform.InverseTransformPoint(bc.m_Player.transform.position);


                    string[] hitArray = new string[] { "HitLeft", "HitRight", "HitFront", "HitBack" };
                    float xWeight, yWeight;
                    xWeight = Mathf.Abs(relativePoint.x);
                    yWeight = Mathf.Abs(relativePoint.y);
                    int animIndex = 0;
                    if (xWeight > yWeight)
                    {
                        if (relativePoint.x < 0)
                        {
                            animIndex = 0;
                        }
                        else
                        {
                            animIndex = 1;
                        }
                    }
                    else
                    {
                        if (relativePoint.z > 0)
                        {
                            animIndex = 2;
                        }
                        else
                        {
                            animIndex = 3;
                        }
                    }

                    anim.SetTrigger(hitArray[animIndex]);

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

    IEnumerator KillAfterDeath()
    {
        m_Dead = true;
        m_Bat.DeactivateCollider();
        yield return new WaitForSeconds(0.2f);
        GameObject ragdoll = Instantiate(m_RagDoll);

        ragdoll.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = m_DefaultColor;
        ragdoll.transform.position = this.transform.position;
        ragdoll.transform.rotation = this.transform.rotation;
        yield return new WaitForSeconds(5.0f);
        m_DeathCam.enabled = true;
        m_MainCam.gameObject.GetComponent<ThirdPersonOrbitCam>().enabled = false;
        m_MainCam.enabled = false;
        Destroy(this.gameObject);
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
