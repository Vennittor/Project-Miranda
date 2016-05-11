using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	Transform _transform = null;
	PlayerScanner _scanner = null;

	KeyCode keyMoveUp = KeyCode.Q;
	KeyCode keyMoveDown = KeyCode.E;
	KeyCode keyScan = KeyCode.Space;

	[Header("Movement")]
	[SerializeField]
	float moveSpeed = 5.0f;
	[SerializeField]
	float rotateSpeed = 10f;
	bool isControllable = true;

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
		GameManager.Instance.EvOnPauseSet += SetControllable;
	}

	void FixedUpdate() {
		if (isControllable) {
			_transform.Translate(UpdateMovementInput());
			_transform.Rotate(0, CustomInput.GetAxisDirection("Horizontal") * rotateSpeed * Time.deltaTime, 0);

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
		Vector3 tMovement =
			CustomInput.GetAxisDirection("Vertical") * moveSpeed * Vector3.forward * Time.deltaTime;

		// LAZY HACK
		if (CustomInput.GetButton(keyMoveUp))
			tMovement += Vector3.up * moveSpeed * Time.deltaTime;
		else if (CustomInput.GetButton(keyMoveDown))
			tMovement += -Vector3.up * moveSpeed * Time.deltaTime;

		return tMovement;
	}

	void LinkReferences() {
		_transform = GetComponent<Transform>();

		_scanner = GetComponentInChildren<PlayerScanner>();
	}

	void SetControllable(bool mControllable) {
		isControllable = !mControllable;
	}
}
