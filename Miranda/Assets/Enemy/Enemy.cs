using UnityEngine;
using System.Collections;

public enum EnemyState : byte {
	NONE = 0,
}

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour, IScannable {
	protected Transform _transform = null;
	protected CharacterController _controller = null;

	protected Player player = null;

	[Header("Enemy")]
	[SerializeField]
	protected QuotePayload description = QuotePayload.Uninitialized;
	[SerializeField]
	protected float delayAI = 0.1f;
	[SerializeField]
	protected float aggroRange = 5.0f;

	protected Vector3 startPosition = Vector3.zero;

	#region MonoBehaviour
	void Awake() {
		LinkReferences();
		startPosition = _transform.position;
		StartCoroutine(UpdateAI());
	}

	public virtual void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position,aggroRange);
	}
	#endregion

	#region IScannable
	public virtual void OnScanned() {
		QuoteBoxManager.Instance.ShowText(description);
	}
	#endregion

	public IEnumerator UpdateAI() {
		while (!GameManager.Instance.IsPaused) {
			TickAI();
			yield return new WaitForSeconds(delayAI);
		}
	}

	public virtual void TickAI() {
		
	}

	protected bool PlayerIsNear() {
		return (Vector3.Distance(_transform.position, player.transform.position) <= aggroRange);
	}

	public virtual void LinkReferences() {
		_transform = GetComponent<Transform>();
		_controller = GetComponent<CharacterController>();

		player = FindObjectOfType<Player>();
	}
}
