using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameAnalyticsSDK;
public class HandAnimation : MonoBehaviour {


    // Update is called once per frame
    void Start() {
		
		//GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Tutorial Step " + GameManager.Instance.tutorialSteps );
     
        //Pressing();
        Reset();
        transform.position = Camera.main.WorldToScreenPoint(GameObject.Find("Grid").transform.GetChild(1).GetChild(1).position);
        Pressing();
    }
	public void UpdateHand()
	{
		
			Debug.Log ("Entro en el updateHand");
			Reset ();

     
			if (GameManager.Instance.tutorialSteps == 1) {

				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Tutorial Step " + (GameManager.Instance.tutorialSteps - 1));
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Tutorial Step " + GameManager.Instance.tutorialSteps);

				Debug.Log ("Paso 1");
				transform.position = Camera.main.WorldToScreenPoint (GameObject.Find ("Grid").transform.GetChild (0).GetChild (1).position);
          
				Pressing ();
			} else if (GameManager.Instance.tutorialSteps == 2) {
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Tutorial Step " + (GameManager.Instance.tutorialSteps - 1));
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Tutorial Step " + GameManager.Instance.tutorialSteps);


				Debug.Log ("Paso Drag");
           

				Dragging ();
			} else if (GameManager.Instance.tutorialSteps == 3) {
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Tutorial Step " + (GameManager.Instance.tutorialSteps - 1));
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Tutorial Step " + GameManager.Instance.tutorialSteps);

				Debug.Log ("Paso tres");
				Pressing ();
				transform.position = GameObject.Find ("EggButton").transform.GetChild (0).position;
			}

	}

	public void Pressing() {
       
    	transform.GetComponent<RectTransform>().DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.55f).OnComplete(() => {
			transform.GetComponent<RectTransform>().DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.55f).OnComplete(() => {
				Pressing();
        	}).SetEase(Ease.Linear);
    	}).SetEase(Ease.Linear);
    }

	public void Dragging()
	{
		transform.DOScale (new Vector3(0.8f,0.8f,0.8f),0.6f).OnComplete(() => {
			transform.DOMove (Camera.main.WorldToScreenPoint(GameObject.Find("Animals").transform.GetChild(0).position), 0.4f, false).OnComplete(() => {
				transform.DOScale (new Vector3(1,1,1),0.6f).OnComplete(() => {
					transform.localScale = Vector3.one;
					transform.position = Camera.main.WorldToScreenPoint(GameObject.Find("Animals").transform.GetChild(1).position);
					Dragging();
				}).SetEase(Ease.Linear);
			}).SetEase(Ease.Linear);
		}).SetEase(Ease.Linear);
	}
	public void Reset()
	{
		transform.DOKill();
		//transform.position = positions [GameManager.Instance.tutorialSteps];
	}
}
