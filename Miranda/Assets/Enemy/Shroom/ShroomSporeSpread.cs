using UnityEngine;
using System.Collections;

public class ShroomSporeSpread : MonoBehaviour {
	Transform _transform = null;

	[SerializeField]
	float timeSpan = 1f;
	[SerializeField]
	float growthRate = 0.1f;

	void Awake() {
		LinkReferences();
		Invoke("End",timeSpan);
	}

	void OnTriggerEnter(Collider other) {
		Player p = other.GetComponent<Player>();
		if (p) {
			p.SetThreatSpores(true);
		}
	}

	void FixedUpdate() {
		_transform.localScale = new Vector3(
			_transform.localScale.x+growthRate,
			_transform.localScale.y+growthRate,
			_transform.localScale.z+growthRate);
	}

	void End() {
		Destroy(gameObject);
	}

	void LinkReferences() {
		_transform = GetComponent<Transform>();
	}
}
