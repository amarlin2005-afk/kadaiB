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

    public override void OnEnter()
    {
        particle.Play();

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
