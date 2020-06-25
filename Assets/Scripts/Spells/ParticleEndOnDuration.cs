using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEndOnDuration : MonoBehaviour
{
	ParticleSystem particle;
	public float destroyDelay;

	private void Start()
	{
		particle = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (particle.time >= particle.main.duration + destroyDelay)
		{
			Destroy(gameObject);
		}
	}
}
