using DG.Tweening;
using UnityEngine;

public class ParticleStamp : StampArea
{
    public ParticleSystem particle;
    public GameObject stampArea;

    [Header("消える時のフェードアウト")]
    [Tooltip("フェードアウトにかかる時間 [秒]")]
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Tooltip("フェードアウトの補間カーブ")]
    [SerializeField] private Ease fadeOutEase = Ease.InSine;
    
    [SerializeField] private AudioClip audioClip1;
    [SerializeField] private AudioClip audioClip2;

    public override void OnEnter()
    {
        
        Debug.Log("ParticleStamp OnEnter");
        particle.Play();
        
        var audioClip = Random.Range(0, 2) == 0 ? audioClip1 : audioClip2;
        if (audioClip != null)AudioSource.PlayClipAtPoint(audioClip, transform.position);

        // フェードアウトしてから非表示にする
        StampFade.FadeOut(stampArea, fadeOutDuration, fadeOutEase, () =>
        {
            if (stampArea != null) stampArea.SetActive(false);
        });

        Destroy(gameObject, 4);
    }
    
    public override void OnStay()
    {
        
    }
    
    public override void OnExit()
    {
        
    }
}
