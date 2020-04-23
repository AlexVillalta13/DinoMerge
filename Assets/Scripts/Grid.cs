using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public static Grid Instance;
	public Land landToPlaceAt;
	public List<Land> lands;
	public GameObject part4;
	void Start()
	{
		Instance = this;
	}
		
	public void AppearLand()
	{
		foreach(Land land in lands)
		{
			if(land.id == GameManager.Instance.nextLandToUnlock)
			{
				GameManager.Instance.persistance.landInts[land.id] = 1;
				land.gameObject.SetActive(true);
			}
		}
		GameManager.Instance.nextLandToUnlock++;
		GameManager.Instance.persistance.SaveData ();
	}
}
