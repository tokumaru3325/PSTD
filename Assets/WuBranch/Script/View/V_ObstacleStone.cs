using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class V_ObstacleStone : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
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
    /// 採掘者の生成ボタン
    /// </summary>
    private V_MinerSpawnButton _minerSpawner;

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
        _minerSpawner = FindFirstObjectByType<V_MinerSpawnButton>();
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

    /// <summary>
    /// 注目される
    /// </summary>
    public void Highlight()
    {
        _mySprite.sortingOrder = 40;
        _mySprite.sortingLayerName = "Obstacle";
    }

    /// <summary>
    /// 注目されない
    /// </summary>
    public void Unhighlight()
    {
        _mySprite.sortingOrder = 30;
        _mySprite.sortingLayerName = "Default";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _healthGauge.HideGauge();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _healthGauge.ShowGauge();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_myController.CanBeSelected)
            return;

        Debug.Log($"stone clicked");
        if (_minerSpawner)
            _minerSpawner.SpawnMiner(this.gameObject);
    }
}
