using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraAdjust : MonoBehaviour {

	float yOffset;
	public float yOffsetPos1;
	public float yOffsetPos2;
	public float yOffsetPos3;
	public float yOffsetPos4;
	public float yOffsetPos5;
	public float yOffsetPos6;
	public float yOffsetPos7;
	public float yOffsetPos8;
	public float yOffsetPos9;

	public bool forcingCam;
	public float[] positions;
	public float pos1;
	public float pos2;
	public float pos3;
	public float pos4;
	public float pos5;
	public float pos6;
	public float pos7;
	public float pos8;
	public float pos9;

	 float maxY;
	 float maxX;
	 float minY;
	 float minX;
	 float midX;
	 float midY;
	 float absX;
	 float absY;

	void Start () {

	}

	// Update is called once per frame
	void Update () {
		FitGridInCamera (); //This will not be in an update!!!
	}
	//Camera.main.orthographicSize = (float ) (0 * Screen.height / Screen.width * rows);

	void FitGridInCamera()
	{
		maxY = -5;
		maxX = -5;
		minY = 5;
		minX = 5;
		midX = 0;
		midY = 0;
		absX = 0;
		absY = 0;

		foreach (Land land in Grid.Instance.lands) {
			if (land.gameObject.activeSelf) 
			{
				if (land.gameObject.transform.position.x < minX) {
					minX = land.gameObject.transform.position.x;
				}
				if (land.gameObject.transform.position.x > maxX) {
					maxX = land.gameObject.transform.position.x;
				}
				if (land.gameObject.transform.position.y < minY) {
					minY = land.gameObject.transform.position.y;
				}
				if (land.gameObject.transform.position.y > maxY) {
					maxY = land.gameObject.transform.position.y;
				}
				midX = (minX + maxX) / 2;
				midY = (minY + maxY) / 2;
				absX = Mathf.Abs (minX) + Mathf.Abs (maxX);
				absY = Mathf.Abs (minY) + Mathf.Abs (maxY);
			}
		}

		float abs = 0; 

		//abs = absX*1.5f >= absY ? absX/0.8f +1 : absY/1.2f; 

		/*
		if (absX >= absY) {
			Debug.Log ("absX: " + absX + " absY: " + absY);
			abs = absX;// * 1.3f;
		} 
		else {
			Debug.Log ("absY: " + absY + "absX: " + absX);
			abs = absY;
		}
		*/
		//abs = absX >= absY ? 

		if (forcingCam) {

			if (GameManager.Instance.playerLevel > 30) {
				Camera.main.orthographicSize = pos9; //6.85f;
				yOffset = yOffsetPos9;
			}
			else if (GameManager.Instance.playerLevel > 25) {
				Camera.main.orthographicSize = pos8; //6.06f;
				yOffset = yOffsetPos8;
			}

			else if (GameManager.Instance.playerLevel > 20) {
				Camera.main.orthographicSize = pos7;//5.17f;
				yOffset = yOffsetPos7;
			}
			else if (GameManager.Instance.playerLevel > 13) {
				Camera.main.orthographicSize = pos6;//4.64f;
				yOffset = yOffsetPos6;
			}

			else if (GameManager.Instance.playerLevel > 10) {
				Camera.main.orthographicSize = pos5; //4.3f;
				yOffset = yOffsetPos5;
			}
			else if (GameManager.Instance.playerLevel > 7) {
				Camera.main.orthographicSize = pos4; //4.22f;
				yOffset = yOffsetPos4;
			}
			else if (GameManager.Instance.playerLevel > 4) {
				Camera.main.orthographicSize = pos3; //3.55f;
				yOffset = yOffsetPos3;
			}
			else if (GameManager.Instance.playerLevel > 1) {
				Camera.main.orthographicSize = pos2; //3.55f;
				yOffset = yOffsetPos2;
			} 

			else if (GameManager.Instance.playerLevel == 1) {
				Camera.main.orthographicSize = pos1; //3.25f;
				yOffset = yOffsetPos1;
			}
			Camera.main.transform.position = new Vector3 (midX, midY +yOffset, -10); // here I will add the offset when side buttons appear
		}

		else{
		abs = absY;
		Camera.main.orthographicSize = abs * 1.2f;//1.25f;// + 1;// / 2;
		}
	}
}
