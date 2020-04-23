using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public int amountToPool;
    public GameObject objectToPool;
    public bool shouldExpand;
}

public class ParticlePooler : MonoBehaviour {

	public int numberOfParticlesToCleanAt = 0;
    public static ParticlePooler SharedInstance;
    public List<ObjectPoolItem> itemsToPool;

    public List<GameObject> particleObjects;

    public GameObject container;
    void Awake()
    {
        SharedInstance = this;
    }
    // Use this for initialization
    void Start()
    {
        particleObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {

                GameObject obj = (GameObject)Instantiate(item.objectToPool, container.transform);
                obj.SetActive(false);
                particleObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < particleObjects.Count; i++)
        {
            if (!particleObjects[i].activeInHierarchy && particleObjects[i].tag == tag)
            {
				numberOfParticlesToCleanAt++;
				if (numberOfParticlesToCleanAt > 15) {
					SleepParticleObjects ();
				}
                return particleObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
					GameObject obj = (GameObject)Instantiate(item.objectToPool, container.transform);
                    obj.SetActive(false);
                    particleObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
	public void SleepParticleObjects()
	{
		//if (Grid.Instance.part4.activeSelf) {
			//Grid.Instance.part4.SetActive (false);
		//}
		foreach (GameObject item in particleObjects) {
			if (item.GetComponent<ParticleSystem> () != null) {
				if (!item.GetComponent<ParticleSystem> ().isPlaying) {
					item.gameObject.SetActive (false);
				}
			} 
			else if (item.name == "LevelUpParticles(Clone)") {
				ParticleSystem[] effects = item.GetComponentsInChildren<ParticleSystem>();
				if (!effects [0].isPlaying) {
					item.SetActive (false);
				}
			}
		}
	}
}
