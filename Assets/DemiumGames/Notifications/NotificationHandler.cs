using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace DemiumGames.Notifications{

	//Ejemplo de manager de notificaciones muy simple. 
	public class NotificationHandler : MonoBehaviour {

		[System.Serializable]
		private struct NotificationData
		{
			public int id;
			public string title;
			public string text;
			public int seconds; 
		}

		string[] texts;
		int[] notificationsSeconds;
		private string smallIcon = Resources.noticono;
		private string largeIcon = Resources.noticonobig; 

		public int hours;
		public int minutes;
		public int seconds;

		[SerializeField]
		private List<NotificationData> notifications;


		public void CancellAllNotifications()
		{
			NotificationManager.Instance.CancelAllNotifications (); 
		}

		// Use this for initialization
		void Start () {
			this.CancellAllNotifications(); 
			texts = new string[] {
				"Your dinos are wating for you!", 
				"Play Dino Merge Now!", 
				"Discover new Dinos!", 
				"Feed your hungry dinos",
				"It’s been a while. Those dinos won’t merge themselves.",
				"Come back to see your dinos.",
				"It's been a while :(",
				"We miss you :("
			};
			notificationsSeconds = new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		}

		void OnApplicationPause(bool pauseStatus)
		{
			DateTime dateTime = DateTime.Now;
			DateTime dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day + 1, hours, minutes, seconds);

			double secondsTillFirstNotification = dt.Subtract (dateTime).TotalSeconds;

			for (int i = 2; i < notificationsSeconds.Length; i++) {
				if (i > 6) {
					notificationsSeconds [i] = (int)secondsTillFirstNotification + ((i - 5) * 604800);
				}
				else{
					notificationsSeconds [i] = (int)secondsTillFirstNotification + (i * 86400);
				}
			}

			//texts [4] += GameManager.Instance.highscore + " score now!";
			//texts [5] += GameManager.Instance.highscore;
			if (pauseStatus)
			{
				int randomy;
				Debug.Log ("Pausado"); 
				for (int i = 2; i < notifications.Count; i++) {
					if (i > 5) {
						randomy = UnityEngine.Random.Range (6, texts.Length - 1);
					} 
					else
					{
						randomy = UnityEngine.Random.Range (0, texts.Length - 3);
					}

					NotificationManager.Instance.SendNotification(notifications[i].id, notifications[i].title, texts[randomy], smallIcon, largeIcon, notificationsSeconds[i]);
				}
				string landFulltext = "Land is full. So many eggs to open!";
				string moneyFullText = "Your dinos have earned you " + TextManager.instance.FormatTotalCurrency(OffTimeManager.Instance.GetMaxEarning()) + " while you were away. Come and collect it!";
				int fiveMin = 300;

				NotificationManager.Instance.SendNotificationWithAppIcon(notifications[0].id, notifications[0].title, landFulltext, smallIcon, fiveMin);
				NotificationManager.Instance.SendNotificationWithAppIcon(notifications [1].id, notifications [0].title, moneyFullText, smallIcon, OffTimeManager.Instance.GetMaxSeconds());
			}
			else
			{
				Debug.Log ("No pausado"); 
				this.CancellAllNotifications(); 
			}
		}
	}
}
