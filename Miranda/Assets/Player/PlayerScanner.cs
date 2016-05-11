using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PlayerScanner : MonoBehaviour {
	Transform _transform = null;
	//Collider _collider = null;
	Renderer _renderer = null;

	[SerializeField]
	List<GameObject> targets = new List<GameObject>();

	#region MonoBehaviour
	void Awake() {
		LinkReferences();
		SetVisible(false);
	}

	void Update() {
		for(int i=0; i < targets.Count; i++) {
			if (targets[i])
				Debug.DrawLine(transform.position,
					targets[i].transform.position);
			else
				targets.Remove(targets[i]);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (!targets.Contains(other.gameObject))
			targets.Add(other.gameObject);
	}

	void OnTriggerExit(Collider other) {
		if (targets.Contains(other.gameObject))
			targets.Remove(other.gameObject);
	}
	#endregion

	/// <summary>
	/// Performs a scan based on the collider. Returns the final target chosen.
	/// </summary>
	/// <returns>Scanned target</returns>
	public GameObject PerformScanOnNearest() {
		GameObject tNearest = null;
		List<GameObject> tAll = new List<GameObject>();
		tAll.AddRange(FindScannablesInObjects(targets));
		if (tAll.Count > 0) {
			tNearest = FindClosestTarget(tAll);
			ScanTarget(tNearest);
		}
		return tNearest;
	}

	public List<GameObject> PerformScanOnAll() {
		List<GameObject> tAll = new List<GameObject>();
		tAll.AddRange(FindScannablesInObjects(targets));
		foreach(GameObject go in tAll)
			ScanTarget(go);
		return tAll;
	}

	GameObject FindClosestTarget(List<GameObject> mTargets) {
		GameObject tNearestTarget = null;
		float tNearestDistance = -1f;
		float d = -1f;

		Debug.Log("Scannables detected: " + mTargets.Count);
		foreach(GameObject t in mTargets) {
			Debug.Log("Scannable: " + t);

			d = Vector3.Distance(t.transform.position,_transform.position);
			if (d < tNearestDistance) {
				Debug.Log("New closest object: " + t);
				tNearestDistance = d;
				tNearestTarget = t;
			}
			tNearestTarget = t;
		}

		return tNearestTarget;
	}

	void ScanTarget(GameObject mTarget) {
		try{
			mTarget.GetComponent<IScannable>().OnScanned();
		} catch (System.Exception e) {
			Debug.LogError("UUTF??" + e,this);
		}
	}

	public void SetVisible(bool isVisible) {
		_renderer.enabled = isVisible;
	}

	#region Find Scannables
	List<GameObject> FindScannablesInObjects(List<GameObject> mObjects) {
		List<GameObject> scannables = new List<GameObject>();

		foreach(GameObject go in mObjects) {
			if (ValidateScannable(go))
				scannables.Add(go);
		}
		return scannables;
	}

	bool ValidateScannable(GameObject mHit) {
		if (mHit.GetComponent<IScannable>() != null)
			return true;
		return false;
	}
	#endregion

	void LinkReferences() {
		_transform = GetComponent<Transform>();
		//_collider = GetComponent<Collider>();
		_renderer = GetComponent<Renderer>();
	}
}
