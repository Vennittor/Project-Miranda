using UnityEngine;
using System.Collections;

public enum SeekerState {
	NONE = 0,
	IDLE,
	WANDER,
	PURSUE
}

public class Seeker : Enemy {
	[Header("Seeker")]
	[SerializeField]
	SeekerState seekerState = SeekerState.NONE;

	[SerializeField]
	float wanderSpeed = 3f;
	[SerializeField]
	float pursuitSpeed = 6f;

	Vector3 homePos = Vector3.zero;

	#region Properties
	#endregion

	#region IScannable
	public override void OnScanned() {
		base.OnScanned();
	}
	#endregion

	#region MonoBehaviour
	void Start() {
		homePos = _transform.position;
		seekerState = SeekerState.IDLE;
	}

	void Update() {}
	#endregion

	#region Enemy
	public override void TickAI() {
		base.TickAI();

		switch(seekerState) {
		default: break;
		case SeekerState.IDLE:
			UpdateIdle();
			break;
		case SeekerState.PURSUE:
			UpdatePursue();
			break;
		}
	}
	#endregion

	void UpdateIdle() {
		if (PlayerIsNear()) {
			if ((player.State & ThingState.SPORE_THREAT) == ThingState.SPORE_THREAT) {
				seekerState = SeekerState.PURSUE;
			}
		}
		else {
			// TEMPORARY - return to home pos
			_transform.position = Vector3.MoveTowards(
				_transform.position,homePos,Time.fixedDeltaTime * wanderSpeed);
			_transform.LookAt(homePos);
		}
	}

	void UpdatePursue() {
		_transform.LookAt(player.transform);
		_transform.position = Vector3.MoveTowards(
			_transform.position,player.transform.position,Time.fixedDeltaTime * pursuitSpeed);
		if (!PlayerIsNear())
			seekerState = SeekerState.IDLE;
		if ((player.State & ThingState.SPORE_THREAT) != ThingState.SPORE_THREAT) {
			seekerState = SeekerState.IDLE;
		}
	}

	public override void LinkReferences() {
		base.LinkReferences();
	}
}
