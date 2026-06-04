using UnityEngine;
using System.Collections.Generic;

public class ParticleStamp : StampArea
{
    public List<ParticleSystem> particles;
    public GameObject stampArea;
    public override void OnEnter()
    {
        foreach (var particle in particles)
        { 
            particle.Play();
        }
        stampArea.SetActive(false);
        
        
        Destroy(gameObject,4);
    }
    
    public override void OnStay()
    {
        
    }
    
    public override void OnExit()
    {
    }
}
