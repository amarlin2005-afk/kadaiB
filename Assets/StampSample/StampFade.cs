using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// スタンプ配下の Renderer マテリアルアルファをフェードさせる共通処理。
/// （URP / Built-in / Particle 系の各色プロパティに対応）
/// </summary>
public static class StampFade
{
    // マテリアルのアルファを探す際の候補プロパティ（URP / Built-in / Particle 系）
    private static readonly string[] ColorProperties = { "_BaseColor", "_Color", "_TintColor" };

    /// <summary>
    /// 対象配下の全 Renderer のマテリアルアルファを 0 から元の値へフェードインさせる。
    /// </summary>
    public static void FadeIn(GameObject target, float duration, Ease ease)
    {
        if (target == null) return;

        foreach (var r in target.GetComponentsInChildren<Renderer>(true))
        {
            // インスタンス化されたマテリアルに対してフェードをかける
            foreach (var mat in r.materials)
            {
                var prop = GetColorProperty(mat);
                if (prop == null) continue;

                // 元のアルファを保持し、いったん透明にしてから元の値へ戻す
                var color = mat.GetColor(prop);
                var targetAlpha = color.a;

                color.a = 0f;
                mat.SetColor(prop, color);

                mat.DOFade(targetAlpha, prop, duration).SetEase(ease);
            }
        }
    }

    /// <summary>
    /// 対象配下の全 Renderer のマテリアルアルファを 0 へフェードアウトさせる。
    /// 全フェード完了後に <paramref name="onComplete"/> を呼ぶ。
    /// </summary>
    public static void FadeOut(GameObject target, float duration, Ease ease, Action onComplete = null)
    {
        if (target == null)
        {
            onComplete?.Invoke();
            return;
        }

        var sequence = DOTween.Sequence();
        foreach (var r in target.GetComponentsInChildren<Renderer>(true))
        {
            // インスタンス化されたマテリアルに対してフェードをかける
            foreach (var mat in r.materials)
            {
                var prop = GetColorProperty(mat);
                if (prop == null) continue;

                sequence.Join(mat.DOFade(0f, prop, duration).SetEase(ease));
            }
        }

        // フェード対象に色プロパティが無ければ即完了
        if (sequence.Duration() <= 0f)
        {
            sequence.Kill();
            onComplete?.Invoke();
            return;
        }

        if (onComplete != null) sequence.OnComplete(() => onComplete());
    }

    /// <summary>
    /// マテリアルが持つアルファ操作用の色プロパティ名を返す（無ければ null）。
    /// </summary>
    private static string GetColorProperty(Material mat)
    {
        foreach (var p in ColorProperties)
        {
            if (mat.HasProperty(p)) return p;
        }
        return null;
    }
}
