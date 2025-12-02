using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public struct TowerStage
{
    /// <summary>
    /// タワーのスプライト
    /// </summary>
    [SerializeField]
    public Sprite towerSprite;

    /// <summary>
    /// 体力の閾値(パーセント)
    /// </summary>
    [SerializeField]
    public float healthThresholdPecent;
}

public class V_PlayerTower : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    /// <summary>
    /// 各段階のタワーモデル
    /// </summary>
    [SerializeField]
    private List<TowerStage> _towerStages;

    /// <summary>
    /// プレイヤーのタワーコントローラー
    /// </summary>
    [SerializeField]
    private C_PlayerTowerController _playerController;

    /// <summary>
    /// タワーのスプライトレンダラー
    /// </summary>
    [SerializeField]
    private SpriteRenderer _tower;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_playerController)
        {
            _playerController.SetView(this);
        }
        _damageFlash = GetComponentInChildren<DamageFlash>();
        _uiShake = GetComponentInChildren<V_UIShake>();
        SetInitTowerImg();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleDamage(10f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HandleDamage(10f);
        }
    }

    /// <summary>
    /// タワーの見た目を初期化
    /// </summary>
    private void SetInitTowerImg()
    {
        float maxThreshold = 0f;
        int index = 0;
        for (int i = 0; i < _towerStages.Count; i++)
        {
            if (_towerStages[i].healthThresholdPecent > maxThreshold)
            {
                maxThreshold = _towerStages[i].healthThresholdPecent;
                index = i;
            }
        }
        if (_tower)
            _tower.sprite = _towerStages[index].towerSprite;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="Damage"></param>
    private void HandleDamage(float Damage)
    {
        // タワーにダメージを与える
        _playerController.DecreaseHP(Damage);

        // エフェクト
        _damageFlash.TriggerFlash();
        _uiShake.Shake();
    }

    /// <summary>
    /// タワーの体力を更新する
    /// </summary>
    /// <param name="hp">新しい体力</param>
    public void UpdateHP(float hp, float maxHp)
    {
        float healthRate = hp / maxHp;
        // タワーの段階を更新する
        for (int i = 0; i < _towerStages.Count; i++)
        {
            if (healthRate <= _towerStages[i].healthThresholdPecent)
            {
                if (_tower)
                {
                    _tower.sprite = _towerStages[i].towerSprite;
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
