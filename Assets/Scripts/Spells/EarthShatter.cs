using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthShatter : SpellProjectile
{
	public GameObject EarthShatterAOEPrefab;

    public void EarthShatterAOE()
	{
		Instantiate(EarthShatterAOEPrefab, transform.position, new Quaternion());
		Destroy(gameObject);
	}
}
