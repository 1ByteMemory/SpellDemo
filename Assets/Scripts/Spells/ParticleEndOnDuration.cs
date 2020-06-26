using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEndOnDuration : MonoBehaviour
{
	ParticleSystem particle;
	public float destroyDelay;

	float endTime;



	private void OnEnable()
	{
		particle = GetComponent<ParticleSystem>();
		endTime = Time.time + particle.main.duration + destroyDelay;
	}

	private void Update()
	{
		if (Time.time >= endTime)
		{
			Destroy(gameObject);
		}
	}
}
