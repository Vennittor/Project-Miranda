using UnityEngine;
using System.Collections;

enum ShroomState : byte {
	NONE = 0,
	IDLE,
	WANDER,
	CHASE,
	SPORE
}

[RequireComponent(typeof(CharacterController))]
public class Shroom : Enemy {
	[Header("Shroom")]
	[SerializeField]
	ShroomState shroomState = ShroomState.NONE;

	[SerializeField]
	float wanderSpeed = 5f;
	[SerializeField]
	float chaseSpeed = 8f;

	Vector3 targetPosition = Vector3.zero;

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

	#region Enemy
	public override void TickAI () {
		base.TickAI();
		switch(shroomState) {
		default: break;
		case ShroomState.IDLE:
			UpdateIdle();
			break;
		case ShroomState.WANDER:
			UpdateWander();
			break;
		case ShroomState.SPORE:
			break;
		}
	}
	#endregion

	#region States
	void UpdateIdle() {
		// TODO clean this stuff up and cull magic numbers
		if (Random.Range(1,11) == 10) {
			targetPosition = new Vector3(
				Random.Range(startPosition.x-5,startPosition.x+5),
				transform.position.y,
				Random.Range(startPosition.z-5,startPosition.z+5));
			shroomState = ShroomState.WANDER;
		}

		if (PlayerIsNear()) {
			shroomState = ShroomState.CHASE;
		}
	}

	void UpdateWander() {
		targetPosition.Normalize();
		_controller.SimpleMove(targetPosition * wanderSpeed * Time.deltaTime * 10);

		if (PlayerIsNear()) {
			shroomState = ShroomState.CHASE;
		}
	}

	void UpdateChase() {
		targetPosition = player.transform.position;
		//targetPosition.Normalize();
		_controller.SimpleMove(targetPosition * chaseSpeed * Time.deltaTime * 10);
	}
	#endregion

	public override void LinkReferences() {
		base.LinkReferences();
	}
}
