using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    /// <summary>
    /// フラッシュの色
    /// </summary>
    [ColorUsage(true, true)]
    [SerializeField]
    private Color _flashColor = Color.white;

    /// <summary>
    /// フラッシュの持続時間
    /// </summary>
    [SerializeField]
    private float _flashDuration = 0.25f;

    /// <summary>
    /// フラッシュの速度曲線
    /// </summary>
    [SerializeField]
    private AnimationCurve _flashSpeedCurve;

    private SpriteRenderer _spriteRenderer;
    private Material _material;
    private Coroutine _flashCoroutine;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer)
            _material = _spriteRenderer.material;
    }

    public void TriggerFlash()
    {
        _flashCoroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        // 色を設定してフラッシュを開始
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _flashDuration)
        {
            // 経過時間に基づいてフラッシュ量を更新
            elapsedTime += Time.deltaTime;

            // フラッシュ量を0から1へ線形補間
            currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), elapsedTime / _flashDuration);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    /// <summary>
    /// // 色を設定する
    /// </summary>
    private void SetFlashColor()
    {
        if (_material)
        {
            _material.SetColor("_FlashColor", _flashColor);
        }
    }

    /// <summary>
    /// フラッシュ量を設定する
    /// </summary>
    private void SetFlashAmount(float amount)
    {
        if (_material)
        {
            _material.SetFloat("_FlashAmount", amount);
        }
    }
}
