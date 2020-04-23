using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DemiumGames.Notifications{

	public class NotificationManager : MonoBehaviour {

	    public delegate void DGNotificationCallback();

		
	    private AndroidJavaClass intermediateClass;
	    private AndroidJavaClass notificationManager; 
	    private static NotificationManager instance;

	    private Dictionary<int, DGNotificationCallback> callbackDictionary; 
	   

	    void Awake()
	    {
	        if (instance == null)
	        {
	            if (callbackDictionary == null)
	                callbackDictionary = new Dictionary<int, DGNotificationCallback>(); 
	            intermediateClass = new AndroidJavaClass("com.demiumgames.notificationmodule.DGIntermediateActivity");
	            notificationManager = new AndroidJavaClass("com.demiumgames.notificationmodule.NotificationFragment");
	            instance = this; 
	        }
	    }

		//Lo mismo que en OnApplicationPause
	    void Start()
	    {
	        int number = intermediateClass.GetStatic<int>("number");
	        if (number != -1)
	        {
	            if (callbackDictionary.ContainsKey(number))
	                callbackDictionary[number]();
				intermediateClass.CallStatic ("Reset"); 

	        }
	    }

		/// <summary>
		/// Setea un callback para un identificador de notificación concreto
		/// </summary>
		/// <param name="id">Identificador de la notificación</param>
		/// <param name="callBack">Función o función ánonima a ser llamada.</param>
	    public void SetCallbackInt(int id, DGNotificationCallback callBack)
	    {
	        if (callbackDictionary == null)
	            callbackDictionary = new Dictionary<int, DGNotificationCallback>(); 
	        callbackDictionary.Add(id, callBack); 
	    }


		/// <summary>
		/// Manda la notificación con el icono a elección.
		/// </summary>
		/// <param name="id">Identificador único de la notificación.</param>
		/// <param name="title">Texto de título</param>
		/// <param name="text">Texto del cuerpo de la notificación</param>
		/// <param name="smallIcon">Resources. iconoPequeño.</param>
		/// <param name="largeIcon">Resources. icono grande </param>
	    public void SendNotification(int id, string title, string text, string smallIcon, string largeIcon, int seconds)
	    {

	        this.notificationManager.CallStatic("SendNotification", id, title, text, smallIcon, largeIcon, seconds); 

	    }
		/// <summary>
		/// Manda la notificación con el icono de la aplicación como icono.
		/// </summary>
		/// <param name="id">Identificador único de la notificación.</param>
		/// <param name="title">Texto de título</param>
		/// <param name="text">Texto del cuerpo de la notificación</param>
		/// <param name="smallIcon">Resources. iconoPequeño.</param>
	    public void SendNotificationWithAppIcon(int id, string title, string text, string smallIcon, int seconds)
	    {
	        SendNotification(id, title, text, smallIcon, "app_icon", seconds); 
	    }

	    

		/// <summary>
		/// Cancela la notificación, proporcionandole todos los datos.
		/// </summary>
		/// <param name="id">Identificador único de la notificación.</param>
		/// <param name="title">Texto de título</param>
		/// <param name="text">Texto del cuerpo de la notificación</param>
		/// <param name="smallIcon">Resources. iconoPequeño.</param>
		/// <param name="largeIcon">Resources. icono grande </param>
	    public void CancelNotification(int id, string title, string text, string smallIcon, string largeIcon)
	    {
	        this.notificationManager.CallStatic("CancelNotification", id, title, text, smallIcon, largeIcon);
	        this.notificationManager.CallStatic("CancelNotification", id, title, text, smallIcon, "app_icon");

	    }

		/// <summary>
		/// Cancela la notificación, proporcionandole el id único.
		/// </summary>
		/// <param name="id">identificador de notificacione.</param>
		public void CancelNotification(int id){
			this.notificationManager.CallStatic ("CancelNotification", id); 
		}


		/// <summary>
		/// Cancela todas las notificaciones registradas previamente
		/// </summary>

		public void CancelAllNotifications(){
			
			this.notificationManager.CallStatic ("CancellAllNotifications"); 
		}


		//En el OnAppplicationPause comprobamos si el número de la actividad intermedia está seteado, en cuyo caso 
		//hemos accedido a la aplicación a través de una notificación. 
	    void OnApplicationPause(bool pauseStatus)
	    {
	        if (!pauseStatus)
	        {
	            int number = intermediateClass.GetStatic<int>("number");

	            if (number != -1)
	            {
	                if (callbackDictionary.ContainsKey(number))
	                    callbackDictionary[number](); 
					intermediateClass.CallStatic ("Reset"); 
	            }
	        }

	    }


	   

	    public static NotificationManager Instance
	    {
	        get
	        {
	            return instance;
	        }

	    }
	}
}
