using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class AnimalPoolManager : MonoBehaviour {

	public static AnimalPoolManager Instance;

	public List<Animal> unchangingAnimals;
	public List<Animal> animals;
	public List<Animal> animalsToRecover;

	public bool startLoadingButton = false;

	int turnsForNextGiftEgg = 5;
	public int chanceGiftEggLow;
	public int chanceGiftEggHigh;

	void Start()
	{
		Instance = this;
		RefreshNextGiftEgg ();
	}
	void Update()
	{
		if (startLoadingButton || GameManager.Instance.tutorialWaiting) {
			DoTimeForButtonNewBox ();
		}
	}
	public void RefreshNextGiftEgg()
	{
		turnsForNextGiftEgg = Random.Range (chanceGiftEggLow, chanceGiftEggHigh + 1);
	}
	public void DoTimeForButtonNewBox()
	{
		if (GameManager.Instance.tutorial == false) {
			if (GameManager.Instance.SECONDSFORBOX < GameManager.Instance.timeManager.seconds.TotalSeconds + GameManager.Instance.currentSecondsLeftForBox) {
				//Debug.Log ("Instantiating Little One");
				int piecesAvailable = GetHowManyLandsAvailable ();
				//Debug.Log ("piecesAvailable: " + piecesAvailable);
				if (animals.Count > 0 && piecesAvailable > 0) {
					if (turnsForNextGiftEgg == 0) {
						if (GameManager.Instance.persistance.maxAnimalUnlocked > 4) {
							PlaceAnimalOnPlace (-2, AddAnimalsToRecover ());
						} 
						else {
							PlaceAnimalOnPlace (0, AddAnimalsToRecover ());
						}
						RefreshNextGiftEgg ();
					} 
					else
					{
						PlaceAnimalOnPlace (0 ,AddAnimalsToRecover ());
						turnsForNextGiftEgg--;
					}
					GameManager.Instance.boxesPerSession++;
				}
				//PlaceAnimalOnPlace (AddAnimalsToRecover ());
				if (GameManager.Instance.numberOfLands > 0) {
					GameManager.Instance.currentSecondsLeftForBox = 0;
					GameManager.Instance.timeManager.ResetSeconds ();
				}
			}
			GameManager.Instance.ColorBoxButton (GameManager.Instance.currentSecondsLeftForBox);
		} 
		else if(GameManager.Instance.tutorialWaiting == false)
		{
			GameManager.Instance.tutorialWaiting = true;
			if (animals.Count > 0) {
				PlaceAnimalOnPlace (0, AddAnimalsToRecover ());
				GameManager.Instance.boxesPerSession++;
			}
			//PlaceAnimalOnPlace (AddAnimalsToRecover ());
			if (GameManager.Instance.numberOfLands > 0) {
				GameManager.Instance.currentSecondsLeftForBox = 0;
				GameManager.Instance.timeManager.ResetSeconds ();
			}
		}
	}

	public void UpdateDinos(Animal animal)
	{
		animalsToRecover.Add (animal);
		animals.Remove (animal);
	}

	public void AddAnimalToAnimals(Animal animal)
	{
		animalsToRecover.Remove (animal);
		animals.Add (animal);
		animal.level = -1;
		if (animal.level >= 0) {
			animal.transform.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [animal.level];
		} 
		else 
		{
			animal.transform.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [0];
		}

	}
	public Animal AddAnimalsToRecover()
	{
		Animal animalini = animals[0];
		animals.Remove(animalini);
		animalsToRecover.Add (animalini);
		return(animalini);
	}
		
	public int GetHowManyLandsAvailable()
	{
		Land[] freeAnimals = (from land in Grid.Instance.lands
		where land.animal == null && land.gameObject.activeSelf == true
		select land).ToArray ();

		GameManager.Instance.numberOfLands = freeAnimals.Length;

		if (freeAnimals.Length > 0)
			Grid.Instance.landToPlaceAt = freeAnimals [Random.Range(0, freeAnimals.Length)];

		return GameManager.Instance.numberOfLands;
	}


	public void PlaceAnimalOnPlace(int levelAtWhichBought, Animal animal)
	{
		if (GameManager.Instance.tutorial) {
			animal.GetComponent<SpriteRenderer>().sortingOrder = 2;
			animal.DoAnimationBox (Grid.Instance.lands[13 + GameManager.Instance.tutorialSteps]);
			Grid.Instance.lands[13 + GameManager.Instance.tutorialSteps].animal = animal;
			animal.level = 0; 
			Grid.Instance.landToPlaceAt = null;
		} 
		else
		{
			animal.GetComponent<SpriteRenderer> ().sortingOrder = 2;
			animal.DoAnimationBox (Grid.Instance.landToPlaceAt);
			Grid.Instance.landToPlaceAt.animal = animal;
			if (levelAtWhichBought == -2) {
				animal.giftEgg = true;
				animal.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [43];
				GameManager.Instance.persistance.animalInts [Grid.Instance.landToPlaceAt.id] = -2;
			}
			else if (levelAtWhichBought > 0) {

				int indexOfAnimal = unchangingAnimals.IndexOf (animal);
				GameManager.Instance.persistance.animalHiddenInts [Grid.Instance.landToPlaceAt.id] = levelAtWhichBought;//unchangingAnimals[
				animal.hiddenLevel = levelAtWhichBought;
				animal.GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.sprites [41]; //yellow egg
			} 
			animal.level = 0;

			if (GameManager.Instance.persistance.animalInts [Grid.Instance.landToPlaceAt.id] != -2) {
				GameManager.Instance.persistance.animalInts [Grid.Instance.landToPlaceAt.id] = 0;
			}
			Grid.Instance.landToPlaceAt = null;
		}
		GameManager.Instance.persistance.SaveData ();
	}
}
