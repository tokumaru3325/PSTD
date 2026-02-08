using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(V_ObstacleStone))]
public class C_ObstacleStone : MonoBehaviour, IObstacleStoneController
{
    [Tooltip("初期データ")]
    [SerializeField]
    private M_Obstacle _initData;

    /// <summary>
    /// プレイヤーのタワービュー
    /// </summary>
    private IObstacleStoneView _myView;

    /// <summary>
    /// プレイヤーのタワーモデル
    /// </summary>
    private M_ObstacleStone _model;

    /// <summary>
    /// 死んだ後の処理
    /// </summary>
    public Action<Vector3> OnDead;

    /// <summary>
    /// 選ばれるか
    /// </summary>
    public bool CanBeSelected { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_initData)
            _model = new M_ObstacleStone(_initData);
        else
            _model = new M_ObstacleStone();

        _myView = GetComponent<IObstacleStoneView>();
        _model.OnHPChanged += OnUpdateHP;
        CanBeSelected = false;
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
            SoundManager.Instance.PlaySE(SoundId.RockBreak, SEPlayParams.Default);
            Dead().Forget();
            return;
        }

        SoundManager.Instance.PlaySE(SoundId.Mining, SEPlayParams.Default);
        _myView?.HandleDamageEffect();
    }

    /// <summary>
    /// 体力更新時の処理
    /// </summary>
    /// <param name="hp">今の体力</param>
    /// <param name="maxHP">最大体力</param>
    private void OnUpdateHP(float hp, float maxHP)
    {
        _myView?.UpdateHP(hp, maxHP);
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

    /// <summary>
    /// 死亡したか
    /// </summary>
    /// <returns>true: はい, false: いいえ</returns>
    public bool IsDead()
    {
        return _model.HP <= 0;
    }

    /// <summary>
    /// 選ばれる状態になる
    /// </summary>
    public void EnableBeSelected()
    {
        CanBeSelected = true;
        _myView.Highlight();
    }

    /// <summary>
    /// 選ばれない状態になる
    /// </summary>
    public void DisableBeSelected()
    {
        CanBeSelected = false;
        _myView.Unhighlight();
    }
}
