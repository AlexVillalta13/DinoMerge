using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DemiumGames.Saveable;

public class Persistance : MonoBehaviour {

	public bool tutorialSeen = false;
	public bool shopTutorialSeen = false;

	public int[] landInts;// = {1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	public int[] animalInts;// = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	public int[] animalHiddenInts;
	public int[] animalMysteryInts;
	public int[] animalGiftInts;
	public int[] animalRewardInts;

	public int maxAnimalUnlocked;
	bool hey = false;

	void Start()
	{

		if(!File.Exists(Application.persistentDataPath + "/playerInfo.json"))
		{

		}
		else
		{
			try {
				LoadDataTutorial();
				//GameManager.Instance.timeManager.TimeManagerLoad();
				LoadData();

			} catch (Exception e){
				Debug.LogError(e.Message); 
			};
		}
		OffTimeManager.Instance.LoadLandsAndAnimalsFromPersistance ();
		OffTimeManager.Instance.LoadCurrencyGainedOnOffTime (GameManager.Instance.timeManager.seconds.TotalSeconds);

	}
	public void LoadTimeStuff ()
	{
		try{
			PlayerData data = JsonFormatterHelper.Load<PlayerData> (Application.persistentDataPath, "playerInfo.json", false); //false means no security
			GameManager.Instance.timeManager.oldDate = DateTime.FromBinary (data.dateBinary);
		}
		catch(Exception e)
		{
			//GameManager.Instance.timeManager.oldDate = DateTime.UtcNow;
		}
	}
	public void LoadDataTutorial()
	{
		if(File.Exists(Application.persistentDataPath + "/playerInfo.json"))
		{
			PlayerData data = JsonFormatterHelper.Load<PlayerData> (Application.persistentDataPath, "playerInfo.json", false); 
			GameManager.Instance.tutorial = data.tutorial;
			shopTutorialSeen = data.shopTutorial;
		}
	}

	public void LoadData()
	{
		if (GameManager.Instance.tutorial == false) {
			PlayerData data = JsonFormatterHelper.Load<PlayerData> (Application.persistentDataPath, "playerInfo.json", false); 

			landInts = data.landInts;
			animalInts = data.animalInts;

			GameManager.Instance.playerLevel = data.playerLevel;
			maxAnimalUnlocked = data.maxAnimalUnlocked;
			GameManager.Instance.currency = data.currency;

			GameManager.Instance.experience = data.experience;
			GameManager.Instance.playerLevel = data.playerLevel;
			GameManager.Instance.nextLandToUnlock = data.nextLandToUnlock;
			GameManager.Instance.tutorial = data.tutorial;


			try{
				animalHiddenInts = data.animalHiddenLevels;
				GameManager.Instance.currencyPerSecond = data.currencyPerSecond;
				if(data.priceListGold.Length > 3)
				{
					ShopManager.Instance.priceList.gold = data.priceListGold;
					ShopManager.Instance.LoadPriceListGoldFromPersistance (data.priceListGold);
				}
			}
			catch(Exception e) 
			{
				File.Delete (Application.persistentDataPath + "/playerInfo.json");
			}
			try{
				animalMysteryInts = data.animalMysteryInts;
				animalGiftInts = data.animalGiftInts;
				animalRewardInts = data.animalRewardInts;
				shopTutorialSeen = data.shopTutorial;
			}
			catch(Exception e) {
				File.Delete (Application.persistentDataPath + "/playerInfo.json");
			}

			if (GameManager.Instance.persistance.maxAnimalUnlocked > 5) {
				ShopTutorial.Instance.ShowShopButton ();
				Debug.Log ("Max level: " + GameManager.Instance.persistance.maxAnimalUnlocked);
			} 
			else if(GameManager.Instance.persistance.maxAnimalUnlocked == 5)
			{
				ShopTutorial.Instance.StartShopTutorial ();
			}
			else 
			{
				Debug.Log ("Max level: " +  GameManager.Instance.persistance.maxAnimalUnlocked);
			}
		}
	}
	public void SaveData()
	{
		PlayerData data = new PlayerData ();

		data.dateBinary = GameManager.Instance.timeManager.oldDate.ToBinary ();
		data.landInts = landInts;
		data.animalInts = animalInts;
		data.animalHiddenLevels = animalHiddenInts;

		data.maxAnimalUnlocked = maxAnimalUnlocked;
		data.currency = GameManager.Instance.currency;
		data.currencyPerSecond = GameManager.Instance.currencyPerSecond;
		data.experience = GameManager.Instance.experience;
		data.playerLevel = GameManager.Instance.playerLevel;
		data.nextLandToUnlock = GameManager.Instance.nextLandToUnlock;
		data.tutorial = GameManager.Instance.tutorial;
		data.shopTutorial = shopTutorialSeen;
		data.animalMysteryInts = animalMysteryInts;
		data.animalGiftInts = animalGiftInts;
		data.animalRewardInts = animalRewardInts;

		Debug.Log ("Saving");

		data.priceListGold = ShopManager.Instance.priceList.gold;

		long aux = DateTime.Now.ToBinary ();
		DateTime.FromBinary (aux);

		JsonFormatterHelper.Save (data, Application.persistentDataPath, "playerInfo.json", false);
	}
}

[Serializable]
class PlayerData
{
	public long dateBinary;

	public int[] animalInts = {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
	public int[] landInts = { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
	public int[] animalHiddenLevels = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};

	public int[] animalMysteryInts = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
	public int[] animalGiftInts = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
	public int[] animalRewardInts = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};

	public double[] priceListGold;
	public int maxAnimalUnlocked = 0;
	public double currency = 0;
	public int experience = 0;
	public int playerLevel = 1;
	public int nextLandToUnlock = 6;
	public bool tutorial = false;
	public double currencyPerSecond = 0;
	public bool shopTutorial = false;
}