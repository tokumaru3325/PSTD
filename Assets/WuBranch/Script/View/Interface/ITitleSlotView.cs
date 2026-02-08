public interface ITitleSlotView : IView
{
    ReelInfo[] ReelSprites { get; }

    void SetReelsSprite(ReelInfo[] data);

    void StartWinEffect();
}
