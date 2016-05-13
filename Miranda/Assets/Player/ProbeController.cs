﻿using UnityEngine;
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

	public Joint tetherJoint = null;
	public GameObject tetheredObject = null;

	public float laserRadius = 1.0f;
	public float laserDistance = 10.0f;

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
		if (Input.GetMouseButton(0))
		{
			if (leftTool == 0) 
			{
				Tether ();
			}

			if (leftTool == 1)
			{
				StartCoroutine ( LaserAnimation () );

				Ray ray = new Ray ();
				ray.origin = this.gameObject.transform.position;
				ray.direction = transform.forward;

				RaycastHit hit;
					
				if (Physics.SphereCast (ray, laserRadius, out hit, laserDistance)) {
					GameObject hitObject = hit.collider.gameObject;

					if (hitObject.transform.parent != null) {
						GameObject hitParent = hitObject.transform.parent.gameObject;

						if (hitParent.tag == "Destructable") {
							Destructable hitDestScript = hitParent.GetComponent<Destructable> ();
							hitDestScript.Break ();
						}
					}
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			TetherRelease ();
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

		rb.AddForce (force, ForceMode.Acceleration);

		//Adjust Angular Velocities (Z Axis)
		force = Vector3.zero;

		if (Input.GetKey(KeyCode.Q)) 
			force += rotateSpeed * transform.forward;

		if (Input.GetKey (KeyCode.E)) 
			force += rotateSpeed * -transform.forward;

		rb.AddTorque (force, ForceMode.Acceleration);

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

	void Tether()
	{
		//Tether ON
		//Flash Tether effects
		//If no Object, stop
		//if object, continue tether
		//StartCoroutine ( TetherAnimation() );

		//Sphere Cast
		//attach physics object to tether joint
		//tetherJoint.connectedBody = other.gameobject;
	}
	void TetherRelease()
	{
		//detach tethered object from joint.
		tetherJoint.connectedBody = null;
		StopCoroutine ( TetherAnimation () );
		//Tether OFF
	}

	IEnumerator TetherAnimation()
	{
		//set indicator color
		//turn indicator light on
		//start particle effect


		yield return new WaitForEndOfFrame();
	}

	IEnumerator LaserAnimation()
	{
		interactField.SetVisible(true);
		yield return new WaitForSeconds(scanDelay);
		interactField.SetVisible(false);
	}

	IEnumerator Scan() {
		interactField.SetVisible(true);
		yield return new WaitForSeconds(scanDelay);
		interactField.PerformScanOnAll();
		interactField.SetVisible(false);
	}
}
