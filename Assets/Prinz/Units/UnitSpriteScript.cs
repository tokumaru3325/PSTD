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
}
