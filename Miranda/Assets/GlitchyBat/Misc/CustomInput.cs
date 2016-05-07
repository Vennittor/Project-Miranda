using UnityEngine;
using System.Collections;

public static class CustomInput {
	public static float deadzone = 0.1f;

	#region Axis
	public static bool GetAxisDown(string mAxisName) {
		if (Input.GetAxis(mAxisName) > Mathf.Abs(deadzone))
			return true;
		return false;
	}

	public static int GetAxisDirection(string mAxisName) {
		if (Input.GetAxis(mAxisName) > deadzone)
			return 1;
		else if (Input.GetAxis(mAxisName) < -deadzone)
			return -1;
		return 0;
	}
	#endregion

	#region Buttons
	public static bool GetButtonDown(string mButtonName) {
		return Input.GetButtonDown(mButtonName);
	}

	public static bool GetButtonUp(string mButtonName) {
		return Input.GetButtonUp(mButtonName);
	}

	public static bool GetButton(string mButtonName) {
		return Input.GetButton(mButtonName);
	}
	#endregion

	#region Keys
	public static bool GetButtonDown(KeyCode mKey) {
		return Input.GetKeyDown(mKey);
	}

	public static bool GetButtonUp(KeyCode mKey) {
		return Input.GetKeyUp(mKey);
	}

	public static bool GetButton(KeyCode mKey) {
		return Input.GetKey(mKey);
	}
	#endregion
}
