using UnityEngine;

public class C_PlayerTowerController : MonoBehaviour
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
        _playerModel = new M_Tower(Max_HP);
        _playerModel.OnHPChanged += OnUpdateHP;
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
        _playerModel.SetHP(_playerModel.HP - damage);
    }

    /// <summary>
    /// タワーの体力を回復させる
    /// </summary>
    /// <param name="heal">回復量</param>
    public void IncreaseHP(float heal)
    {
        _playerModel.SetHP(_playerModel.HP + heal);
    }

    private void OnUpdateHP(float hp)
    {
        _playerView.UpdateHP(hp);
    }

    /// <summary>
    /// タワーが破壊されたかどうかを判定する
    /// </summary>
    /// <returns>破壊されていればtrue、そうでなければfalse</returns>
    public bool IsDead()
    {
        return _playerModel.HP <= 0;
    }
}
