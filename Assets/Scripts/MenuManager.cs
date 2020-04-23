using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DemiumGames.AdMobManager;
using GameAnalyticsSDK;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour {

	public static MenuManager Instance = null;

	public GameObject storePanel;
	public GameObject storeGoToButton;
	public GameObject darknessForStore;

	public GameObject popupNoLand;
	public GameObject pupupMysteryEgg;
	public GameObject popupNoVideos;

	public bool interAdLoaded = false;
	public float addTimeToPass;
	float nextTime;
	int videosWatchedPerSession;
	bool timeToShowInter = false;

	void Start () {
		Instance = this;
		nextTime = Time.time + addTimeToPass;

		AdMobManager.Instance.SetOnRewardLoaded (() => {
			Debug.Log("Video Loaded Correctly");
			//PopupManager.Instance.upgradeButtonMystery.SetActive(true);
			//PopupManager.Instance.upgradeMaxMystery.SetActive(true);
			PopupManager.Instance.RefreshLoadingOrUpgradeButton();
		});

		AdMobManager.Instance.SetOnRewardFailedToLoad(() => {
			//videoLoaded = false;
			//StartCoroutine(LoadingVideo());
			Debug.Log ("Checking video connectivity");
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				GameAnalytics.NewDesignEvent ("Video Load Failed due to Internet Connection",1);
			}
			else
			{
				GameAnalytics.NewDesignEvent ("Video Load Failed due to other reasons",1);
			}
		});
			
		AdMobManager.Instance.SetOnInterLoaded (() => {

			GameAnalytics.NewDesignEvent("Loaded Inter");
			interAdLoaded = true;
		});

		AdMobManager.Instance.SetOnInterFailedToLoad (() => {
			if(Application.internetReachability != NetworkReachability.NotReachable)
			{
				GameAnalytics.NewDesignEvent ("Inter Failed due to Internet Connection",1);
			}
		});
		AdMobManager.Instance.SetOnInterClicked(() => {
			GameAnalytics.NewDesignEvent ("Inter Clicked",1);
		});
		AdMobManager.Instance.SetOnInterReturn (() => {
			GameAnalytics.NewDesignEvent ("Inter Return",1);
		});
		AdMobManager.Instance.LoadInter ();
		StartCoroutine(LoadingVideo());

	}
	void Update()
	{
		if (nextTime < Time.time + addTimeToPass) {
			nextTime = Time.time + addTimeToPass;
			timeToShowInter = true;
			//if (AdMobManager.Instance.isInterLoaded () == false) {
				//AdMobManager.Instance.LoadInter ();

			//}
		}
	}
	public void GoToStore()
	{
		PopupManager.Instance.RefreshLoadingOrUpgradeButton();
		ShopManager.Instance.timesEnteredInStore++;
		GameManager.Instance.visitingShop = true;
		storePanel.SetActive (true);
		storeGoToButton.SetActive (false);
		darknessForStore.SetActive (true);

		if (GameManager.Instance.shopTutorial == false) {
			if (AdMobManager.Instance.isInterLoaded () == true && timeToShowInter == true) {
				AdMobManager.Instance.ShowInter ();
				timeToShowInter = false;
			} 
		}
		else if (AdMobManager.Instance.isInterLoaded () == false) 
		{
			AdMobManager.Instance.LoadInter ();
		}
	}
	public void ExitStore()
	{
		if (GameManager.Instance.shopTutorial == false) 
		{
			GameManager.Instance.visitingShop = false;
			storePanel.SetActive (false);
			storeGoToButton.SetActive (true);
			darknessForStore.SetActive (false);

			if (AdMobManager.Instance.isInterLoaded () == false) {
				AdMobManager.Instance.LoadInter ();
			}
			StartCoroutine (LoadingVideo ());
		}
	}
		
	public void ShowPopUpNoLand()
	{
		popupNoLand.SetActive(true);
	}
	public void HidePopUpNoLand()
	{
		popupNoLand.SetActive(false);
	}
	public void ShowPopupNoVideos()
	{
		popupNoVideos.SetActive (true);
	}
	public void HidePopupNoVideos()
	{
		popupNoVideos.SetActive (false);
	}
	public void ShowPopUpMysteryEgg()
	{
		pupupMysteryEgg.transform.GetChild (7).GetComponent<Image> ().sprite = GameManager.Instance.sprites [PopupManager.Instance.idOfNewDino];
		pupupMysteryEgg.transform.GetChild (8).GetComponent<TextMeshProUGUI> ().text = "" + ShopManager.Instance.priceList.dinoNames [PopupManager.Instance.idOfNewDino - 1];

		pupupMysteryEgg.SetActive (true);
		TouchInput.Instance.SetTouchActivated (false);
		PopupManager.Instance.RefreshLoadingOrUpgradeButton();
	}
	public void HidePopUpMysteryEgg()
	{
		pupupMysteryEgg.SetActive(false);
		PopupManager.Instance.GenerateDinoGifted ();
		PopupManager.Instance.ResetIDS ();
		TouchInput.Instance.SetTouchActivated (true);
	}

	public void LoadVideo()
	{
		Debug.Log ("Video Not Loaded");
		if (AdMobManager.Instance.IsRewardedVideLoaded () == false) {
			Debug.Log ("AdMobManager.Instance.IsRewardedVideoLoaded: " + AdMobManager.Instance.IsRewardedVideLoaded());
			AdMobManager.Instance.LoadVideo ();

		}
	}
	public void ShowVideo()
	{
		Debug.Log ("Video Show Video");
		Debug.Log ("AdMobManager.Instance.IsRewardedVideoLoaded: " + AdMobManager.Instance.IsRewardedVideLoaded());
		if(AdMobManager.Instance.IsRewardedVideLoaded() == true)
		{
			AdMobManager.Instance.ShowVideoAd (RewardFailed,RewardSuccess);
			//PopupManager.Instance.upgradeButtonMystery.SetActive (false);
			//PopupManager.Instance.upgradeMaxMystery.SetActive (false);
		}
		else
		{
			ShowPopupNoVideos ();
		}
	}

	private void RewardSuccess()
	{
		Debug.Log ("Reward Success");
		videosWatchedPerSession++;
		GameAnalytics.NewDesignEvent ("Video Reward Successful",1);
		PopupManager.Instance.FormDinoGifted ();
		//PopupManager.Instance.ResetIDS();
		/*
		if (GameManager.Instance.videosEverWatched >= 50) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "50 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 40) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "40 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 30) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "30 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 20) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "20 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 15) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "15 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 10) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "10 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 5) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "5 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 4) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "4 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 3) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "3 Videos Watched");
		} else if (GameManager.Instance.videosEverWatched >= 2) {
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "2 Videos Watched");
		} 
		else 
		{
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "1 Video Watched");

		}*/
		StartCoroutine(LoadingVideo());
		PopupManager.Instance.RefreshLoadingOrUpgradeButton();
	}
		
	private void RewardFailed(){
		//LoadRewardedVideo ();
		//StartCoroutine(LoadingVideo());
		Debug.Log ("Reward Failed"); 
		GameAnalytics.NewDesignEvent ("Video Reward Failed",1);
		StartCoroutine(LoadingVideo());
		PopupManager.Instance.RefreshLoadingOrUpgradeButton();
	}


	IEnumerator LoadingVideo()
	{
		yield return new WaitForSeconds(1);
		AdMobManager.Instance.LoadVideo ();
	}
}
