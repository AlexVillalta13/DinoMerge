using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour {

	public int id;
	public Animal animal;
	public bool animating = true;

	public Animal GetAnimal()
	{
		PopupManager.Instance.landBeingUsed = this;
		if (animal != null) {
			return animal;	
		} else {
			return null;
		}
	}

}
