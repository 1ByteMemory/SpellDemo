using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellProjectile : MonoBehaviour
{
	public float projectileForce;
	public UnityEvent OnHitEffect;
	
	//public Vector3 forceDirection;
	Rigidbody rb;
	
	bool hasCast;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.CompareTag("floor"))
			OnHitEffect.Invoke();
	}


	private void Update()
	{
		if (!hasCast)
		{
			hasCast = true;
			if (rb == null)
				return;
			rb.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
		}
		
	}
}
