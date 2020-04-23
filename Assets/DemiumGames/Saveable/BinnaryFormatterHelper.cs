using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using System.Runtime.Serialization; 

namespace DemiumGames.Saveable{

public static class BinnaryFormatterHelper {

	public delegate void CallbackBinnary<T>( T obj); 

	private static string password = "MIRALLES"; 

	public static void Save <T> (T obj, string name, bool secure = false){
		Save (obj, Application.persistentDataPath, name, secure); 
	}

	public static void Save <T> (T obj, string path, string name, bool secure = false){
		BinaryFormatter binaryFormatter = new BinaryFormatter (); 

		binaryFormatter.SurrogateSelector = GetSurrogateSelector(); 

		if (secure) {
			MemoryStream memory = new MemoryStream (); 
			binaryFormatter.Serialize (memory, obj); 
			memory.Close (); 
			byte[] bytes = memory.ToArray (); 
			CypherRSA rsa = new CypherRSA ("MIRALLES"); 
			byte[] encryptedBytes = rsa.Encrypt (bytes); 
			FileStream file = File.Create (path + "/" + name); 
			file.Write (encryptedBytes, 0, encryptedBytes.Length); 
			file.Close (); 

		} else {
			FileStream file = File.Create (path + "/" + name); 
			binaryFormatter.Serialize (file, obj); 
			file.Close (); 
		}
	}

	public static T Load <T>(string name, bool secure = false){
		return Load<T> (Application.persistentDataPath, name, secure); 
	}

	public static void LoadFromBinnary<T>(this MonoBehaviour obj,string url, CallbackBinnary<T> call, bool secure = false){
		obj.StartCoroutine (LoadCoroutine<T>  (url, call, secure)); 

	}



	private static IEnumerator LoadCoroutine<T>(string url, CallbackBinnary<T> call, bool secure){
		WWW www = new WWW (url); 

		yield return www; 

		if (www.error == null) {
			string aux = www.text;

			byte[] byteArr = www.bytes;  
			MemoryStream str; 
			T obj;
			if (secure) {
				CypherRSA rsa = new CypherRSA ("MIRALLES"); 
				str = new MemoryStream (byteArr); 
				byteArr = rsa.Decrypt(str);


			} 
			BinaryFormatter formatter = new BinaryFormatter (); 
			formatter.SurrogateSelector = GetSurrogateSelector (); 
			str = new MemoryStream (byteArr); 
			obj = (T)formatter.Deserialize (str); 
			str.Close (); 

			call (obj);
			www.Dispose ();
			yield break; 

		} else {
			www.Dispose (); 
			yield break; 
		}

	}
	

	public static T Load<T>(string path, string name, bool secure = false){
		T obj = default(T); 
		if (File.Exists (path + "/" + name)) {

			if (secure) {
				CypherRSA rsa= new CypherRSA ("MIRALLES");
				FileStream fileStream = new FileStream (path + "/" + name, FileMode.Open); 
				byte[] bytes = rsa.Decrypt (fileStream); 
				MemoryStream str = new MemoryStream (bytes); 
				BinaryFormatter bf = new BinaryFormatter (); 
				bf.SurrogateSelector = GetSurrogateSelector (); 

				obj = (T)bf.Deserialize (str); 
				str.Close (); 
			} else {
				BinaryFormatter bf = new BinaryFormatter (); 
				bf.SurrogateSelector = GetSurrogateSelector(); 
				FileStream file = File.Open (path+ "/" + name, FileMode.Open); 
				obj = (T)bf.Deserialize (file); 
				file.Close ();
			}


		}

		return obj; 
	}


	public static SurrogateSelector GetSurrogateSelector(){
		SurrogateSelector surrogateSelector = new SurrogateSelector (); 
		Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate (); 
		TextureSerializationSurrogate textureSS = new TextureSerializationSurrogate (); 
		Vector2SerializationSurrogate vector2SS = new Vector2SerializationSurrogate (); 

		surrogateSelector.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), vector2SS); 
		surrogateSelector.AddSurrogate (typeof(Vector3), new StreamingContext (StreamingContextStates.All), vector3SS); 
		surrogateSelector.AddSurrogate (typeof(Texture2D), new StreamingContext (StreamingContextStates.All), textureSS); 
		return surrogateSelector; 
	}





	public static string Password {
		get {
			return password;
		}
		set {
			password = value;
		}
	}
}
}




