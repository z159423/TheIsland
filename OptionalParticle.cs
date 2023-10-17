using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    public void Play()
    {
        particle.Play();
        if (!particle.isPlaying)
        {
            particle.Clear();
        }
    }

    public void Stop()
    {
        if (particle.isPlaying)
            particle.Stop();
    }
}
