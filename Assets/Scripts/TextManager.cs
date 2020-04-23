using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextManager : MonoBehaviour {

	public double testMoney;
	public string letter;
	public string[] letters;

	public static TextManager instance;

	void Awake () {
		instance = this;
	}

	public string FormatTotalCurrency(double currency)
	{
		string currencyString = currency.ToString ("0." + new string ('#', 339));

		double doubleString = Math.Floor (Double.Parse(String.Format("{0:0.##}",currencyString)));
		int lengthOfDigits = doubleString.ToString ("0." + new string ('#', 339)).Length;

		if (lengthOfDigits < 5) {
			letter = letters [(lengthOfDigits / 4)];
		}
		else 
		{
			letter = letters [((lengthOfDigits - 1) / 3)];
		}

		currencyString = doubleString.ToString ("0." + new string ('#', 339));

		string currencyString2 = null;
		string currencyString3 = null;

		if (lengthOfDigits % 3 == 0) {
			currencyString2 = currencyString.Substring (0, currencyString.Length - (currencyString.Length - 3));

			currencyString3 = currencyString.Substring (3);
		} 
		else if (lengthOfDigits % 3 == 1) {
			currencyString2 = currencyString.Substring (0, currencyString.Length - (currencyString.Length - 1));
			currencyString3 = currencyString.Substring (1);
		} else if (lengthOfDigits % 3 == 2) 
		{
			currencyString2 = currencyString.Substring (0, currencyString.Length - (currencyString.Length - 2));
			currencyString3 = currencyString.Substring (2);
		}

		if(currencyString3.Length > 3)
		{
			currencyString3 = currencyString3.Remove(currencyString3.Length - (currencyString3.Length - 2));
		}
		else if(currencyString3.Length > 2)
		{
			currencyString3 = currencyString3.Remove(currencyString3.Length - (currencyString3.Length - 2));
		}

		double finalDouble = double.Parse(currencyString);
		if (currencyString3 != "") {
			finalDouble = double.Parse ("." + currencyString3) + double.Parse(currencyString2);
		}
		string finalString = "" + finalDouble.ToString() + "" + letter;
		return finalString;
	}
}
