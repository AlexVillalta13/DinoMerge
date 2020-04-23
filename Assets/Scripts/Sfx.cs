using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sfx : MonoBehaviour {

    private AudioSource genericButton;
    private AudioSource mezclar;
    private AudioSource music;
    private AudioSource openEgg;


	// Use this for initialization
	void Start () {

        genericButton = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        mezclar = transform.GetChild(1).gameObject.GetComponent<AudioSource>();
        music = transform.GetChild(2).gameObject.GetComponent<AudioSource>();
        openEgg = transform.GetChild(3).gameObject.GetComponent<AudioSource>();
    }


    public void genericButtonPlay(){

        genericButton.Play();
        
        
    }
    public void MezclarPlay()
    {

        mezclar.Play();


    }
    public void musicPlay()
    {

        music.Play();


    }
    public void openEggPlay()
    {

        openEgg.Play();


    }

    public void genericButtonStop()
    {

        genericButton.Stop();


    }
    public void MezclarStop()
    {

        mezclar.Stop();


    }
    public void musicStop()
    {

        music.Stop();


    }
    public void openEggStop()
    {

        openEgg.Stop();


    }
}
