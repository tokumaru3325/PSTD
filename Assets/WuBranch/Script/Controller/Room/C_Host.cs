using Unity.Netcode;
using UnityEngine;

public class C_Host : MonoBehaviour
{
    void Start()
    {
        StartHost();
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
