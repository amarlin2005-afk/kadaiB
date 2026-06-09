using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialogueSpritePlayer : MonoBehaviour
{
    [Header("表示するSpriteRenderer")]
    [SerializeField] private SpriteRenderer dialogueRenderer;

    [Header("表示する画像")]
    [SerializeField] private List<Sprite> dialogueSprites;

    [Header("各画像の表示時間")]
    [SerializeField] private List<float> displayTimes;

    [Header("フェード時間")]
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private CenterPostProcessController centerPostProcessController;

    private void OnEnable()
    {
        centerPostProcessController.OnPlayDialogue += StartDialogue;
        dialogueRenderer.enabled = false;
    }

    private void OnDisable()
    {
        centerPostProcessController.OnPlayDialogue -= StartDialogue;
    }

    private void StartDialogue()
    {
        dialogueRenderer.enabled = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        Debug.Log("PlaySequence");

        // 最初は透明
        Color color = dialogueRenderer.color;
        color.a = 0;
        dialogueRenderer.color = color;

        for (int i = 0; i < dialogueSprites.Count; i++)
        {
            // スプライト切り替え
            dialogueRenderer.sprite = dialogueSprites[i];

            // フェードイン
            dialogueRenderer.DOFade(1f, fadeDuration);

            // 表示時間
            float displayTime = 3f;

            if (i < displayTimes.Count)
                displayTime = displayTimes[i];

            yield return new WaitForSeconds(displayTime);

            // 最後以外はフェードアウト
            if (i != dialogueSprites.Count - 1)
            {
                dialogueRenderer.DOFade(0f, fadeDuration);

                yield return new WaitForSeconds(fadeDuration);
            }
        }
    }
}