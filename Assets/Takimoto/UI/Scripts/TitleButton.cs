using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
    }

    public void OnButtonDown_Quit()
    {
        Application.Quit();
    }
}
