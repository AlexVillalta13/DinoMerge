using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using DG.Tweening;
using DG.Tweening;

public class TouchInput : MonoBehaviour
{
	public static TouchInput Instance;
	bool touchActivated = true;
	//public float yTouchSpace;
	public LayerMask touchInputMask;
	RaycastHit hit;

	public Vector2 initialPosition;


	public bool pressingLandObject = false;
	GameObject heldThingForShadow;

	public void SetTouchActivated(bool touchActivated)
	{
		this.touchActivated = touchActivated;
	}
	void Start()
	{
		Instance = this;
	}
	void Update ()
	{
		if (touchActivated) {
			if (GameManager.Instance.visitingShop) {
			} else {
				#if UNITY_EDITOR
				//InputEditor(); 
				#endif
				//InputMobile ();
				InputEditor ();
			}
		}
	}


	private void InputMobile(){
		if (Input.touchCount > 0) {
			Touch touch = Input.touches [0];
			Vector3 position3D = touch.position;

			Ray ray = Camera.main.ScreenPointToRay (touch.position);

			if (touch.phase == TouchPhase.Began) {
				if (Physics.Raycast (ray, out hit, 500.5f, touchInputMask)) {
					GameObject landRecipient = hit.transform.gameObject;
					//GameManager.Instance.selectedAnimal = landRecipient;
					pressingLandObject = true;
					//Debug.Log ("OnTouchDown");
				}
			} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {

				//Debug.Log ("OnTouchEnded");
				pressingLandObject = false;
			} else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {

				if (pressingLandObject) {
					//Debug.Log ("Should be moving");
					//if (GameManager.Instance.selectedPiece != null) {
					//GameManager.Instance.selectedPiece.transform.positionT = position + new Vector2 (0, yTouchSpace);
					//}
				}

			} 
		} 
	}

	private void InputEditor(){
		Vector2 position = Input.mousePosition; 
		position = (Vector2)Camera.main.ScreenToWorldPoint (position);

		if (GameManager.Instance.holdingObject) {
			GameManager.Instance.animalRecipient.GetComponent<SpriteRenderer> ().sortingOrder = 3;
			GameManager.Instance.animalRecipient.transform.position = position;// + new Vector2(0, yTouchSpace);

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 500.5f, touchInputMask)) {
				//Debug.Log("changing color");
				if(heldThingForShadow != null)
				{
					heldThingForShadow.GetComponent<SpriteRenderer>().color = new Color(1,1,1);	
				}

				heldThingForShadow = hit.transform.gameObject;
				heldThingForShadow.GetComponent<SpriteRenderer>().color = new Color(0.8f,0.8f,0.8f);
			}
			else{
				if(heldThingForShadow != null)
				{
					heldThingForShadow.GetComponent<SpriteRenderer>().color = new Color(1,1,1);	
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			pressingLandObject = false;
			if (GameManager.Instance.animalRecipient != null) {
				GameManager.Instance.animalRecipient.GetComponent<SpriteRenderer> ().sortingOrder = 1;
			}
			GameManager.Instance.holdingObject = false;

			if(heldThingForShadow != null)
			{
				heldThingForShadow.GetComponent<SpriteRenderer>().color = new Color(1,1,1);	
				heldThingForShadow = null;
			}
			//Debug.LogWarning ("MouseUP");

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 500.5f, touchInputMask)) {
				if(GameManager.Instance.animalRecipient != null)
				{
					GameManager.Instance.targetLandRecipent = hit.transform.GetComponent<Land>();

					GameManager.Instance.SetAnimal(GameManager.Instance.animalRecipient);
				}
			} else {
				if(GameManager.Instance.animalRecipient != null && GameManager.Instance.landRecipient != null)
				{
					GameManager.Instance.animalRecipient.transform.position = GameManager.Instance.landRecipient.transform.position;
				}
			}
			GameManager.Instance.targetLandRecipent = null;
			GameManager.Instance.animalRecipient = null;
			GameManager.Instance.landRecipient = null;

		} else if (Input.GetMouseButton (0)) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 500.5f, touchInputMask)) {

				if (GameManager.Instance.landRecipient == null) {
					GameManager.Instance.landRecipient = hit.transform.GetComponent<Land>();

					if (Input.GetMouseButtonDown (0)) {
						//Debug.LogWarning ("MouseDown");
						pressingLandObject = true;
					}
				}
				else
				{
					//pressingLandObject = true;
				}
				if (pressingLandObject) {
					if (GameManager.Instance.landRecipient != null) {	
						Animal animal = GameManager.Instance.landRecipient.GetAnimal ();
						if (animal != null) {
							bool isBox = GameManager.Instance.CheckIfBox(animal);
							if(isBox)
							{
								//Debug.Log("Box!");
								GameManager.Instance.holdingObject = false;
								pressingLandObject = false;
								GameManager.Instance.landRecipient = null;
								GameManager.Instance.animalRecipient = null;
								GameManager.Instance.targetLandRecipent = null;
								GameManager.Instance.OpenBox(animal);
							}
							else
							{
								GameManager.Instance.animalRecipient = animal;
								GameManager.Instance.holdingObject = true;
								DOTween.Kill (GameManager.Instance.animalRecipient.transform);
								//GameManager.Instance.animalRecipient.transform.DOMove().Kill (false);
								GameManager.Instance.animalRecipient.DoAnimation ();

								pressingLandObject = false;
							}
						}
						else
						{
							GameManager.Instance.landRecipient = null;
						}
					}
				}
			}
		}
	}
}
