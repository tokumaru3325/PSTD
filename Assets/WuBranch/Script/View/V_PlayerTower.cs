using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public struct TowerStage
{
    /// <summary>
    /// タワーのスプライト
    /// </summary>
    [SerializeField]
    public Sprite towerSprite;

    /// <summary>
    /// 体力の閾値
    /// </summary>
    [SerializeField]
    public float healthThreshold;
}

public class V_PlayerTower : MonoBehaviour
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
    /// ダメージフラッシュエフェクト
    /// </summary>
    private DamageFlash _damageFlash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_playerController)
        {
            _playerController.SetView(this);
        }
        _damageFlash = GetComponent<DamageFlash>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HandleDamage(10f);
        }
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="Damage"></param>
    private void HandleDamage(float Damage)
    {
        // タワーにダメージを与える
        _playerController.DecreaseHP(10f);

        // エフェクト
        _damageFlash.TriggerFlash();
    }

    /// <summary>
    /// タワーの体力を更新する
    /// </summary>
    /// <param name="hp">新しい体力</param>
    public void UpdateHP(float hp)
    {
        // タワーの段階を更新する
        for (int i = 0; i < _towerStages.Count; i++)
        {
            if (hp <= _towerStages[i].healthThreshold)
            {
                GetComponent<SpriteRenderer>().sprite = _towerStages[i].towerSprite;
                break;
            }
        }
    }
}
