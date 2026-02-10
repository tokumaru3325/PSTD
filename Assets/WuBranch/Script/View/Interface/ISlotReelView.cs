using UnityEngine;

public interface ISlotReelView : IView
{
    Vector2 ImageSize { get; }

    float StopOffset { get; }

    void Initialize(Vector2 size, Sprite[] sprites, float offset);

    void MoveReel(float distance);

    int FindTarget();

    float GetItemY(int index);
    float GetFirstItemY();
    int GetItemCount();
}
