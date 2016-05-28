using UnityEngine;
using System.Collections;

public class LodgedObject : MonoBehaviour, ITether 
{
	Rigidbody rb;

	void Start () 
	{
		rb = this.GetComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.isKinematic = true;
	}

	void Update ()
	{
	
	}

	public void Tether()
	{
		rb.useGravity = true;
		rb.isKinematic = false;
	}
}
