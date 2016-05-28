using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Scannable : MonoBehaviour, IScannable
{
	bool beenScanned = false;

	public MonoBehaviour eventScript = null;

	Renderer _renderer = null;
	Color baseColor = Color.white;
	public Color scanColor = Color.blue;

	public Color flashColor = Color.cyan;
	public float flashDelay = 2.6f;

	[SerializeField]
	QuotePayload message = QuotePayload.Uninitialized;

	void Awake() {
		_renderer = GetComponent<Renderer>();
		baseColor = GetComponent<Renderer>().material.color;
	}

	void Start()
	{

	}

	public void OnScanned() {
		Debug.Log(name + " detected.",this);
		beenScanned = true;
		StartCoroutine(ChangeColor(scanColor));
		QuoteBoxManager.Instance.ShowText(message);

		if (eventScript != null) 
		{
			//eventScript.Event ();
		}
	}

	IEnumerator ChangeColor(Color mColor) 
	{
		_renderer.material.color = mColor;
		yield return new WaitForSeconds(1.0f);
		_renderer.material.color = baseColor;
	}

	IEnumerator FlashColor()
	{
		while(!beenScanned)
		{
			
		yield return new WaitForSeconds (flashDelay);

		//Loop for colorflash increase
			//Increase Color by increment
			//yield return new WaitForSeconds(0.05f)
		
		//Loop for colorFlash decrease
			//decrease Color by increment
			//yield return new WaitForSeconds(0.05f)
		}
	}
}
