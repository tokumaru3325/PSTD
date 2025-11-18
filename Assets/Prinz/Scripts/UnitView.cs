using UnityEngine;

public class UnitView : MonoBehaviour
{
    public void UpdateHealth(int hp)
    {
        // update sprite, bar, etc.
    }

    public void PlayMoveAnimation()
    { 
        // call animation here
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
