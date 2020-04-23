using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GameAnalyticsSDK;
using DemiumGames.AdMobManager;
using GoogleMobileAds.Api;

public class GameManager : MonoBehaviour {
	public bool tutorial;
	public bool tutorialWaiting;
	public int tutorialSteps = 0;
	public GameObject handTutorial;
	public bool visitingShop = false;
	public GameObject storeGoToButton;
	public int eggTaps = 0;
	public int boxesPerSession = 0;
	public int mergesPerSession = 0;

	public static GameManager Instance = null;
	public Persistance persistance; //Don't Assign in editor
	public TextManager textManager;
	public TimeManager timeManager;

	public bool holdingObject = false;
	public int numberOfLands;

	public int playerLevel;
	public int experience;
	public double currency;
	public double currencyPerSecond;

	public float startingNeededExperience; //always start here than use percentageExperience to expand 
	public float percentageExperience = 0.25f; //Experience needed to grow
	public float[] experiencePerLevel;
	public double[] goldPerAnimalLevel;


	public GameObject experienceBar;
	public GameObject eggButtonBar;

	public GameObject totalGold;
	public GameObject goldPerSecond;
	public GameObject experienceObject;
	public GameObject playerLevelObject;
	public TextMeshProUGUI goldPerSecondText;
	public TextMeshProUGUI totalGoldText;
	public TextMeshProUGUI experienceText;
	public TextMeshProUGUI playerLevelObjectText;

	public double timeForCurrency;
	public int nextLandToUnlock;
	public int SECONDSFORBOX;
	public int currentSecondsLeftForBox;
	public int maxLevelForAnimals;
	public int eggsNeeded;
	//TouchStuff
	public Land landRecipient = null;
	public Land targetLandRecipent = null;
	public Animal animalRecipient = null;

    public Sprite[] sprites;

	public double moneyTemp = 0;

	public int maxAvailableDinosInStore;

    private Sfx sfx;
    private GameObject hand;

	public bool shopTutorial = false;

	public double offTimeCurrencyToDouble;

    public int timeLeft = 10;
	void Awake()
	{
		Instance = this;

	}
	void Start()
	{
		Collect ();
        sfx = GameObject.Find("Sfx").GetComponent<Sfx>();
        hand = GameObject.Find("Hand");
        StartCoroutine(PutHand());

        LoadNeededExperiences ();
		totalGoldText = totalGold.GetComponent <TextMeshProUGUI> ();
		goldPerSecondText = goldPerSecond.GetComponent <TextMeshProUGUI> ();
		experienceText = experienceObject.GetComponent<TextMeshProUGUI> ();
		playerLevelObjectText = playerLevelObject.GetComponent<TextMeshProUGUI> ();
		if (GameManager.Instance.tutorial == false) {
			handTutorial.SetActive (false);
		}

		AdMobManager.Instance.SetOnBannerClicked(() => {
			GameAnalytics.NewDesignEvent ("Banner Clicked",1);
		});
		AdMobManager.Instance.SetOnBannerLoaded(() => {
			AdMobManager.Instance.ShowBanner();
			GameAnalytics.NewDesignEvent ("Banner Loaded",1);
		});
		AdMobManager.Instance.SetOnBannerFailedToLoad (() => {
			if(Application.internetReachability != NetworkReachability.NotReachable)
			{
				GameAnalytics.NewDesignEvent ("Banner Failed To Load With Internet",1);
			}
		});
		AdMobManager.Instance.LoadBanner (AdSize.Banner, AdPosition.Top);
    }
	public void HideHand()
	{
		handTutorial.SetActive (false);
	}
	void Update()
	{
		AnimalPoolManager.Instance.GetHowManyLandsAvailable ();
        //Debug.Log(timeLeft);
		if (shopTutorial == false) {
			if (timeLeft <= 0) {
				if (tutorial == false && numberOfLands > 1) {
					hand.SetActive (true);
					hand.transform.position = eggButtonBar.transform.GetChild (0).position;
				}
			} 
			if (numberOfLands == 0 && tutorial == false) {
				hand.SetActive (false);
			}
		} 
		else 
		{
			hand.SetActive (false);
		}

		Show ();
		Collect ();
	}
		
	public void UpdateUI()
	{
		experienceBar.GetComponent<Image> ().fillAmount = ((float) experience / experiencePerLevel[playerLevel -1]);
		experienceText.text = "" + experience + "/" + (int)experiencePerLevel [playerLevel - 1];

		moneyTemp = 0;

		foreach (Land land in Grid.Instance.lands) {
			if (land.animal != null && land.animal.level > 0) {
				moneyTemp += land.animal.CollectMoney ();
			}
		}
		playerLevelObjectText.text = ""+ playerLevel;
		goldPerSecondText.text = textManager.FormatTotalCurrency (currencyPerSecond);
		totalGoldText.text = textManager.FormatTotalCurrency (currency);
	}

	public void SetTimeForCurrency()
	{
		timeForCurrency = GameManager.Instance.timeManager.timeForCurrencySeconds.TotalSeconds;// + 1;
	}
	public void Collect()
	{
		if (GameManager.Instance.timeManager.timeForCurrencySeconds.TotalSeconds > timeForCurrency + 1) {
			//Debug.Log ("GameManager.Instance.timeManager.timeForCurrencySeconds.TotalSeconds: " + GameManager.Instance.timeManager.timeForCurrencySeconds.TotalSeconds );
			SetTimeForCurrency ();
			moneyTemp = 0;

			foreach (Land land in Grid.Instance.lands) {
				if (land.animal != null && land.animal.level > 0) {
					moneyTemp += land.animal.CollectMoney ();
					//Debug.Log ("Collecting");
				}
				goldPerSecondText.text = textManager.FormatTotalCurrency (moneyTemp);
				currencyPerSecond = moneyTemp;
				totalGoldText.text = textManager.FormatTotalCurrency (currency);
			}
		}
	}

	public void Show()
	{
		if (holdingObject && animalRecipient != null) {
			
			foreach (Land land in Grid.Instance.lands) {
				if (land.gameObject.activeSelf && land.animal != null) {
					if (land.animal.level == animalRecipient.level) {
						if (animalRecipient != land.animal) {
							land.transform.GetChild (0).gameObject.SetActive (true);
						}
					}
				}
			}
		}
		else {
			foreach (Land land in Grid.Instance.lands) {
				if (land.transform.GetChild (0).gameObject.activeSelf) {
					land.transform.GetChild (0).gameObject.SetActive (false);
				}
			}
		}
	}
	public bool CheckIfBox(Animal box)
	{
		if (box.level == 0) {
			box.UpgradeLevel (landRecipient, landRecipient);
			landRecipient = null;
			targetLandRecipent = null;
			animalRecipient = null;

			return true;
		}
		return false;
	}

	public void SetAnimal(Animal animaly)
	{
		if (animaly.level == 0) {
			OpenBox (animaly);
			GameManager.Instance.persistance.animalInts [targetLandRecipent.id] = 1;
		}
		else if (targetLandRecipent == landRecipient) {
			animaly.transform.position = landRecipient.transform.position;
		}
		else if (targetLandRecipent.animal == null) {

			int temp = Instance.persistance.animalInts [landRecipient.id];
			GameManager.Instance.persistance.animalInts [landRecipient.id] = -1;
			GameManager.Instance.persistance.animalInts [targetLandRecipent.id] = temp;


			PlaceOnEmptyLand (animaly);
		} 
		else if (targetLandRecipent.animal.level == animaly.level) {
			MergeAnimals (animaly);
		}
		else
		{
			SwitchAnimals(animaly);
		}
		GameManager.Instance.persistance.SaveData ();
		goldPerSecondText.text = textManager.FormatTotalCurrency (moneyTemp);
		currencyPerSecond = moneyTemp;

		targetLandRecipent = null;
		animalRecipient = null;
		landRecipient = null;
	}
	public void OpenBox(Animal animaly)
	{
        sfx.openEggPlay();
        ParticleSystem boxy = ParticlePooler.SharedInstance.GetPooledObject ("BoxOpen").GetComponent<ParticleSystem>();
		ParticleSystem boxy2 = ParticlePooler.SharedInstance.GetPooledObject ("FireFlyUp").GetComponent<ParticleSystem>();

		boxy.transform.position = animaly.transform.position;
		boxy2.transform.position = animaly.transform.position;
		boxy.gameObject.SetActive(true);
		boxy2.gameObject.SetActive(true);
		boxy.Play ();
		boxy2.Play ();

		//boxy.
		//Effects!
	}
	public void PlaceOnEmptyLand(Animal animaly)
	{
		targetLandRecipent.animal = animaly;
		animaly.transform.position = targetLandRecipent.transform.position;
		landRecipient.animal = null;
	}

	public void SwitchAnimals(Animal animaly)
	{
		if (targetLandRecipent.animating == true) {
			animaly.transform.position = landRecipient.transform.position;
		} else {

			Animal animalTemp = targetLandRecipent.animal;
			targetLandRecipent.animal = animaly;
			animaly.transform.position = targetLandRecipent.transform.position;
			landRecipient.animal = animalTemp;
			animalTemp.transform.position = landRecipient.transform.position;


			int tempy = GameManager.Instance.persistance.animalInts [targetLandRecipent.id];
			int tempy2 = GameManager.Instance.persistance.animalInts [landRecipient.id];
			int tempy3 = GameManager.Instance.persistance.animalHiddenInts [targetLandRecipent.id];
			int tempy4 = GameManager.Instance.persistance.animalHiddenInts [landRecipient.id];

			GameManager.Instance.persistance.animalInts [targetLandRecipent.id] = tempy2;
			GameManager.Instance.persistance.animalInts [landRecipient.id] = tempy;
			GameManager.Instance.persistance.animalHiddenInts [targetLandRecipent.id] = tempy4;
			GameManager.Instance.persistance.animalHiddenInts [landRecipient.id] = tempy3;
		}
	}

	public void AddExperience()
	{
		if (experience >= experiencePerLevel [playerLevel - 1]) {
			if (playerLevel < 35) {

				experience = experience - (int)experiencePerLevel [playerLevel - 1];

				playerLevel++;

				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "PlayeLevel " + GameManager.Instance.playerLevel);

				playerLevelObjectText.text = "" + playerLevel;

				Grid.Instance.AppearLand ();
			}
		} else if (experience >= 40 && playerLevel == 1) {
			playerLevel++;
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "PlayeLevel " + GameManager.Instance.playerLevel);


			playerLevelObjectText.text = "" + playerLevel;
			Grid.Instance.AppearLand ();

			//Passed Level One
		}
		experienceBar.GetComponent<Image> ().fillAmount = ((float)experience / experiencePerLevel [playerLevel - 1]);
	}

	public void MergeAnimals(Animal animaly)
	{
		if (mergesPerSession == 0 && GameManager.Instance.tutorial) {
			GameManager.Instance.tutorialSteps++;
			handTutorial.GetComponent<HandAnimation> ().UpdateHand ();
		}
			
		Animal animule = animaly;
		if (animule.level != maxLevelForAnimals) {

			if (animule.level < 40) {
				experience += animule.level;

				mergesPerSession++;

				sfx.MezclarPlay ();


				ParticleSystem part1 = ParticlePooler.SharedInstance.GetPooledObject ("FireFlyExpand").GetComponent<ParticleSystem> ();
				ParticleSystem part2 = ParticlePooler.SharedInstance.GetPooledObject ("PathParticle").GetComponent<ParticleSystem> ();
				ParticleSystem part3 = ParticlePooler.SharedInstance.GetPooledObject ("ParticleGodRay").GetComponent<ParticleSystem> ();


				part2.GetComponent<ParticleToUI> ().point = GameObject.FindGameObjectWithTag ("StarCentre");
				part2.GetComponent<ParticleToUI> ().LoadStuff ();
				part1.transform.position = targetLandRecipent.transform.position;
				part2.transform.position = targetLandRecipent.transform.position;
				part3.transform.position = targetLandRecipent.transform.position;

				part1.gameObject.SetActive (true);
				part2.gameObject.SetActive (true);
				part3.gameObject.SetActive (true);
				part1.Play ();
				part2.Play ();
				part3.Play ();
			

				AddExperience ();
			}
			targetLandRecipent.animal.UpgradeLevel (targetLandRecipent, landRecipient);
			AnimalPoolManager.Instance.AddAnimalToAnimals (animule);
			animaly.transform.position = new Vector2 (0, -6);
			landRecipient.animal = null;
		} 
		else {
			//Do switch if max level
			SwitchAnimals(animaly);
		}
		experienceText.text = "" + experience + "/" + (int)experiencePerLevel [playerLevel - 1];
	}

	public void ColorBoxButton (int currentSecondsLeftForBox)
	{
		eggButtonBar.GetComponent<Image> ().fillAmount = ((float)GameManager.Instance.timeManager.seconds.TotalSeconds + ((float)currentSecondsLeftForBox)) / SECONDSFORBOX;
	}
	public void LoadNeededExperiences()
	{
		experiencePerLevel [0] = 40;
		for (int i = 1; i < maxLevelForAnimals - 5; i++) {
			experiencePerLevel[i] += experiencePerLevel[i-1] + Mathf.Round(experiencePerLevel[i-1] * 0.25f);
		}
	}
	public void LoadTimePersistance()
	{
		persistance.LoadTimeStuff ();
	}

	public void PressButtonToSpeedUpSpawn()
	{
		if(tutorial && tutorialSteps == 3)
		{
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Tutorial Step " + GameManager.Instance.tutorialSteps );

			tutorial = false;
			//storeGoToButton.SetActive (true);
			tutorialWaiting = false;
			tutorialSteps = 4;
			handTutorial.SetActive (false);
		}
		eggTaps++;
		currentSecondsLeftForBox += 1;
		if (tutorial == false) {
			
			timeLeft = 5;
			hand.SetActive (false);
		}
        if (currentSecondsLeftForBox > SECONDSFORBOX) {
				currentSecondsLeftForBox = SECONDSFORBOX;
		}
	}

  
    public IEnumerator PutHand()
    {

        while (true)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
        } 
        
    }
}

[System.Serializable]
public class TouchStuff{
	public GameObject landRecipient = null;
	public GameObject targetLandRecipent = null;
	public GameObject animalRecipient = null;
}