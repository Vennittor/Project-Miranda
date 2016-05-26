using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BootSequence : MonoBehaviour {
	[SerializeField]
	QuotePayload dialogue = QuotePayload.Uninitialized;

	[SerializeField]
	KeyCode bootKey = KeyCode.Space;

	[SerializeField]
	string targetScene = string.Empty;

	[SerializeField]
	float textResetDelay = 15f;

	void Start() {
		QuoteBoxManager.Instance.DefaultQuoteBox.DefaultTextSpeed = QuoteBoxSpeed.FAST;
		StartCoroutine(DelayTextReset(textResetDelay));
	}

	void Update() {
		if (Input.GetKeyDown(bootKey)) {
			SceneManager.LoadScene(targetScene);
		}
	}

	IEnumerator DelayTextReset(float mSeconds) {
		while(isActiveAndEnabled) {
			QuoteBoxManager.Instance.ShowText(dialogue);
			yield return new WaitForSeconds(mSeconds);
			SceneManager.LoadScene("Boot");
		}
	}
}
