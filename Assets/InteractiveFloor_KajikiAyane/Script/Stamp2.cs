using UnityEngine;

public class Stamp2 : StampArea
{
     public ParticleSystem particle;
    public GameObject stampArea;
    public override void OnEnter()
    {
        particle.Play();
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



