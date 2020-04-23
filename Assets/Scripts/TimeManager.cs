using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameAnalyticsSDK;

public class TimeManager : MonoBehaviour {

	public DateTime currentDate;
	public DateTime oldDate;
	public DateTime oldDateForPausePersistance;
	public DateTime sessionStart;
	public DateTime sinceFocusOnDate;

	public TimeSpan seconds;
	public TimeSpan eternalSeconds;
	public TimeSpan timeForCurrencySeconds;

	bool loadSavedPersistance = false;
	bool timePaused = false;

	void Start()
	{
		LoadGameData ();
		Debug.Log ("START AGAIN");
		Debug.Log ("OLDDATE: " + oldDate);
	}
	public void LoadGameData()
	{
		sessionStart = sinceFocusOnDate = currentDate = DateTime.Now;
		GameManager.Instance.persistance.LoadDataTutorial();
		if (GameManager.Instance.tutorial == false) {
			GameManager.Instance.HideHand ();
			GameManager.Instance.LoadTimePersistance ();
			GetSecondsBetweenOldandCurrentDate ();
		}

		ResetSeconds ();
	}

	void Update ()
	{
		{
			GetSecondsBetweenOldandCurrentDate ();
		}
	}

	void GetSecondsBetweenOldandCurrentDate()
	{
		seconds = currentDate.Subtract(oldDate);
		eternalSeconds = currentDate.Subtract (sessionStart);
		currentDate = System.DateTime.Now;
		timeForCurrencySeconds = DateTime.Now.Subtract (sinceFocusOnDate);
	}
	public void ResetSeconds()
	{
		oldDate = currentDate;
	}
	void OnApplicationPause(bool isPaused)
	{
		Debug.Log ("OLD DATE: " + oldDate);
		Debug.Log ("OnApplicationPause");
		Debug.Log ("isPaused: " + isPaused);
		if (isPaused) {
			Debug.Log ("Application State: Pausing");
			oldDate = DateTime.Now;
			oldDateForPausePersistance = DateTime.Now;

			Debug.Log ("oldDateForPausePersistance Set");
			GameManager.Instance.persistance.SaveData ();

			GameAnalytics.NewDesignEvent ("Store entered times", ShopManager.Instance.timesEnteredInStore);
			GameAnalytics.NewDesignEvent ("Items bought per session" , ShopManager.Instance.timesBoughtAnything);
			ShopManager.Instance.timesEnteredInStore = 0;

			if (GameManager.Instance.eggTaps > 10) {
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "User Using Egg Button");
			}
			if (GameManager.Instance.persistance.maxAnimalUnlocked > 3) {
				GameAnalytics.NewDesignEvent ("Eggs received per Session", GameManager.Instance.boxesPerSession);
				GameAnalytics.NewDesignEvent ("Merges per Session", GameManager.Instance.mergesPerSession);
				GameManager.Instance.boxesPerSession = 0;
				GameManager.Instance.mergesPerSession = 0;
			}
		} else if (isPaused == false && GameManager.Instance.currency != 0) {
			Debug.Log ("Application State: Resuming");
			sinceFocusOnDate = DateTime.Now;

			currentDate = DateTime.Now;
			oldDate = DateTime.Now;
			TimeSpan totalPassedSecondsApplicationPaused = currentDate.Subtract (oldDateForPausePersistance);
			Debug.Log ("totalPassedSecondsApplicationPaused.TotalSeconds: " + totalPassedSecondsApplicationPaused.TotalSeconds);

			OffTimeManager.Instance.LoadCurrencyGainedOnOffTime (totalPassedSecondsApplicationPaused.TotalSeconds);
			GetSecondsBetweenOldandCurrentDate ();

			GameManager.Instance.timeForCurrency = 0;
			GameManager.Instance.persistance.SaveData ();
		} 
		else 
		{
			GameManager.Instance.persistance.LoadTimeStuff ();
			GameManager.Instance.persistance.LoadData ();
			Debug.Log ("Application State: Starting");
			currentDate = DateTime.Now;
			seconds = currentDate.Subtract (oldDate);
		
		}
	}
}
