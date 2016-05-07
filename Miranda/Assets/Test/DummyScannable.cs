using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DummyScannable : MonoBehaviour, IScannable, IInteractable {
	Renderer _renderer = null;

	[SerializeField]
	QuotePayload message = QuotePayload.Uninitialized;

	void Awake() {
		_renderer = GetComponent<Renderer>();
	}

	public void OnScanned() {
		Debug.Log(name + " detected.",this);
		StartCoroutine(ChangeColor(Color.blue));
		QuoteBoxManager.Instance.ShowText(message);
	}

	public void OnInteracted() {
		
	}

	IEnumerator ChangeColor(Color mColor) {
		_renderer.material.color = mColor;
		yield return new WaitForSeconds(1.0f);
		_renderer.material.color = Color.white;
	}
}
