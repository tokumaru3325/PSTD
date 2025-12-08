using System;
using UnityEngine;

public class C_RoomList : MonoBehaviour
{
    /// <summary>
    /// 新しい部屋を作った際の通知
    /// </summary>
    public Action<GameObject> OnCreated;

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

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateRoomFront(M_RoomFrontData data)
    {
        GameObject roomObject = Instantiate(_roomFrontPrefab);
        C_RoomFront roomFront = roomObject.GetComponent<C_RoomFront>();
        if (roomFront)
        {
            roomFront.Init(data);
        }

        OnCreated?.Invoke(roomObject);
    }
}
