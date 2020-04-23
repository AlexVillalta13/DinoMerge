using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  DG.Tweening;
public class ParticleToUI : MonoBehaviour {

    public GameObject point;
    public float speed = 1f;
    private RectTransform m_RectTransform;
    private Vector3 posicion;
    // Use this for initialization
    void Start () {
		LoadStuff ();
    }
	public void LoadStuff()
	{
		m_RectTransform = point.GetComponent<RectTransform>();
		Camera camera = Camera.main;
		posicion = Camera.main.ScreenToWorldPoint(m_RectTransform.transform.position);

		transform.DOMove(posicion, speed, false).OnComplete(() => {
			point.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f).OnComplete(() => {
				point.transform.DOScale(new Vector3(1, 1, 1), 0.15f).SetEase(Ease.Linear);
			}).SetEase(Ease.Linear);

			ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();

			var main = ps.main;
			main.loop = false;

			ParticleSystem child = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();

			var mainChild  = child.main;
			mainChild.loop = false;

		}).SetEase(Ease.Linear);
	}
}
