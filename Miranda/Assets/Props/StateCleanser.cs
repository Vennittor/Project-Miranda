using UnityEngine;
using System.Collections;

public class StateCleanser : MonoBehaviour {
	public void Cleanse(Player mPlayer) {
		mPlayer.SetThreatSpores(false);
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Player>()) {
			Player p = other.GetComponent<Player>();
			Cleanse(p);
		}
	}
}
