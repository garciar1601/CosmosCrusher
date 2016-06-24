using UnityEngine;
using System.Collections;

public class ParticleKiller : MonoBehaviour {

	void Update () {
        if (!gameObject.transform.GetComponentInChildren<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
