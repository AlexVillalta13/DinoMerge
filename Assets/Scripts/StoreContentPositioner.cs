using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreContentPositioner : MonoBehaviour {

	public float yPosition;

	// Use this for initialization
	void Start () {
		//yPosition = transform.position.y;	
	}

	public float GetYPosition()
	{
		return yPosition;
	}
}
