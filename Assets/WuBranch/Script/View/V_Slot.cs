using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class V_Slot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSlot()
    {
        SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
    }
}
