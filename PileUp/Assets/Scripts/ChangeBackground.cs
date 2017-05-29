using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeBackground : MonoBehaviour 
{
	Color current;
	Color bgcolor;
	float duration = 1.5f;
	private float t = 0;


	void Start () 
	{
		InvokeRepeating ("ChangeColors", 0.0f, 5.0f); 
	}


	//Changing Background Colors smoothly.
	private void ChangeColors()
	{
		
		current = Camera.main.backgroundColor;
		bgcolor = new Color (Random.value, Random.value, Random.value);
		Camera.main.backgroundColor = Color.Lerp (current, bgcolor, t+0.1f);

		if (t < 1)
		{ 
			t += Time.deltaTime/duration;
		}
	}

}
