using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class V_ObstacleStone : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{

    /// <summary>
    /// 各段階の画像
    /// </summary>
    [SerializeField]
    private List<HealthBasedSprite> _phaseSprites;

    /// <summary>
    /// スプライトレンダラー
    /// </summary>
    [SerializeField]
    private SpriteRenderer _mySprite;

    /// <summary>
    /// 体力ゲージ
    /// </summary>
    [SerializeField]
    private V_HealthGauge _healthGauge;

    /// <summary>
    /// ダメージフラッシュエフェクト
    /// </summary>
    private DamageFlash _damageFlash;

    /// <summary>
    /// UIシェイクエフェクト
    /// </summary>
    private V_UIShake _uiShake;

    /// <summary>
    /// コントローラー
    /// </summary>
    [SerializeField]
    private C_ObstacleStone _myController;

    void Awake()
    {
        if (_myController)
        {
            _myController.SetView(this);
        }
        _damageFlash = GetComponentInChildren<DamageFlash>();
        _uiShake = GetComponentInChildren<V_UIShake>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitSprite();
    }

    /// <summary>
    /// タワーの見た目を初期化
    /// </summary>

    private void InitSprite()
    {
        float maxThreshold = 0f;
        int index = 0;
        for (int i = 0; i < _phaseSprites.Count; i++)
        {
            if (_phaseSprites[i].ThresholdPecent > maxThreshold)
            {
                maxThreshold = _phaseSprites[i].ThresholdPecent;
                index = i;
            }
        }
        if (_mySprite)
            _mySprite.sprite = _phaseSprites[index].Sprite;
    }

    /// <summary>
    /// 表示順番を設定
    /// </summary>
    /// <param name="order">表示順番</param>
    public void SetOrder(int order)
    {
        _mySprite.sortingOrder = order;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="Damage"></param>
    public void HandleDamageEffect()
    {
        Debug.Log($"stone effect");
        // エフェクト
        _damageFlash.TriggerFlash();
        _uiShake.Shake();
    }

    /// <summary>
    /// 体力を更新する
    /// </summary>
    /// <param name="hp">新しい体力</param>
    public void UpdateHP(float hp, float maxHp)
    {
        float healthRate = hp / maxHp;
        // 段階を更新する
        for (int i = 0; i < _phaseSprites.Count; i++)
        {
            if (healthRate <= _phaseSprites[i].ThresholdPecent)
            {
                if (_mySprite)
                {
                    _mySprite.sprite = _phaseSprites[i].Sprite;
                }
            }
        }
        // 体力ゲージを更新する
        _healthGauge.SetGauge(healthRate);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _healthGauge.HideGauge();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _healthGauge.ShowGauge();
    }
}
