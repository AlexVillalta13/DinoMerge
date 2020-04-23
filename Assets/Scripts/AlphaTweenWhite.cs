using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DemiumGames.AdMobManager;
public class AlphaTweenWhite : MonoBehaviour {

		public float targetAlpha = 0;
		public float duration = 1.0f;
		public float delay = 0;

		public GameObject completionDelegate;
		public string completionMessage = "TweenAlphaComplete";

		SpriteRenderer spr;
		float startTime;
		float startAlpha;

		// Use this for initialization
		void OnEnable () {
			spr = GetComponent<SpriteRenderer>();
			startTime = Time.time;
			startAlpha = spr.color.a;
			Debug.Log ("start time: " + Time.time + " and a: " + startAlpha);
		}

		// Update is called once per frame
		void Update () {


			Color sc = spr.color;
			float pg = (Time.time - startTime);

			if (pg < delay) {
				sc.a = startAlpha;
				spr.color = sc;
				return;
			}

			pg -= delay;
			pg /= duration;

			if (pg >= 1.0f) {
				pg = 1.0f;
				this.enabled = false;
			}

			sc.a = Mathf.Lerp(startAlpha,targetAlpha,pg);
			spr.color = sc;

			if (pg >= 1.0f) {
				if (completionDelegate != null) {
					completionDelegate.SendMessage(completionMessage,this);
				}
			}


		}
	}
