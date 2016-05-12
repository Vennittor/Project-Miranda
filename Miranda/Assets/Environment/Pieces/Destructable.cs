using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour 
{
	public GameObject brokenPiece = null;


	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}

	public void Break()
	{
		GameObject loadedPiece = Instantiate (brokenPiece) as GameObject;

		loadedPiece.transform.DetachChildren ();
		Destroy (loadedPiece);

		Destroy (this.gameObject);
	}
}
