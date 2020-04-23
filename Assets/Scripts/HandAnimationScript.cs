using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameAnalyticsSDK;
public class HandAnimationScript : MonoBehaviour {

	void Start() {
		Reset();
		Pressing();
	}
	public void UpdateHand()
	{
		Pressing ();
	}

	public void Pressing() {

		transform.GetComponent<RectTransform>().DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.55f).OnComplete(() => {
			transform.GetComponent<RectTransform>().DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.55f).OnComplete(() => {
				Pressing();
			}).SetEase(Ease.Linear);
		}).SetEase(Ease.Linear);
	}

	public void Reset()
	{
		transform.DOKill();
	}
}
