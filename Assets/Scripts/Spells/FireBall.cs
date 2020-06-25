using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : SpellProjectile
{
	public GameObject FireAOEPrfab;
   
    public void FireAOE()
	{
		Instantiate(FireAOEPrfab, transform.position , Quaternion.LookRotation(Vector3.up));
		
		Destroy(gameObject);
	}
}
