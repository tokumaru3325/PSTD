using Unity.Netcode;
using UnityEngine;

public class C_Client : MonoBehaviour
{

    void Start()
    {
        StartClient();
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
