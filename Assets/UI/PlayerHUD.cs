using UnityEngine.UI;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public Image hpImage;

    public void SetHP(float value)
    {
        hpImage.fillAmount = value;
    }
}
