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

	private int interactableLayer = 9;

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

	public bool useTetherJoint = true;
	public GameObject tetherGO = null;
	SpringJoint tetherJoint = null;
	public GameObject tetheredObject = null;
	public float tetherGrabRange = 0.7f;
	private bool flashingTether = false;
	private bool tethering = false;

	public float laserRadius = 1.0f;
	public float laserDistance = 10.0f;
	bool flashingLaser = false;
	public float laserDelay = 1.0f;
	private bool mining = false;

	private bool teleporting = false;

	private bool shielded = false;

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

		interactableLayer = LayerMask.NameToLayer("Interactable");

		scanner.SetActive(false);
		laser.SetActive(false);
		tether.SetActive(false);
		shield.SetActive(false);
		//teleport.SetActive(false);

		flashingLaser = false;

		scannerPart = scanner.GetComponent<ParticleSystem> ();

		tetherJoint = tetherGO.GetComponent<SpringJoint> ();

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
			if(tethering)
				TetherRelease();
			
			if( !scanning && !mining )
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
			if(tethering)
				TetherRelease();

			if(teleporting)
				TeleportOff();
			
			leftTool++;
			if (leftTool > 1)
				leftTool = 0;
		}
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			if(teleporting)
				TeleportOff();

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
				TeleportOff ();

				if( !flashingLaser)
					Tether ();
			}

			//Laser
			if (leftTool == 1)
			{
				TeleportOff();

				Laser ();
			}
		}

		//Right Tools
		if(Input.GetMouseButtonDown(1))
		{
			if (rightTool == 0) 
			{
				
				ShieldOn ();
			}

			if (rightTool == 1) 
			{
				if(!teleporting)
					TeleportOn();
			}
		}

		if (Input.GetMouseButtonUp (1))
		{
			ShieldOff ();

			if ( ValidTeleport () )
				Teleport ();
			else
				TeleportOff();
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

		Ray ray = new Ray ();
		ray.origin = this.gameObject.transform.position;
		ray.direction = transform.forward;

		RaycastHit hit;
		float castDist = Vector3.Distance(ray.origin, tetherGO.transform.position);

		if ( Physics.SphereCast(ray, tetherGrabRange, out hit, castDist) ) 
		{
			GameObject hitObject = hit.collider.gameObject;

			while (hitObject.transform.parent != null)
			{
				hitObject = hitObject.transform.parent.gameObject;
			}

			if( hitObject.GetComponent<ITether>() != null)
				hitObject.GetComponent<ITether>().Tether();

			if (hitObject.layer == interactableLayer && hitObject.GetComponent<Rigidbody>() != null)
			{
				tetheredObject = hitObject;

				tetherGO.SetActive (true);
				if (useTetherJoint) 
				{
					tetherJoint.connectedBody = tetheredObject.GetComponent<Rigidbody> ();
				}
				else 
				{
					tetheredObject.transform.SetParent(tetherGO.transform, true);
					tetheredObject.transform.position = tetherGO.transform.position;
					//move to tetherGO
				}
				tetheredObject.GetComponent<Rigidbody> ().useGravity = false;
			}

		}

		tether.SetActive (true);
	}
	void TetherRelease()
	{
		if (tetheredObject != null) 
		{
			if (useTetherJoint) 
			{
				tetherJoint.connectedBody = null;
			}
			else
			{
				tetheredObject.transform.SetParent (null, true);
			}
			tetheredObject.GetComponent<Rigidbody> ().useGravity = true;
		}
		tetherGO.SetActive(false);

		tethering = false;
		tether.SetActive (false);
	}

	void Laser()
	{
		if( flashingLaser == false)
		{
			StartCoroutine ( LaserAnimation() );
		}


	}
	IEnumerator LaserAnimation()
	{
		flashingLaser = true;
		//do startup flash of animation
		laser.SetActive(true);

		yield return new WaitForSeconds(laserDelay);

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

		laser.SetActive(false);

		flashingLaser = false;
		//stop animation
	}

	void ShieldOn()
	{
		shielded = true;

		shield.SetActive (true);
	}

	void ShieldOff()
	{
		shielded = false;

		shield.SetActive (false);
	}

	void TeleportOn()
	{
		teleporting = true;
		//if not teleporting
			//turn on Teleport child Object
			//switch controls to Teleport Child Object

				//if not Run Teleport
				//Else TeleportOff();
	}

	bool ValidTeleport()
	{
		//SphereCast to check if within other objects
		//if no invlaid objects
			//return true;
		//else
			return false;
	}

	void Teleport()
	{
		//Run Teleport effects
		//Move Probe to Teleport location
		//return Teleport Effect to Probe center
		//TeleportOff();
	}

	void TeleportOff()
	{
		teleporting = false;
		//return control to Probe
		//Teleport Effect Off
	}
}
