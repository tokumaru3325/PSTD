using UnityEngine;

/// <summary>
/// シーンをまたいでの共通変数
/// </summary>
public class C_GlobalVariable : MonoBehaviour
{
    private M_GlobalVariable _datas;

    void Awake()
    {
        // 唯一にする
        C_GlobalVariable[] list = FindObjectsByType<C_GlobalVariable>(FindObjectsSortMode.None);
        if (list.Length >= 2)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _datas = new M_GlobalVariable();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// プレイヤーの自身の名前を設定
    /// </summary>
    /// <param name="name">名前</param>
    public void SetMyName(string name)
    {
        _datas.SetMyName(name);
    }

    /// <summary>
    /// プレイヤーの自身の名前を取得
    /// </summary>
    /// <returns>名前</returns>
    public string GetMyName()
    {
        return _datas.MyName;
    }

    /// <summary>
    /// 作った部屋の情報をセット
    /// </summary>
    /// <param name="data">部屋のデータ</param>
    public void SetRoomData(M_RoomData data)
    {
        _datas.SetRoom(data);
    }
}
