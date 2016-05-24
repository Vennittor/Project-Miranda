using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour, IBurn
{
	public GameObject brokenPiece = null;

	public void Burn()
	{
		GameObject loadedPiece = Instantiate (brokenPiece) as GameObject;

		loadedPiece.transform.DetachChildren ();
		Destroy (loadedPiece);

		Destroy (this.gameObject);
	}
}
