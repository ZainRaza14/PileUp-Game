using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RemoveRubble : MonoBehaviour 
{
	//For Removing the Waste Material when it falls down on the plane
	private void OnCollisionEnter(Collision c)
	{
		Destroy (c.gameObject);
	}

}
