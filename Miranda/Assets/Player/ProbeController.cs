using UnityEngine;
using System.Collections;

public class ProbeController : MonoBehaviour
{
	Rigidbody rb;

	[Header("References")]

	public PlayerScanner interactField = null;
	public Light ambientLight = null;
	public Light spotLight = null;
	public Light indicatorLight = null;

	public TestTeleport testTeleport = null;

	private Color defaultLightColor = Color.white;

	[Header("Movement")]
	//velocity varialbes
	public float moveSpeedMax = 10.0f;	//maximum speed allowed in any particular direction
	public float throttleSpeed = 1.0f;	//if velocity in any direction drops below this value, it well be set to zero
	public float forwardAcc = 8.0f;
	public float horAcc = 8.0f;
	public float vertAcc = 8.0f;
	public float shiftAcc = 1.5f;
	public float rotateSpeed = 5.0f;

	public float normalDrag = 1.0f;
	public float normalAngularDrag = 1.0f;
	public float stabilizeDrag = 5.0f;
	public float stabilizeAngDrag = 5.0f;

	public float mousespeed = 5.0f;
	public int currentLight = 0;

	[Header("Tools")]
	//Temp variables change to Enumerators and a List of current tools.
	public int leftTool = 0;
	public int rightTool = 0;

	public bool scanning = false;

	public Joint tetherJoint = null;
	public GameObject tetheredObject = null;
	public bool flashingTether = false;

	public float laserRadius = 1.0f;
	public float laserDistance = 10.0f;
	bool flashingLaser = false;
	public float laserDelay = 1.0f;

	[Header("  Tool Effects")]	 //Tool Particle Effects

	public GameObject scanner = null;
	public ParticleSystem scannerPart = null;
	public GameObject laser = null;
	public GameObject tether = null;
	public GameObject shield = null;
	public GameObject teleport = null;

	[Header("Control Options")]
	public bool reverseYInput = true;

	private bool isControllable = true;

	[Header("Scan")]
	[SerializeField] float scanDelay = 1.0f;

	void Start () 
	{
		rb = GetComponent<Rigidbody>();

		rb.drag = normalDrag;
		rb.angularDrag = normalAngularDrag;

		GameManager.Instance.EvOnPauseSet += SetControllable;
		Cursor.lockState = CursorLockMode.Locked;

		scanner.SetActive(false);
		laser.SetActive(false);
		tether.SetActive(false);
		shield.SetActive(false);
		//teleport.SetActive(false);

		flashingLaser = false;

		scannerPart = scanner.GetComponent<ParticleSystem> ();



		if(gameObject.GetComponent<TestTeleport>() != null)
			testTeleport = gameObject.GetComponent<TestTeleport>();
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
		}

		if (isControllable) {

			HandleComponentInput();
			ActivateTools ();
			UpdateMouselook();
			ApplyForces ();
			ThrottleVelocity ();
			Stabilize ();

		}
	}

	#region Update phases
	void HandleComponentInput() {
		if (Input.GetKeyDown(KeyCode.F)) 
		{
			if( !scanning )
			StartCoroutine( Scan() );
		}

		if (Input.GetKeyDown(KeyCode.R)) 
		{
			ambientLight.enabled = !ambientLight.enabled;
			spotLight.enabled = !spotLight.enabled;
		}

		if(Input.GetKeyDown(KeyCode.V))
		{
			reverseYInput = !reverseYInput;
		}
			
	}

	void ActivateTools()
	{
		#region Tool Switch Temp
		if (Input.GetKeyDown (KeyCode.Z)) 
		{
			if(leftTool == 0)
				TetherRelease();
			
			leftTool++;
			if (leftTool > 1)
				leftTool = 0;
		}
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			rightTool++;
			if(rightTool > 1)
				rightTool = 0;
		}
		#endregion

		//Left Tools
		if (Input.GetMouseButtonUp (0)) 
		{
			TetherRelease ();
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (leftTool == 0) 
			{
				Tether ();
			}

			//Laser
			if (leftTool == 1)
			{
				Laser ();
			}
		}

		//Right Tools
		if(Input.GetMouseButtonDown(1))
		{
			if (leftTool == 0) 
			{
				//Shield();
			}

			if (leftTool == 1) 
			{
				//Teleport();
			}
		}


	}

	void UpdateMouselook() 
	{
		Vector3 mouseAxis = new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X") * mousespeed, 0);

		if (reverseYInput)
			mouseAxis.x *= -1;

		transform.Rotate(mouseAxis);
	}

		#region Movement Forces
	void ApplyForces()
	{
		Vector3 force = Vector3.zero;

		//Adjust Planar
		if (Input.GetKey(KeyCode.W)) 
			force += forwardAcc * transform.forward;

		if (Input.GetKey(KeyCode.S)) 
			force += forwardAcc * -transform.forward;

		if (Input.GetKey(KeyCode.A)) 
			force += horAcc * -transform.right;

		if (Input.GetKey (KeyCode.D)) 
			force += horAcc * transform.right;
		if(Input.GetKey (KeyCode.Space))
		{
			force += vertAcc * transform.up;
		}
		if (Input.GetKey (KeyCode.X)) 
		{
			force += vertAcc * -transform.up;
		}

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) 
		{
			force *= shiftAcc;
		}

		rb.AddForce (force, ForceMode.Acceleration);

		//Adjust Angular Velocities (Z Axis)
		force = Vector3.zero;

		if (Input.GetKey (KeyCode.Q)) 
		{
			transform.Rotate (Vector3.forward * rotateSpeed * Time.deltaTime);
			//force += rotateSpeed * transform.forward;
		}

		if (Input.GetKey (KeyCode.E))
		{
			transform.Rotate (-Vector3.forward * rotateSpeed * Time.deltaTime);
			//force += rotateSpeed * -transform.forward;
		}

		//rb.AddTorque (force, ForceMode.Acceleration);

	}


	void ThrottleVelocity()
	{
		//Get current Velocity
		Vector3 currentVel = Vector3.zero;
		currentVel = rb.velocity;

		Vector3 setVel = currentVel;

		setVel.x = Mathf.Clamp(setVel.x,-moveSpeedMax,moveSpeedMax);
		setVel.y = Mathf.Clamp(setVel.y,-moveSpeedMax,moveSpeedMax);
		setVel.z = Mathf.Clamp(setVel.z,-moveSpeedMax,moveSpeedMax);

		//if velocity is minute, set to zero
		if (Mathf.Abs(currentVel.x) < 0.05f) 
			setVel.x = 0f;
		
		if (Mathf.Abs(currentVel.y) < 0.05f) 
			setVel.y = 0f;
		
		if (Mathf.Abs(currentVel.z) < 0.05f) 
			setVel.z = 0f;

		//apply changes to velocity
		rb.velocity = setVel;


	}

	void Stabilize()
	{
		if (Input.GetKey (KeyCode.X))
		{
			rb.drag = stabilizeDrag;
			rb.angularDrag = stabilizeAngDrag;
		} else {
			rb.drag = normalDrag;
			rb.angularDrag = normalAngularDrag;
		}


	}
		#endregion
	#endregion

	void SetControllable(bool mControllable) {
		isControllable = !mControllable;
	}

	IEnumerator Scan() 
	{
		scanning = true;
		scanner.SetActive(true);
		//interactField.SetVisible(true);

		yield return new WaitForSeconds(scanDelay);

		interactField.PerformScanOnAll();
//		interactField.SetVisible(false);
		scanner.SetActive(false);

		scanning = false;
	}

	void Tether()
	{
		//Flash Tether effects
		//If no Object, stop
		//if object, continue tether

		//Sphere Cast
		//attach physics object to tether joint
		//tetherJoint.connectedBody = other.gameobject;

		tether.SetActive (true);
	}
	void TetherRelease()
	{
		//detach tethered object from joint.
		//tetherJoint.connectedBody = null;

		tether.SetActive (false);
	}

	void Laser()
	{
		if( flashingLaser == false)
		{
			StartCoroutine ( LaserAnimation() );
		}

		Ray ray = new Ray ();
		ray.origin = this.gameObject.transform.position;
		ray.direction = transform.forward;

		RaycastHit hit;

		if (Physics.SphereCast (ray, laserRadius, out hit, laserDistance)) 
		{
			GameObject hitObject = hit.collider.gameObject;

			while (hitObject.transform.parent != null)
			{
				hitObject = hitObject.transform.parent.gameObject;
			}

			if( hitObject.GetComponent<IBurn>() != null)
			{
				hitObject.GetComponent<IBurn>().Burn();
			}
		}
	}
	IEnumerator LaserAnimation()
	{
		flashingLaser = true;
		//do startup flash of animation
		laser.SetActive(true);

		yield return new WaitForSeconds(laserDelay);

		laser.SetActive(false);

		flashingLaser = false;
		//stop animation
	}

	void Shield()
	{

	}

	void Teleport()
	{

	}
}
