using System;
using System.Collections.Generic;
using UnityEngine;

public class C_RoomList : MonoBehaviour
{
    /// <summary>
    /// 新しい部屋を作った際の通知
    /// </summary>
    public Action<List<GameObject>> OnCreated;

    /// <summary>
    /// 部屋のプレハブ
    /// </summary>
    [SerializeField]
    private GameObject _roomFrontPrefab;

    /// <summary>
    /// 部屋を探すもの
    /// </summary>
    private C_RoomSeeker _roomSeeker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _roomSeeker = FindFirstObjectByType<C_RoomSeeker>();
        _roomSeeker.OnFoundRoomData += CreateRoomFront;
    }

    /// <summary>
    /// 受付を作る
    /// </summary>
    /// <param name="data">受付のデータ</param>
    private void CreateRoomFront(List<M_RoomFrontData> datas)
    {
        List<GameObject> roomFronts = new();
        foreach (M_RoomFrontData data in datas)
        {
            GameObject roomObject = Instantiate(_roomFrontPrefab);
            C_RoomFront roomFront = roomObject.GetComponent<C_RoomFront>();
            if (roomFront)
            {
                roomFront.Init(data);
            }
            roomFronts.Add(roomObject);
        }
        OnCreated?.Invoke(roomFronts);
    }
}
