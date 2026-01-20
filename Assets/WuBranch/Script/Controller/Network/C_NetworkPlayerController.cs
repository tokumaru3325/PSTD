using UnityEngine;
using Unity.Netcode;

public class C_NetworkPlayerController : NetworkBehaviour
{
    /// <summary>
    /// タワーの最大体力
    /// </summary>
    [SerializeField]
    private float Max_HP = 100f;

    /// <summary>
    /// プレイヤーのタワービュー
    /// </summary>
    private V_PlayerTower _playerView;

    /// <summary>
    /// プレイヤーのタワーモデル
    /// </summary>
    private M_Tower _playerModel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ビューを設定する
    /// </summary>
    /// <param name="view">プレイヤーのタワービュー</param>
    public void SetView(V_PlayerTower view)
    {
        _playerView = view;
    }

    /// <summary>
    /// タワーの体力を減少させる
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void DecreaseHP(float damage)
    {
        if (_playerModel.HP <= 0)
            return;

        _playerModel.SetHP(_playerModel.HP - damage);

        if (_playerView)
            _playerView.HandleDamageEffect();
    }

    /// <summary>
    /// 体力更新時の処理
    /// </summary>
    /// <param name="hp"></param>
    private void OnUpdateHP(float hp)
    {
        if (_playerView)
            _playerView.UpdateHP(hp, Max_HP);
    }

    /// <summary>
    /// タワーが破壊されたかどうかを判定する
    /// </summary>
    /// <returns>破壊されていればtrue、そうでなければfalse</returns>
    public bool IsDead()
    {
        return _playerModel.HP <= 0;
    }

    //[2025/12/23] プリンス START
    // public M_Tower GetM_Tower()
    // {
    //     return _playerModel;
    // }
    //[2025/12/23] プリンス END
}
