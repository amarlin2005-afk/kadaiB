using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StampGenerator : MonoBehaviour
{
    [Header("スタンプエリアの左上にオブジェクトを置いてここにアタッチ")]
    [SerializeField] private Transform stampAreaA;

    [Header("スタンプエリアの右下にオブジェクトを置いてここにアタッチ")]
    [SerializeField] private Transform stampAreaB;

    [SerializeField] private int maxStamps;
    [SerializeField] private float stampInterval;

    [Header("スタンプ表示時のフェードイン")]
    [Tooltip("フェードインにかかる時間 [秒]")]
    [SerializeField] private float fadeInDuration = 0.5f;

    [Tooltip("フェードインの補間カーブ")]
    [SerializeField] private Ease fadeInEase = Ease.OutSine;

    public List<StampArea> stampPrefabs;
    public List<StampArea> activeStamps = new();

    public void Generate()
    {
        activeStamps.RemoveAll(s => s == null);

        if (activeStamps.Count < maxStamps)
            TryGenerateStamp();
    }

    private void TryGenerateStamp()
    {
        if (stampPrefabs.Count == 0) return;

        var pos = new Vector2(
            Random.Range(stampAreaA.position.x, stampAreaB.position.x),
            Random.Range(stampAreaB.position.z, stampAreaA.position.z)
        );

        foreach (var stamp in activeStamps)
        {
            if (Vector2.Distance(pos, stamp.StampPosition) < stampInterval)
                return;
        }

        var worldPos = new Vector3(pos.x, stampAreaA.position.y, pos.y);
        var newStamp = Instantiate(stampPrefabs[Random.Range(0, stampPrefabs.Count)], worldPos, Quaternion.identity, transform);

        StampFade.FadeIn(newStamp.gameObject, fadeInDuration, fadeInEase);

        activeStamps.Add(newStamp);
    }
}
