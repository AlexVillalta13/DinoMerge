using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameAnalyticsSDK;

public class Animal : MonoBehaviour {

	public int level;
	public int hiddenLevel;
	public bool mysteryEgg;
	public bool giftEgg;
	float speed;

	private SpriteRenderer rend; 

	private float originalScale; 
	void Start(){
		rend = GetComponent<SpriteRenderer> ();
		speed = Random.Range(0.3f, 0.9f);
		DoAnimation ();
		originalScale = this.transform.localScale.x;
		//DoCurrencyAnimation ();
	}

	public void UpgradeLevel(Land landTarget, Land landRecipient) //BOTH CAN BE Recipient in case of box but level = 0
	{
		if (giftEgg || mysteryEgg) {
			//Call POP UP Method 
			PopupManager.Instance.SetAnimal(this);
			PopupManager.Instance.FormDinoGifted();
			MenuManager.Instance.ShowPopUpMysteryEgg();
		}
		else if (level == -1) {
			level = 1;
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "AnimalLevelReached " + GameManager.Instance.persistance.maxAnimalUnlocked);
		} 
		else 
		{
			if (hiddenLevel > 0) {
				level = hiddenLevel;
				GameManager.Instance.persistance.animalInts[landTarget.id] = level;
				GameManager.Instance.persistance.animalHiddenInts[landTarget.id] = -1;


				GameManager.Instance.experience += ShopManager.Instance.priceList.experienceForBuying[level-1];

				GameManager.Instance.AddExperience ();
				if (GameManager.Instance.experience >= GameManager.Instance.experiencePerLevel[GameManager.Instance.playerLevel -1])
				{
					GameManager.Instance.experience = GameManager.Instance.experience - (int)GameManager.Instance.experiencePerLevel [GameManager.Instance.playerLevel - 1];
				}

				ParticleSystem part2 = ParticlePooler.SharedInstance.GetPooledObject ("PathParticle").GetComponent<ParticleSystem>();
				part2.GetComponent<ParticleToUI> ().point = GameObject.FindGameObjectWithTag ("StarCentre");
				part2.GetComponent<ParticleToUI> ().LoadStuff ();
				part2.transform.position = transform.position;
				part2.gameObject.SetActive(true);
				part2.Play ();

				GameManager.Instance.UpdateUI ();
			} 
			else {
				level++;
			}
			if (GameManager.Instance.persistance.maxAnimalUnlocked < level) {
				GameManager.Instance.persistance.maxAnimalUnlocked = level;

				if (GameManager.Instance.persistance.maxAnimalUnlocked == 5) {
					ShopTutorial.Instance.StartShopTutorial ();
				}
			}
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "AnimalLevelReached " + GameManager.Instance.persistance.maxAnimalUnlocked);

			if (GameManager.Instance.tutorial) {
				if (GameManager.Instance.tutorialSteps < 2) {
					GameManager.Instance.tutorialSteps++;
					GameManager.Instance.handTutorial.GetComponent<HandAnimation> ().UpdateHand ();
					if (GameManager.Instance.tutorialSteps < 2) {
						GameManager.Instance.tutorialWaiting = false;
					}
				}
			}
		}
		if (giftEgg == false && mysteryEgg == false) {
			hiddenLevel = 0;
			rend.sprite = GameManager.Instance.sprites [level];
		}
		if (hiddenLevel < 1) {
			if (giftEgg == false && mysteryEgg == false) 
			{
				if (landRecipient.animal == landTarget.animal) { //Box
					GameManager.Instance.persistance.animalInts [landTarget.id] = 1;
				} else { //At least level 1
					GameManager.Instance.persistance.animalInts [landTarget.id] = level;
					GameManager.Instance.persistance.animalInts [landRecipient.id] = -1;
				}

				if (rend == null) {
					rend = GetComponent<SpriteRenderer> (); 
				}
			}
		}
		GameManager.Instance.persistance.SaveData ();
	}
	public void DoAnimationBox(Land land)
	{
		land.animating = true;
		Vector3 endPosition = land.transform.position;
		transform.localPosition = endPosition + new Vector3 (0, 3, 0);

		transform.DOMove (endPosition, 0.8f).OnComplete (() => {
			land.animating = false;
			land.animal.GetComponent<SpriteRenderer>().sortingOrder = 1;
			if(land.animal.level == -1)
			{
				land.animal.level = 0;
			}
		});

	}
	public void SetAnimating(Land land)
	{
		land.animating = false;
	}

	public double CollectMoney()
	{
		GameManager.Instance.currency += GameManager.Instance.goldPerAnimalLevel [level];
		return GameManager.Instance.goldPerAnimalLevel [level];
	}
	public void DoAnimation()
	{
		this.transform.DOPunchScale (new Vector3 (0.00f, 0.05f, 0.05f), speed, 1, 1f).OnComplete (DoAnimation).OnKill (() => {
			this.transform.localScale = new Vector3(originalScale, originalScale,1); 
		});
	}
	public void DoCurrencyAnimation()
	{
		this.transform.DOMove (new Vector3 (0, 0, 0), 1, false).OnComplete(() => {
			//DoCurrencyAnimation();
			//Transform child = transform.GetChild(1);
			//child.GetComponent<PopUpText>().TextUp(transform.position);
		});
	}
}
