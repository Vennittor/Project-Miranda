using UnityEngine;
using System.Collections;

public class ProbeController : MonoBehaviour
{
	public Rigidbody rb;

	//readout Variables,  for testing only
	public float ZSpeed = 0.0f;
	public float XSpeed = 0.0f;
	public float YSpeed = 0.0f;

	//velocity varialbes
	public float moveSpeedMax = 10.0f;	//maximum speed allowed in any particular direction
	public float throttleSpeed = 1.0f;	//if velocity in any direction drops below this value, it well be set to zero
	public float forwardAcc = 8.0f;
	public float horAcc = 8.0f;
	public float angularAcc = 5.0f;

	public float normalDrag = 1.0f;
	public float normalAngularDrag = 1.0f;

	public float stabilizeDrag = 5.0f;
	public float stabilizeAngDrag = 5.0f;

	//Input checks
	bool moveForward = false;
	bool moveBackward = false;
	bool moveLeft = false;
	bool moveRight = false;

	bool rotateLeft = false;
	bool rotateRight = false;

	bool stabilizer = false;

	bool pressR = false;
	bool PressF = false;

	//old variables
//	public float forwardSpeed = 0.0f;
//
//	public float forwardDec = 1.0f;
//	public float forwardCap = 25.0f;
//
//	public float horSpeed = 0.0f;
//
//	public float horDec = 1.0f;
//	public float horCap = 25.0f;
//
//	public float rotSpeed = 0.0f;
//	public float rotAcc = 8.0f;
//	public float rotDec = 8.0f;
//	public float rotCap = 60.0f;
//
//	public float defautlDec = 1.0f;

	public float mousespeed = 5.0f;
	public GameObject interactFeild = null;
	public int CurrentLight = 0;
	public GameObject ambeintLight = null;
	public GameObject spotLight = null;

	void Start () 
	{
		rb = GetComponent <Rigidbody> ();

		normalDrag = rb.drag;
		normalAngularDrag = rb.angularDrag;
	}
	
	void Update () 
	{
		Vector3 currentVel = rb.velocity;

		GatherInputs ();
		ApplyForces ();
		ThrottleVelocity ();
		Stabilize ();

		//Velocity information, testing only
		ZSpeed = rb.velocity.z;
		XSpeed = rb.velocity.x;
		YSpeed = rb.velocity.y;

	}

	void GatherInputs ()
	{
		//PITCH AND YAW MOVEMENT
		transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, Input.GetAxis("Mouse X")) * -1 * mousespeed);

		moveForward = false;
		moveBackward = false;
		moveLeft = false;
		moveRight = false;

		rotateLeft = false;
		rotateRight = false;

		stabilizer = false;

		pressR = false;
		PressF = false;

		//Forward / backward Inputs
		if (Input.GetKey (KeyCode.W)) 
		{
			moveForward = true;
		}
		if (Input.GetKey (KeyCode.S)) 
		{
			moveBackward = true;
		}

		//Left / Right Inputs
		if (Input.GetKey (KeyCode.A)) 
		{
			moveLeft = true;
		}
		if (Input.GetKey (KeyCode.D)) 
		{
			moveRight = true;
		}

		//Rotate Left / Right
		if (Input.GetKey (KeyCode.Q))
		{
			rotateLeft = true;
		}
		if (Input.GetKey (KeyCode.E))
		{
			rotateRight = true;
		}

		//Stabilizer
		if (Input.GetKey (KeyCode.X))
		{
			stabilizer = true;
		}


	}

	void ApplyForces()
	{
		Vector3 force = Vector3.zero;

		//Adjust Planar
		if (moveForward) 
		{
			force += forwardAcc * transform.up;
		}

		if (moveBackward) 
		{
			force += forwardAcc * -transform.up;
		}

		if (moveLeft) 
		{
			force += horAcc * -transform.right;
		}

		if (moveRight) 
		{
			force += horAcc * transform.right;
		}

		rb.AddForce (force);

		//Adjust Angular Velocities (Z Axis)
		force = Vector3.zero;

		if (rotateLeft) 
		{
			force += angularAcc * -transform.right;
		}
		if (rotateRight) 
		{
			force += angularAcc * transform.right;
		}

		rb.AddTorque (force);

	}

	void ThrottleVelocity()
	{
		//Get current Velocity
		Vector3 currentVel = Vector3.zero;
		currentVel = rb.velocity;

		Vector3 setVel = currentVel;

		//check each direction and keep it under the max move speed
		if (currentVel.x > moveSpeedMax) 
		{
			setVel.x = moveSpeedMax;
		}
		if (currentVel.x < -moveSpeedMax)
		{
			setVel.x = -moveSpeedMax;
		}

		if (currentVel.y > moveSpeedMax) 
		{
			setVel.y = moveSpeedMax;
		}
		if (currentVel.y < -moveSpeedMax)
		{
			setVel.y = -moveSpeedMax;
		}

		if (currentVel.z > moveSpeedMax) 
		{
			setVel.z = moveSpeedMax;
		}
		if (currentVel.z < -moveSpeedMax)
		{
			setVel.z = -moveSpeedMax;
		}

		//if velocity is minute, set to zero
		if (currentVel.x < 0.05f && currentVel.x > -0.05f) 
		{
			setVel.x = 0f;
		}
		if (currentVel.y < 0.05f && currentVel.y > -0.05f) 
		{
			setVel.y = 0f;
		}
		if (currentVel.z < 0.05f && currentVel.z > -0.05f) 
		{
			setVel.z = 0f;
		}

		//apply changes to velocity
		rb.velocity = setVel;
	}

	void Stabilize()
	{
		if (stabilizer)
		{
			rb.drag = stabilizeDrag;
			rb.angularDrag = stabilizeAngDrag;
		} else {
			rb.drag = normalDrag;
			rb.angularDrag = normalAngularDrag;
		}
	
	}
}
