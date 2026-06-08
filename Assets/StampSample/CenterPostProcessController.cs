using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using InteractiveFloor;

/// <summary>
/// 作品の起動トリガー。
/// フィールドの中心に鑑賞者が来たら、PostProcessing(URP Volume)の
/// Vignette の Intensity を下げ、ColorAdjustments の Post Exposure を 0 に戻す
/// アニメーションを DOTween で再生する。
/// アニメーション完了後、<see cref="OnActivationComplete"/> を通知し、
/// 後続のスタンプ生成フローへ繋ぐ。
///
/// フロー: 人が中心に来る ＝＞ PostProcessing アニメーション ＝＞ スタンプ生成開始
///
/// StampSample 内で完結するシステム。
/// </summary>
public class CenterPostProcessController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("鑑賞者の位置を取得する SensorObjectManager")]
    [SerializeField] private SensorObjectManager _sensorObjectManager;

    [Tooltip("Vignette と ColorAdjustments を持つ URP の Volume")]
    [SerializeField] private Volume _volume;

    [Header("Field Center (XZ)")]
    [Tooltip("フィールドの中心。未指定ならこのオブジェクトの位置を使用する。")]
    [SerializeField] private Transform _fieldCenter;

    [Tooltip("中心とみなす半径（ワールド空間）")]
    [SerializeField] private float _centerRadius = 1.0f;

    [Header("Target Values (アニメーション後の値)")]
    [Tooltip("下げたあとの Vignette Intensity")]
    [SerializeField] private float _targetVignetteIntensity = 0.0f;

    [Tooltip("戻したあとの Post Exposure（0 に戻す）")]
    [SerializeField] private float _targetPostExposure = 0.0f;

    [Header("Tween")]
    [Tooltip("アニメーションにかかる時間 [秒]")]
    [SerializeField] private float _duration = 1.0f;

    [Tooltip("補間カーブ")]
    [SerializeField] private Ease _ease = Ease.InOutSine;
    

    /// <summary>
    /// 中心に人が来て、PostProcessing のアニメーションが完了したときに呼ばれる。
    /// （スタンプ生成の開始フックとして使用する）
    /// </summary>
    public event Action OnActivationComplete;

    /// <summary>
    /// 起動シーケンスが開始済みか（このフローは一度きり）。
    /// </summary>
    public bool IsActivated => _isActivated;

    private Vignette _vignette;
    private ColorAdjustments _colorAdjustments;

    private bool _isActivated;
    private Sequence _sequence;

    private void Awake()
    {
        if (_fieldCenter == null) _fieldCenter = transform;

        if (_volume != null && _volume.profile != null)
        {
            _volume.profile.TryGet(out _vignette);
            _volume.profile.TryGet(out _colorAdjustments);
        }
    }

    private void Update()
    {
        // 起動済みなら以降の監視は不要（一度きりのフロー）
        if (_isActivated) return;

        if (IsAnyViewerAtCenter())
            Activate();
    }

    /// <summary>
    /// 起動シーケンス。PostProcessing アニメーションを再生し、完了を通知する。
    /// </summary>
    private void Activate()
    {
        _isActivated = true;

        _sequence = DOTween.Sequence();

        if (_vignette != null)
        {
            _sequence.Join(DOTween.To(
                    () => _vignette.intensity.value,
                    v => _vignette.intensity.value = v,
                    _targetVignetteIntensity, _duration)
                .SetEase(_ease));
        }

        if (_colorAdjustments != null)
        {
            _sequence.Join(DOTween.To(
                    () => _colorAdjustments.postExposure.value,
                    v => _colorAdjustments.postExposure.value = v,
                    _targetPostExposure, _duration)
                .SetEase(_ease));
        }

        // アニメーション完了後、後続フロー（スタンプ生成）へ通知
        _sequence.OnComplete(() => OnActivationComplete?.Invoke());
    }

    /// <summary>
    /// いずれかの鑑賞者が中心の半径内にいるか
    /// </summary>
    private bool IsAnyViewerAtCenter()
    {
        if (_sensorObjectManager == null || _sensorObjectManager.SensorList == null)
            return false;

        // SensorObject の positionMedian は (x, y) = ワールドの (X, Z)
        var center = new Vector2(_fieldCenter.position.x, _fieldCenter.position.z);

        foreach (var sensorObject in _sensorObjectManager.SensorList)
        {
            var pos = new Vector2(sensorObject.positionMedian.x, sensorObject.positionMedian.y);
            if (Vector2.Distance(pos, center) < _centerRadius)
                return true;
        }

        return false;
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }

    private void OnDrawGizmosSelected()
    {
        var c = _fieldCenter != null ? _fieldCenter : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(c.position, _centerRadius);
    }
}
