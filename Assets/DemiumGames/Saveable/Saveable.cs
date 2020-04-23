using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DemiumGames.Saveable{

public class Saveable : PropertyAttribute{

	public enum SaveableDataType
	{
		JSON, 
		BINNARY
	}

	public readonly bool secure; 
	public readonly SaveableDataType saveType;


	public Saveable(bool secure, SaveableDataType saveType){
		this.secure = secure; 
		this.saveType = saveType; 
	}
}
}