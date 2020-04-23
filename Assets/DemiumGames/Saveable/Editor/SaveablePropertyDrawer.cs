using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System; 


namespace DemiumGames.Saveable{
[CustomPropertyDrawer(typeof(Saveable))]
public class SaveablePropertyDrawer : PropertyDrawer {
	private int height = 30; 
	private int width = 100;

	private bool secure; 

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label){

		if (property.isExpanded) {
			return EditorGUI.GetPropertyHeight (property) + height * 1.3f; 
		} else {
			return EditorGUI.GetPropertyHeight (property); 
		}
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.PropertyField(position, property, label, true);

		Saveable saveable = attribute as Saveable; 
		secure = saveable.secure; 
		Saveable.SaveableDataType saveType = saveable.saveType; 


		if (property.isExpanded) {
			int previousIndentLevel = EditorGUI.indentLevel; 
			EditorGUI.indentLevel = 2; 
			Rect posRect = EditorGUI.IndentedRect (position); 
			float fieldHeight = GetPropertyHeight (property, label) + 2; 


			width =(int) EditorGUIUtility.currentViewWidth / 4; 

			Rect buttonRect = new Rect (width/2 + width, posRect.y + fieldHeight - height - height * 0.1f, width, height); 

			 


			if (GUI.Button (buttonRect, "Save Property")) {
				var typeClass = property.serializedObject.targetObject.GetType(); 
				var field = typeClass.GetField (property.propertyPath); 
				if (field != null) {
					var value = field.GetValue (property.serializedObject.targetObject); 

					var path = EditorUtility.SaveFilePanel ("Save data", "", value.GetType ().ToString () + 
						(saveType == Saveable.SaveableDataType.BINNARY ? ".miralles" : ".json"), 
						(saveType == Saveable.SaveableDataType.BINNARY ? "miralles" : "json")); 
					if (path.Length != 0) {

						switch (saveType) {
							case Saveable.SaveableDataType.BINNARY: 
							BinnaryFormatterHelper.Save (value, GetPathWithoutFileName(path), GetFileName(path), secure); 
							break; 

						case Saveable.SaveableDataType.JSON:
							JsonFormatterHelper.Save (value, GetPathWithoutFileName (path), GetFileName (path), secure); 
							break; 
						}

					}
					EditorGUIUtility.ExitGUI (); 

				}


			}

			buttonRect.x += width + width/5; 

			if (GUI.Button(buttonRect, "Load Property")){
				var typeClass = property.serializedObject.targetObject.GetType(); 
				var field = typeClass.GetField (property.propertyPath); 
				if (field != null) {
					string path = EditorUtility.OpenFilePanel ("Load data", "", saveType == Saveable.SaveableDataType.BINNARY ? "miralles" : "json"); 

					if (path.Length != 0) {
						switch (saveType) {
						case Saveable.SaveableDataType.BINNARY:
							
							var give = BinnaryFormatterHelper.Load <object>(GetPathWithoutFileName (path), GetFileName(path), secure); 
							field.SetValue (property.serializedObject.targetObject, give); 
							break; 

						case Saveable.SaveableDataType.JSON:
							Type type = field.FieldType; 
							var JsonObject = new JsonFormatterObject (); 
							var typeJsonFormatter = JsonObject.GetType (); 

							var method = typeJsonFormatter.GetMethod ("Load"); 
							var genericMethod = method.MakeGenericMethod (type); 
							var giveJSON = genericMethod.Invoke (JsonObject, new object[]{GetPathWithoutFileName (path), GetFileName (path), secure}); 
						

							//var giveJSON = JsonFormatterHelper.Load <object>(GetPathWithoutFileName (path), GetFileName(path), secure); 
							field.SetValue (property.serializedObject.targetObject, giveJSON); 

							break; 
						}
					
					}
					EditorGUIUtility.ExitGUI (); 
				}

			}

		
		}
	}


	public string GetPathWithoutFileName(string path){
		int countLastBar = -1; 

		for (int i = 0; i < path.Length; i++) {
			if (path[i] == '/') {
				countLastBar = i; 
			}
		}
		string aux = ""; 
		for (int i = 0; i < countLastBar; i++) {
			aux += path [i];
		}

		return aux; 
	}


	public string GetFileName(string path){
		int countLastBar = -1; 

		for (int i = 0; i < path.Length; i++) {
			if (path [i] == '/') {
				countLastBar = i; 
			}
		}

		string aux = ""; 
		for (int i = countLastBar + 1; i < path.Length; i++) {
			aux += path [i];
		}

		return aux; 
	}

}
}