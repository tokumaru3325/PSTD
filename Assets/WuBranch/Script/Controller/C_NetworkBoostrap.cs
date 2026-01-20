using Netcode.Transports;
using Unity.Netcode;
using UnityEngine;

public class C_NetworkBoostrap : MonoBehaviour
{
    public GameObject networkManagerPrefab;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindAnyObjectByType<NetworkManager>() == null)
        {
            GameObject networkManager = Instantiate(networkManagerPrefab);
            SteamNetworkingSocketsTransport transport = FindFirstObjectByType<SteamNetworkingSocketsTransport>();
            networkManager.GetComponent<NetworkManager>().NetworkConfig.NetworkTransport = transport;
        }
    }
}
