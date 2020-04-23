using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameAnalyticsSDK;
using DemiumGames.AdMobManager;

public class ShopManager : MonoBehaviour {

	public static ShopManager Instance;
	public ScriptableObjectExample priceList;
	public GameObject[] priceTexts;
	public Button[] buttons;
	public Image[] dinoImages;
	public TextMeshProUGUI[] dinoTexts;

	public int timesEnteredInStore;
	public int timesBoughtAnything;

	public Sprite[] buttonSprites;

	public int[] storePositionsForContentStore;
	public RectTransform ContentStore;

	float nextTime;
	float timeToPass = 1;

	int availableDinosOffset = 3;

	bool interAdLoaded = false;

	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		UpdateStore ();
		nextTime = Time.time + timeToPass;


		AdMobManager.Instance.LoadInter ();

		AdMobManager.Instance.SetOnInterLoaded (() => {

			GameAnalytics.NewDesignEvent("Loaded Inter");
			interAdLoaded = true;
			//AdMobManager.Instance.ShowInter(); 
		});
	}
	void Update()
	{
		if (nextTime < Time.time) {
			nextTime = Time.time + timeToPass;
			UpdateStore ();
		}
	}
	public void LoadPriceListGoldFromPersistance(double[] prices)
	{
		priceList.gold = prices;
		UpdateStore ();
	}

	public void SetOpenPositionOfContentStore()
	{
		float yFloat = ContentStore.transform.GetChild (GameManager.Instance.maxAvailableDinosInStore).GetComponent<StoreContentPositioner> ().GetYPosition ();
		ContentStore.localPosition = new Vector2(ContentStore.localPosition.x , yFloat);
	}

	public void UpdateStore()
	{
		int divederNumber = 0;

		if (GameManager.Instance.persistance.maxAnimalUnlocked >= 10) {
			divederNumber = 4;
		} 
		else 
		{
			divederNumber = 3;
		}
		GameManager.Instance.maxAvailableDinosInStore = GameManager.Instance.persistance.maxAnimalUnlocked - (divederNumber + 1);
		for(int i = 1; i < priceTexts.Length; i++)
		{
			
			if (GameManager.Instance.persistance.maxAnimalUnlocked > i + divederNumber) 
			{
				buttons [i].GetComponent<Button> ().interactable = true;
				dinoImages [i].sprite = GameManager.Instance.sprites [i + 1];
				dinoTexts[i].text = priceList.dinoNames[i];
				priceTexts [i].GetComponent<TextMeshProUGUI> ().text = "" + TextManager.instance.FormatTotalCurrency(priceList.gold [i]);
			}
			else {
				buttons [i].GetComponent<Button> ().interactable = false;
				dinoImages [i].sprite = GameManager.Instance.sprites [42]; // ? eggsprite
				dinoTexts[i].text = "???";
				priceTexts [i].GetComponent<TextMeshProUGUI> ().text = "???";
			}

			if (GameManager.Instance.currency < priceList.gold [i]) {
				buttons [i].GetComponent<Image> ().sprite = buttonSprites [0];
			} 
			else 
			{
				buttons [i].GetComponent<Image> ().sprite = buttonSprites [1];
			}
		}
	}
	private bool CheckIfBuyable(int dinoToUpdate)
	{
		if (GameManager.Instance.currency > priceList.gold [dinoToUpdate] && GameManager.Instance.numberOfLands > 0) 
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public void BuyDino(int dinoToBuy)
	{
		bool canUpdate = CheckIfBuyable (dinoToBuy);

		if (canUpdate) {
			priceTexts [dinoToBuy].GetComponent<TextMeshProUGUI> ().text = "" + TextManager.instance.FormatTotalCurrency (priceList.gold [dinoToBuy]);
			GameManager.Instance.currency -= priceList.gold [dinoToBuy];
			priceList.gold [dinoToBuy] += (priceList.gold [dinoToBuy] - (double)((double)priceList.gold [dinoToBuy] * 0.85f));
			int landToPlaceAt = AnimalPoolManager.Instance.GetHowManyLandsAvailable ();
			SendAnalyticsBuyInformation (dinoToBuy);
			timesBoughtAnything++;
			if (landToPlaceAt > 0) {
				AnimalPoolManager.Instance.PlaceAnimalOnPlace (dinoToBuy + 1, AnimalPoolManager.Instance.AddAnimalsToRecover ());
				GameManager.Instance.boxesPerSession++;
			}
		}
		else
		{
			if (GameManager.Instance.numberOfLands == 0) {
				MenuManager.Instance.ShowPopUpNoLand ();
			}
		}
		UpdateStore ();
	}

	public void SendAnalyticsBuyInformation(int dinoBought)
	{
		if (GameManager.Instance.persistance.maxAnimalUnlocked > 35) {
			GameAnalytics.NewDesignEvent ("Dino: Range 36 to 40 bought", dinoBought);
		} 
		else if (GameManager.Instance.persistance.maxAnimalUnlocked > 30) {
			GameAnalytics.NewDesignEvent ("Dino: Range 31 to 35 bought", dinoBought);
		} 
		else if (GameManager.Instance.persistance.maxAnimalUnlocked > 25) {
			GameAnalytics.NewDesignEvent ("Dino: Range 26 to 30 bought", dinoBought);
		} 
		else if (GameManager.Instance.persistance.maxAnimalUnlocked > 20) {
			GameAnalytics.NewDesignEvent ("Dino: Range 21 to 25 bought", dinoBought);
		} 
		else if (GameManager.Instance.persistance.maxAnimalUnlocked > 15) {
			GameAnalytics.NewDesignEvent ("Dino: Range 16 to 20 bought", dinoBought);
		} 
		else if (GameManager.Instance.persistance.maxAnimalUnlocked > 10) {
			GameAnalytics.NewDesignEvent ("Dino: Range 11 to 15 bought", dinoBought);
		} 
		else if (GameManager.Instance.persistance.maxAnimalUnlocked > 5) {
			GameAnalytics.NewDesignEvent ("Dino: Range 6 to 10 bought", dinoBought);
		} 
		else 
		{
			GameAnalytics.NewDesignEvent ("Dino: Range 1 to 5 bought", dinoBought);
		}
	}
}
