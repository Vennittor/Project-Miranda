﻿using UnityEngine;
using System.Collections;

public class Scannable : MonoBehaviour, IScannable
{
	bool beenScanned = false;

	public IEvent eventScript = null;	//Attach The EventScript to be referenced must be on the same GameObject

	Renderer _renderer = null;
	Color baseColor = Color.white;
	public Color scanColor = Color.blue;

	public Color flashColor = Color.cyan;
	public float flashDelay = 2.6f;

	[SerializeField]
	QuotePayload message = QuotePayload.Uninitialized;

	void Awake() {

		if (GetComponent<Renderer> () != null)
		{
			_renderer = GetComponent<Renderer> ();

			baseColor = GetComponent<Renderer> ().material.color;
		}
	}

	void Start()
	{
		eventScript = GetComponent<IEvent> ();
	}

	public void OnScanned() {
		Debug.Log(name + " detected.",this);
		beenScanned = true;
		StartCoroutine(ChangeColor(scanColor));
		QuoteBoxManager.Instance.ShowText(message);

		if (eventScript != null) 
		{
			eventScript.StartEvent ();
		}
	}

	IEnumerator ChangeColor(Color mColor) 
	{
		if (_renderer != null) 
			_renderer.material.color = mColor;
		
			yield return new WaitForSeconds (1.0f);

		if (_renderer != null) 
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
