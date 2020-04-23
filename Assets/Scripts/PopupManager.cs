using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DemiumGames.AdMobManager;
using UnityEngine.UI;
using TMPro;

public class PopupManager : MonoBehaviour {

	public Animal animalBeingFormed;
	public Land landBeingUsed;

	public int idOfNewDino;

	public GameObject upgradeButtonMystery;
	public GameObject upgradeButtonMysteryText;
	public GameObject upgradeMaxMystery;

	public Sprite[] buttonSprites;
	public static PopupManager Instance;

	void Awake () {
		Instance = this;		
	}
	void Start()
	{
		ResetIDS ();
	}

	public void SetLand (Land newLand)
	{
		landBeingUsed = newLand;
	}
	public void SetAnimal(Animal animalBeingFormed)
	{
		this.animalBeingFormed = animalBeingFormed;
	}
	public void FormDinoGifted()
	{
		Debug.Log ("STEP 1: FormDinoGifted");
		RefreshLoadingOrUpgradeButton ();
		idOfNewDino = Random.Range (idOfNewDino, GameManager.Instance.maxAvailableDinosInStore +1);

		if (idOfNewDino != GameManager.Instance.maxAvailableDinosInStore + 1) {
			idOfNewDino++;
		}
		RefreshLoadingOrUpgradeButton();
		if (idOfNewDino == GameManager.Instance.maxAvailableDinosInStore + 1) {
			upgradeButtonMystery.SetActive (false);
		}
		Debug.Log ("idOfNewDino == GameManager.Instance.maxAvailableDinosInStore + 1: " + idOfNewDino + " - " + (GameManager.Instance.maxAvailableDinosInStore + 1));

		animalBeingFormed.level = idOfNewDino;
		GameManager.Instance.persistance.animalInts [landBeingUsed.id] = idOfNewDino;
		GameManager.Instance.experience += ShopManager.Instance.priceList.experienceForBuying[animalBeingFormed.level-1];
		GameManager.Instance.UpdateUI ();
		animalBeingFormed.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [idOfNewDino];
		MenuManager.Instance.ShowPopUpMysteryEgg ();
		//upgradeButtonMystery.SetActive (false);

		/*
		if (idOfNewDino == tempIdOfNewDino) //Only happends if level is lower than 2 or 3 - available in store
		{
			
			if (idOfNewDino != GameManager.Instance.maxAvailableDinosInStore + 1) {
				idOfNewDino++;
			}
			if (idOfNewDino == GameManager.Instance.maxAvailableDinosInStore + 1) {
				upgradeButtonMystery.SetActive (false);
				RefreshLoadingOrUpgradeButton();
			}
		} 
		else
		{
			idOfNewDino = tempIdOfNewDino;
		}
		animalBeingFormed.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [idOfNewDino];
		this.animalBeingFormed.level = idOfNewDino;
		MenuManager.Instance.ShowPopUpMysteryEgg ();*/
	}
	public void GenerateDinoGifted()
	{
		//animalBeingFormed.UpgradeLevel (GameManager.Instance.landRecipient,GameManager.Instance.landRecipient);
		animalBeingFormed.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [idOfNewDino];
		animalBeingFormed.giftEgg = false;
		animalBeingFormed.mysteryEgg = false;
		//upgradeButtonMystery.SetActive (true);
	}
	public void RefreshLoadingOrUpgradeButton()
	{
		if (idOfNewDino == GameManager.Instance.maxAvailableDinosInStore + 1) {
			upgradeButtonMystery.SetActive (false);
		}
		//upgradeButtonMystery.SetActive (true);
		else if (AdMobManager.Instance.IsRewardedVideLoaded ()) {
			upgradeButtonMystery.GetComponent<Image> ().sprite = buttonSprites [0]; //green
			upgradeButtonMysteryText.GetComponent<TextMeshProUGUI>().text = "Upgrade!";
			upgradeButtonMystery.SetActive (true);
		} 
		else 
		{
			upgradeButtonMystery.GetComponent<Image> ().sprite = buttonSprites [1]; //gray
			upgradeButtonMysteryText.GetComponent<TextMeshProUGUI>().text = "Loading...";
			upgradeButtonMystery.SetActive (true);
		}
	}
	public void ResetIDS()
	{
		idOfNewDino = 2;
	}
}
