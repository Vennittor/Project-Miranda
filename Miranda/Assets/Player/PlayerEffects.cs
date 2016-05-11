using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour {
	[SerializeField]
	ParticleSystem sporeThreat = null;

	public void SetSporeThreat(bool mToSet) {
		sporeThreat.gameObject.SetActive(mToSet);
	}
}
