using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetListener : MonoBehaviour {
	[SerializeField]
	KeyCode keyReset = KeyCode.F1;

	void Update() {
		if (Input.GetKeyDown(keyReset))
			SceneManager.LoadScene("Boot");	
	}
}
