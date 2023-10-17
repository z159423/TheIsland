using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAnimationEvent : MonoBehaviour
{
    [SerializeField] ParticleSystem fallParticle;

    public void FallParticle()
    {
        fallParticle.Play();
    }
}
