using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffTimeManager : MonoBehaviour {

	public static OffTimeManager Instance;
	int maxSecondsEarning = 28800;
	double currencyOff = 0;
	void Awake()
	{
		Instance = this;
	}
		
	public void LoadCurrencyGainedOnOffTime (double totalSecondsPassed)
	{
		//Debug.Log ("totalSecondsPassed: " + totalSecondsPassed);
		double currencyOff = (double) totalSecondsPassed * GameManager.Instance.currencyPerSecond;
		if (maxSecondsEarning * GameManager.Instance.currencyPerSecond < currencyOff) {
			currencyOff = maxSecondsEarning * GameManager.Instance.currencyPerSecond;
		} 
	}
	public double GetMaxEarning()
	{
		double temp = maxSecondsEarning * GameManager.Instance.currencyPerSecond;
		return temp;
	}
	public int GetMaxSeconds()
	{
		return maxSecondsEarning;
	}
	public void ApplyCurrencyGainedOffTime(bool rewardVideoSeen)
	{
		if (rewardVideoSeen) {
			currencyOff = currencyOff * 2;
		}
		GameManager.Instance.currency += currencyOff;
		currencyOff = 0;
		GameManager.Instance.persistance.SaveData ();
	}
	public void LoadLandsAndAnimalsFromPersistance()
	{
		int eggys = (int) GameManager.Instance.timeManager.seconds.TotalSeconds / 10;
		GameManager.Instance.UpdateUI ();
		foreach (Land land in Grid.Instance.lands) {
			if (GameManager.Instance.persistance.landInts [land.id] == 1)
			{
				land.gameObject.SetActive (true);
				land.animating = false;

				if (GameManager.Instance.persistance.animalHiddenInts [land.id] >= 0) {
					land.animal = AnimalPoolManager.Instance.unchangingAnimals [land.id];

					AnimalPoolManager.Instance.UpdateDinos (land.animal);

					land.animal.transform.position = land.transform.position;
					land.animal.level = GameManager.Instance.persistance.animalInts [land.id];
					land.animal.hiddenLevel = GameManager.Instance.persistance.animalHiddenInts [land.id];

					land.animal.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [41];
				} else if (GameManager.Instance.persistance.animalInts [land.id] >= 0) {
				
					land.animal = AnimalPoolManager.Instance.unchangingAnimals [land.id];
					AnimalPoolManager.Instance.UpdateDinos (land.animal);
					land.animal.transform.position = land.transform.position;
					land.animal.level = GameManager.Instance.persistance.animalInts [land.id];

					land.animal.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [land.animal.level];	
				} else if (GameManager.Instance.persistance.animalInts [land.id] == -2) {
					land.animal = AnimalPoolManager.Instance.unchangingAnimals [land.id];
					AnimalPoolManager.Instance.UpdateDinos (land.animal);
					land.animal.transform.position = land.transform.position;
					land.animal.level = 0;
					land.animal.mysteryEgg = true;
					land.animal.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [43];	
				}
				else 
				{
					if (eggys > 0) {
						eggys--;

						land.animal = AnimalPoolManager.Instance.unchangingAnimals [land.id];

						AnimalPoolManager.Instance.UpdateDinos (land.animal);
						//Debug.Log ("land.animal.transform.position;" + land.animal.transform.position);
						land.animal.transform.position = land.transform.position;
						land.animal.level = GameManager.Instance.persistance.animalInts [land.id];
						if (land.animal.level >= 0) {
							land.animal.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [land.animal.level];
						}
						land.animal.level = 0;
						GameManager.Instance.persistance.animalInts[land.id] = 0;
					}
				}
			}
		}
		GameManager.Instance.timeManager.ResetSeconds ();
		AnimalPoolManager.Instance.startLoadingButton = true;
	}
}
