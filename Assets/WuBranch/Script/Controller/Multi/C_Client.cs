using Unity.Netcode;
using UnityEngine;

public class C_Client : MonoBehaviour
{
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
