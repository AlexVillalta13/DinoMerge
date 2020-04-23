using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class ShopTutorial : MonoBehaviour {

	public GameObject shopButtonHand;
	public GameObject shopItemHand;
	public GameObject shopButton;
	public GameObject darkBackgroundTutorial;

	public static ShopTutorial Instance;

	void Start()
	{
		Instance = this;
	}
	public void StoreButtonPressed()
	{
		if (GameManager.Instance.shopTutorial) {
			HideShopButtonHand ();
		}
	}
	public void StoreItemPressed()
	{
		EndTutorial ();
	}

	public void StartShopTutorial()
	{
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Shop Tutorial Started");
		if (GameManager.Instance.persistance.shopTutorialSeen == false) {
			GameManager.Instance.shopTutorial = true;
			TouchInput.Instance.SetTouchActivated (false);
			shopButtonHand.SetActive (true);
			shopItemHand.SetActive (true);

			darkBackgroundTutorial.SetActive (true);
		}
		shopButton.SetActive (true);
	}
	public void HideShopItemHand()
	{
		shopItemHand.SetActive (false);
	}
	public void HideShopButton()
	{
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Shop Button Pressed");
		shopButton.SetActive (false);
	}
	public void ShowShopButton()
	{
		shopButton.SetActive (true);
	}

	public void HideShopButtonHand ()
	{
		shopButtonHand.SetActive (false);
		darkBackgroundTutorial.SetActive (false);
	}
	public void EndTutorial()
	{
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Shop Tutorial Completed");
		if (GameManager.Instance.shopTutorial) 
		{
			GameManager.Instance.shopTutorial = false;
		}
		GameManager.Instance.persistance.shopTutorialSeen = true;
		HideShopButtonHand ();
		HideShopItemHand ();
		TouchInput.Instance.SetTouchActivated (true);

		GameManager.Instance.persistance.SaveData ();
	}
}


