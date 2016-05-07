using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : Component {
	private static T _instance = null;

	private static bool isExiting = false;

	public static T Instance {
		get{
			if (_instance != null)
				return _instance;
			else if (_instance == null && !isExiting) {
				GameObject newManager = new GameObject(string.Empty);
				_instance = newManager.AddComponent<T>();
				_instance.name = "_Singleton" + _instance.GetComponent<T>().ToString();
				Debug.LogWarning("Created " + _instance.name + " as none was available.");
				return _instance;
			}
		return null;
		}
	}

	#region MonoBehaviour
	public virtual void Awake() {
		// check one frame late so all objects in world are prepared
		VerifySingleton();
		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationQuit() {
		isExiting = true;
	}
	#endregion

	protected void VerifySingleton() {
		// Kill self if a singleton is already established
		if (_instance != null) {
			Destroy(gameObject);
		}

		List<T> tInstances = new List<T>();
		tInstances.AddRange(FindObjectsOfType<T>());

		// Not sure which instance should be the singleton.
		if (tInstances.Count > 1 && _instance == null) {
			Debug.LogError("Multiple instances of a singleton exists, not sure which to set reference.",this);
			Debug.Break();
		}

		// I am the singleton
		if (tInstances.Count == 1 && _instance == null) {
			_instance = this as T;
		}
	}
}
