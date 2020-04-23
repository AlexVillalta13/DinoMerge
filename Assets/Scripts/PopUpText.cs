using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopUpText : MonoBehaviour {

    private Vector3 initialPosition;
    public GameObject Animal;
    private Vector3 animalPosition;
    // Use this for initialization
    void Start () {
        //TextUp();
    }
	
	// Update is called once per frame
	void Update () {

        

    }

    public void TextUp()
    {
        gameObject.GetComponent<Text>().CrossFadeAlpha(100, 0.0f, true);
        transform.GetChild(0).gameObject.GetComponent<Image>().CrossFadeAlpha(100, 0.0f, true);
        var  positionAnimal = Camera.main.WorldToScreenPoint(Animal.transform.position); //vector2 del animal
        transform.position = positionAnimal;
        transform.DOMoveY(transform.position.y + 50f,3f,true).OnComplete(() => {
            transform.position = initialPosition;
            gameObject.GetComponent<Text>().CrossFadeAlpha(0, 0.8f, true);
            transform.GetChild(0).gameObject.GetComponent<Image>().CrossFadeAlpha(0, 0.8f, true);
            
            TextUp();
        }).SetDelay(Random.Range(1,2)); 
    }
}
