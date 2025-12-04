using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void SlotResult(int i);
public class SlotSceneManager : MonoBehaviour
{
    private static SlotResult ak = (int i) => { };
    private static bool open = false;
    private static int slotType = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void ChangeScene()
    {
        if(open == false)
        {
            SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
            open = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync("Slot");
            open = false;
        }
    }

    public static void SetFunc(SlotResult sr)
    {
        ak += sr;
    }
}
