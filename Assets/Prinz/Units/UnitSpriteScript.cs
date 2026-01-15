using UnityEngine;

public class UnitSpriteScript : MonoBehaviour
{
    [SerializeField] private UnitView _view;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void OnDeathAnimationEnd()
    {
        _view.OnDeathAnimationEnd();
    }
    public void OnDefeatAnimationStart()
    {
        _view.OnDefeatAnimationStart();
    }
    public void OnDefeatAnimationEnd()
    {
        _view.OnDefeatAnimationEnd();
    }
    public void FaceLeft()
    {
        _view.FaceLeft();
    }
    public void FaceRight()
    {
        _view.FaceRight();
    }
}
