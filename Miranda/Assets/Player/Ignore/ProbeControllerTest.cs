using UnityEngine;
using System.Collections;

public class ProbeControllerTest : MonoBehaviour {
	Transform _transform = null;
	Rigidbody _rigidbody = null;
	PlayerScanner _scanner = null;

	// LAZY
	KeyCode keyScan = KeyCode.F;
	KeyCode keyStabilize = KeyCode.X;

	[Header("Movement")]
	[SerializeField]
	float moveSpeed = 5.0f;
	[SerializeField]
	float rotateSpeed = 10f;
	bool isControllable = true;
	float mouseSpeed = 5f;

	[Header("Scan")]
	[SerializeField]
	float scanWarmup = 1.0f;
	[SerializeField]
	float scanCooldown = 2.0f;
	bool isScanning = false;

	#region Properties
	bool IsControllable {
		set{isControllable = value;}
	}
	#endregion

	#region MonoBehaviour
	void Awake() {
		LinkReferences();
	}

	void Start() {
		// TODO need to unsubscribe at some point to avoid leaking
		GameManager.Instance.EvOnPauseSet += SetControllable;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void FixedUpdate() {
		if (Input.GetKeyDown(KeyCode.Escape))
			Cursor.lockState = CursorLockMode.None;
		
		if (isControllable) {
			//_transform.Translate(UpdateMovementInput());

			_rigidbody.velocity = UpdateMovementInput();

			_transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X") * -1 * mouseSpeed, 0));
			//_transform.Rotate(0, CustomInput.GetAxisDirection("RotateZ") * rotateSpeed * Time.deltaTime * 10, 0);

			if (Input.GetKeyDown(KeyCode.X)) {
				StabilizeVelocity();
			}

			if (Input.GetKey(keyStabilize)) {
				StabilizeRotation();
			}

			if (CustomInput.GetButtonDown(keyScan) && !isScanning) {
				StartCoroutine(Scan());
			}
		}
	}
	#endregion

	IEnumerator Scan() {
		isScanning = true;
		// tether effect or something idk
		yield return new WaitForSeconds(scanWarmup);
		_scanner.PerformScanOnNearest();
		yield return new WaitForSeconds(scanCooldown);
		isScanning = false;
	}

	Vector3 UpdateMovementInput() {
		Vector3 tMovement = _transform.TransformDirection(Vector3.forward) * moveSpeed
			* Time.fixedDeltaTime * CustomInput.GetAxisDirection("Vertical") * 10;
		Vector3 tMovemen2 = _transform.TransformDirection(Vector3.right) * moveSpeed
			* Time.fixedDeltaTime * CustomInput.GetAxisDirection("Horizontal") * 10;


		tMovement = tMovement + tMovemen2;
		/*
		// LAZY HACK
		if (CustomInput.GetButton(keyMoveUp))
			tMovement += Vector3.up * moveSpeed * Time.fixedDeltaTime;
		else if (CustomInput.GetButton(keyMoveDown))
			tMovement += -Vector3.up * moveSpeed * Time.fixedDeltaTime;
		*/

		return tMovement;
	}

	// really wish we didn't have a button to do this. It's awful
	void StabilizeVelocity() {
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.angularVelocity = Vector3.zero;
	}

	void StabilizeRotation() {
		// TODO gradual process
		//Quaternion.Lerp(_transform.localRotation,Quaternion.identity,1.0f);
		_transform.localRotation = Quaternion.Euler(0,0,0);
	}

	void LinkReferences() {
		_rigidbody = GetComponent<Rigidbody>();
		_transform = GetComponent<Transform>();

		_scanner = GetComponentInChildren<PlayerScanner>();
	}

	void SetControllable(bool mControllable) {
		isControllable = !mControllable;
	}
}
