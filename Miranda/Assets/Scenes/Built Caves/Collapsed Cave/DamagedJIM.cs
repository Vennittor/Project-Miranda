using UnityEngine;
using System.Collections;

public class DamagedJIM : MonoBehaviour 
{
	public Light JIMlight = null;

	public int flickers = 10;
	public float flickerTime = 2.0f;
	public float waitTime = 4.0f;

	void Start () 
	{
		JIMlight = GetComponent<Light> ();
		StartCoroutine ( FlickerLight () );
	}

	IEnumerator FlickerLight()
	{
		float waitTime = 0f;
		float delay = 0f;
		while (waitTime < flickerTime) 
		{
			JIMlight.enabled = false;

			delay = Random.Range(0.05f, flickerTime/flickers);
			waitTime += delay;

			yield return new WaitForSeconds (delay);

			JIMlight.enabled = true;

			delay = flickerTime / flickers - delay;
			waitTime += delay;

			yield return new WaitForSeconds (delay);

		}

		JIMlight.enabled = false;

		StartCoroutine ( FlickerWait () );
	}

	IEnumerator FlickerWait()
	{
		yield return new WaitForSeconds (waitTime);

		StartCoroutine ( FlickerLight () );
	}
}
