using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;

    public void setCard(string text, Sprite image)
    {
        this.text.text = text;
        this.image.sprite = image;
    }
}
