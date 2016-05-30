using UnityEngine;
using System.Collections;


public class FirstCaveTetherProbe : MonoBehaviour, IEvent 
{
	public bool eventPlayed = false;

	public Light ambientLight = null;
	public Light eyeLight = null;

	public int flickers = 5;
	public float startUpTime = 1.5f;

	public Light[] lights = new Light[0];


	void Start()
	{
		lights = GetComponentsInChildren<Light> ();

		foreach (Light light in lights) 
		{
			if (light.type == LightType.Spot && eyeLight == null)
				eyeLight = light;
			else if (light.type == LightType.Point && ambientLight == null)
				ambientLight = light;
		}

		if (eyeLight != null)
			eyeLight.enabled = false;
		if (ambientLight != null)
			ambientLight.enabled = false;
	}

	void Update () 
	{
	
	}

	public void StartEvent()
	{
		if (!eventPlayed && eyeLight != null)
			StartCoroutine( FlickerLight () );
	}
	public void StopEvent()
	{

	}

	IEnumerator FlickerLight()
	{
		eventPlayed = true;

		float waitTime = 0f;
		float delay = 0f;
		while (waitTime < startUpTime) 
		{
			eyeLight.enabled = true;
			ambientLight.enabled = true;

			delay = Random.Range(0.1f, startUpTime/flickers);
			waitTime += delay;

			yield return new WaitForSeconds (delay);

			eyeLight.enabled = false;
			ambientLight.enabled = false;

			delay = startUpTime / flickers - delay;
			waitTime += delay;

			yield return new WaitForSeconds (delay);

		}

		eyeLight.enabled = true;
		ambientLight.enabled = true;
	}

}
