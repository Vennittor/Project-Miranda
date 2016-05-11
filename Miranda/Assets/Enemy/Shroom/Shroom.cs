using UnityEngine;
using System.Collections;

enum ShroomState : byte {
	NONE = 0,
	IDLE,
	SPORE
}

public class Shroom : Enemy {
	[Header("Shroom")]
	[SerializeField]
	ShroomState shroomState = ShroomState.NONE;
	[SerializeField]
	ShroomSporeSpread attack = null;

	[SerializeField]
	float sporeStartDelay = 0f;
	[SerializeField]
	float sporeCooldown = 5f;
	[SerializeField]
	bool canSpore = true;

	#region IScannable
	public override void OnScanned() {
		base.OnScanned();
	}
	#endregion

	#region MonoBehaviour
	void Start() {
		shroomState = ShroomState.IDLE;
	}
	#endregion

	#region Spores
	IEnumerator SpewSpores() {
		canSpore = false;
		yield return new WaitForSeconds(sporeStartDelay);
		EmitSpores();
		yield return new WaitForSeconds(sporeCooldown);
		canSpore = true;
	}

	void EmitSpores() {
		if (attack)
			Instantiate(attack,_transform.position,Quaternion.identity);
		else
			Debug.LogError(name + " lacks its attack prefab.");
	}
	#endregion

	#region Enemy
	public override void TickAI() {
		base.TickAI();

		switch(shroomState) {
		default: break;
		}

		if (PlayerIsNear()) {
			if (canSpore)
				StartCoroutine(SpewSpores());
		}
	}
	#endregion

	public override void LinkReferences() {
		base.LinkReferences();
	}
}
