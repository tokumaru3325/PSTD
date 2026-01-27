using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class C_ObstacleStone : MonoBehaviour
{
    [Tooltip("初期データ")]
    [SerializeField]
    private M_Obstacle _initData;

    /// <summary>
    /// プレイヤーのタワービュー
    /// </summary>
    private V_ObstacleStone _myView;

    /// <summary>
    /// プレイヤーのタワーモデル
    /// </summary>
    private M_ObstacleStone _model;

    /// <summary>
    /// 死んだ後の処理
    /// </summary>
    public Action<Vector3> OnDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_initData)
            _model = new M_ObstacleStone(_initData);
        else
            _model = new M_ObstacleStone();

        _model.OnHPChanged += OnUpdateHP;
    }

    /// <summary>
    /// ビューを設定する
    /// </summary>
    /// <param name="view">プレイヤーのタワービュー</param>
    public void SetView(V_ObstacleStone view)
    {
        _myView = view;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">ダメージ</param>
    public void DecreaseHP(float damage)
    {
        if (_model.HP <= 0)
            return;

        _model.SetHP(_model.HP - damage);

        // 死亡
        if (_model.HP <= 0)
        {
            Dead().Forget();
            return;
        }

        if (_myView)
            _myView.HandleDamageEffect();
    }

    /// <summary>
    /// 体力更新時の処理
    /// </summary>
    /// <param name="hp">今の体力</param>
    /// <param name="maxHP">最大体力</param>
    private void OnUpdateHP(float hp, float maxHP)
    {
        if (_myView)
            _myView.UpdateHP(hp, maxHP);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private async UniTaskVoid Dead()
    {
        // 先に見えないようにしたのはDoTweenのアニメーションが終了した前にDestroyしたから
        OnDead?.Invoke(transform.position);
        this.gameObject.SetActive(false);
        // アニメーション終了待ちのための1秒
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        Destroy(this.gameObject);
    }
}
