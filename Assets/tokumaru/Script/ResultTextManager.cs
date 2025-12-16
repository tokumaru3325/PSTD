using TMPro;
using UnityEngine;

public class ResultTextManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TextMeshProUGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TextMeshProUGUI.SetText("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TextChange(int slotNum,int resultNum,bool koyaku)
    {
        if (!koyaku)
        {
            m_TextMeshProUGUI.SetText("はずれ");
        }
        else
        {
            if (slotNum == 0)
            {
                switch (resultNum)
                {
                    case 0:
                        m_TextMeshProUGUI.SetText("リプレイ");
                        break;
                    case 1:
                        m_TextMeshProUGUI.SetText("ベル");
                        break;
                    case 2:
                        m_TextMeshProUGUI.SetText("すいか");
                        break;
                    case 3:
                        m_TextMeshProUGUI.SetText("チェリー");
                        break;
                    case 4:
                        m_TextMeshProUGUI.SetText("7");
                        break;
                    default:
                        m_TextMeshProUGUI.SetText("はずれ");
                        break;

                }
            }
            else if(slotNum == 1)
            {
                m_TextMeshProUGUI.SetText("何かをどのくらいの強さでどれだけ");
            }
            else
            {
                m_TextMeshProUGUI.SetText("何かの効果を誰かに向かってどれだけ");
            }
        }
    }

    public void ResetText()
    {
        m_TextMeshProUGUI.SetText("");
    }
}
