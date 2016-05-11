using UnityEngine;
using System.Collections;

public class ProbeController : MonoBehaviour
{
	Rigidbody rb;

	[Header("References")]

	public PlayerScanner interactField = null;
	public Light ambientLight = null;
	public Light spotLight = null;

	private Color defaultLightColor = Color.white;
	[Header("Movement")]
	//readout Variables,  for testing only
	public float ZSpeed = 0.0f;
	public float XSpeed = 0.0f;
	public float YSpeed = 0.0f;

	//velocity varialbes
	public float moveSpeedMax = 10.0f;	//maximum speed allowed in any particular direction
	public float throttleSpeed = 1.0f;	//if velocity in any direction drops below this value, it well be set to zero
	public float forwardAcc = 8.0f;
	public float horAcc = 8.0f;
	public float rotateSpeed = 5.0f;

	public float normalDrag = 1.0f;
	public float normalAngularDrag = 1.0f;

	public float stabilizeDrag = 5.0f;
	public float stabilizeAngDrag = 5.0f;

	public float mousespeed = 5.0f;
	public int currentLight = 0;

	public bool reverseYInput = true;

	private bool isControllable = true;


	[Header("Scan")]
	[SerializeField] float scanDelay = 1.0f;

	#region Obselete stuff
	//Input checks
	/*
	bool moveForward = false;
	bool moveBackward = false;
	bool moveLeft = false;
	bool moveRight = false;

	bool rotateLeft = false;
	bool rotateRight = false;

	bool stabilizer = false;

	bool pressR = false;
	bool PressF = false;
	*/
	#endregion

	void Start () 
	{
		rb = GetComponent<Rigidbody>();

		rb.drag = normalDrag;
		rb.angularDrag = normalAngularDrag;

		GameManager.Instance.EvOnPauseSet += SetControllable;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
		}

		if (isControllable) {
			Vector3 currentVel = rb.velocity;

			HandleComponentInput();
			UpdateMouselook();
			ApplyForces ();
			ThrottleVelocity ();
			Stabilize ();

			//Velocity information, testing only
			ZSpeed = currentVel.z;
			XSpeed = currentVel.x;
			YSpeed = currentVel.y;
			// Why do these need to be allocated to separate values?
			// Wrapping in a Vector3 would probably be okay.
		}
	}

	#region Update phases
	void HandleComponentInput() {
		if (Input.GetKeyDown(KeyCode.F)) 
		{
			StartCoroutine(Scan());
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

	void UpdateMouselook() 
	{
		Vector3 mouseAxis = new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X") * mousespeed, 0);

		if (reverseYInput)
			mouseAxis.x *= -1;

		transform.Rotate(mouseAxis);
	}

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

		rb.AddForce (force);

		//Adjust Angular Velocities (Z Axis)
		force = Vector3.zero;

		if (Input.GetKey(KeyCode.Q)) 
			force += rotateSpeed * transform.forward;

		if (Input.GetKey (KeyCode.E)) 
			force += rotateSpeed * -transform.forward;

		rb.AddTorque (force);

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

	void SetControllable(bool mControllable) {
		isControllable = !mControllable;
	}

	IEnumerator Scan() {
		interactField.SetVisible(true);
		yield return new WaitForSeconds(scanDelay);
		interactField.PerformScanOnAll();
		interactField.SetVisible(false);
	}
}
