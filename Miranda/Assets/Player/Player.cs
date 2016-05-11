using UnityEngine;
using System.Collections;
using System;

// lol naming conventions
[Flags]
public enum ThingState : byte {
	NONE			= 0 << 0,	// 0000 0000
	SPORE_THREAT	= 1 << 0,	// 0000 0001
}

// set		a | b
// unset	a & (~b)
// has		(a & b) == b
// toggle	a ^ b

public class Player : MonoBehaviour {
	[SerializeField]
	ThingState state = ThingState.NONE;

	PlayerEffects effectsHandler = null;

	#region Properties
	public ThingState State {
		get{return state;}
		set{state = value;}
	}
	#endregion

	#region MonoBehaviour
	void Awake() {
		LinkReference();
	}
	#endregion

	#region Assign states
	public void SetThreatSpores(bool mToSet) {
		ThingState flagSet = ThingState.SPORE_THREAT;

		if (mToSet)
			state = state | flagSet;
		else
			state = state & (~flagSet);

		effectsHandler.SetSporeThreat(mToSet);
	}
	#endregion

	void LinkReference() {
		effectsHandler = GetComponent<PlayerEffects>();
	}
}
