using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class ReelImageSaver : MonoBehaviour
{
    [SerializeField] Sprite[] monaySprite;
    [SerializeField] Sprite[] monsterSprite;
    [SerializeField] Sprite[] buffSprite;

    [SerializeField] Image reelL;
    [SerializeField] Image reelC;
    [SerializeField] Image reelR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (SlotSceneManager.slotType)
        {
            case 0:
                ImageChange(monaySprite);
                break;
            case 1:
                ImageChange(monsterSprite);
                break;
            case 2:
                ImageChange(buffSprite);
                break;
            default:
                ImageChange(monaySprite);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ImageChange(Sprite[] sprite)
    {
        reelL.sprite = sprite[0];
        reelC.sprite = sprite[1];
        reelR.sprite = sprite[2];
    }
}
