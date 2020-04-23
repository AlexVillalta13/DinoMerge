using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="Scriptable", menuName= "DinoPrices", order = 0)]
public class ScriptableObjectExample : ScriptableObject {
	public double[] gold;
	public List<string> dinoNames;
	public List<int> experienceForBuying;
}
