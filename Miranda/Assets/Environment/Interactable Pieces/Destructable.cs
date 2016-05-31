using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour, IBurn
{
	public GameObject brokenPiece = null;

	public void Burn()
	{
		GameObject loadedPiece = Instantiate (brokenPiece) as GameObject;

		loadedPiece.transform.position = this.transform.position;
		loadedPiece.transform.rotation = this.transform.rotation;
		loadedPiece.transform.SetParent (this.transform.parent);
		loadedPiece.transform.localScale = this.transform.localScale;

		loadedPiece.transform.DetachChildren ();
		Destroy (loadedPiece);

		Destroy (this.gameObject);
	}
}
