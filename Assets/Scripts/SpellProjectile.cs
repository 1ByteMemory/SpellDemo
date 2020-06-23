using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
	public float projectileForce;
	public Vector3 forceDirection;
	Rigidbody rb;

	bool hasCast;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

	}

	private void Update()
	{
		if (!hasCast)
		{
			hasCast = true;
			rb.AddForce(forceDirection * projectileForce, ForceMode.Impulse);
		}
		
	}
}
