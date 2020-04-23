using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization; 

namespace DemiumGames.Saveable{

public class Vector3SerializationSurrogate : ISerializationSurrogate{
	
	public void GetObjectData(System.Object obj,SerializationInfo info, StreamingContext context)
	{

		Vector3 v3 = (Vector3)obj;
		info.AddValue("x", v3.x);
		info.AddValue("y", v3.y);
		info.AddValue("z", v3.z);
	}


	public System.Object SetObjectData(System.Object obj,SerializationInfo info,
		StreamingContext context,ISurrogateSelector selector)
	{

		Vector3 v3 = (Vector3)obj;
		v3.x = (float)info.GetValue("x", typeof(float));
		v3.y = (float)info.GetValue("y", typeof(float));
		v3.z = (float)info.GetValue("z", typeof(float));
		obj = v3;
		return obj;
	}



}

public class Vector2SerializationSurrogate : ISerializationSurrogate{
	public void GetObjectData(System.Object obj,SerializationInfo info, StreamingContext context)
	{

		Vector2 v2 = (Vector2)obj;
		info.AddValue("x", v2.x);
		info.AddValue("y", v2.y);
	}


	public System.Object SetObjectData(System.Object obj,SerializationInfo info,
		StreamingContext context,ISurrogateSelector selector)
	{

		Vector2 v2 = (Vector2)obj;
		v2.x = (float)info.GetValue("x", typeof(float));
		v2.y = (float)info.GetValue("y", typeof(float));
		obj = v2;
		return obj;
	}

}

public class TextureSerializationSurrogate : ISerializationSurrogate{
	public void GetObjectData(System.Object obj,SerializationInfo info, StreamingContext context)
	{

		Texture2D text = (Texture2D)obj; 
		byte[] bytes = text.EncodeToPNG (); 

		info.AddValue ("width", text.width); 
		info.AddValue ("height", text.height); 
		info.AddValue ("bytes", bytes); 
		info.AddValue ("name", text.name); 


	}


	public System.Object SetObjectData(System.Object obj,SerializationInfo info,
		StreamingContext context,ISurrogateSelector selector)
	{

		int width = (int)info.GetValue ("width", typeof(int)); 
		int height = (int)info.GetValue ("height", typeof(int)); 

		Texture2D text = new Texture2D (width, height); 

		byte[] bytes = (byte[])info.GetValue ("bytes", typeof(byte[])); 
		text.LoadImage (bytes); 
		text.name = (string)info.GetValue ("name", typeof(string)); 
		obj = text; 
		return obj; 
	}


}
}