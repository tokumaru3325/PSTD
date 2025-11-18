using UnityEngine;

public class TitleButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonDown_Start()
    {
        Debug.Log("Pushed StartButton");
    }

    public void OnButtonDown_Quit()
    {
        Debug.Log("Pushed QuitButton");
    }
}
