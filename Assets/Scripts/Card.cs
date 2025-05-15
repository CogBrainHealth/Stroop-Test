using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Stage stage;
    
    public TextMeshProUGUI text;
    public Image image;
    
    public Sprite originalSprite; 
    public Sprite clickedSprite; // 클릭됐을 때 사용할 스프라이트
    private SpriteRenderer spriteRenderer;
    
    private bool isPointerDown = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = originalSprite;
    }

    public void setCard(string text, Sprite image)
    {
        this.text.text = text;
        this.image.sprite = image;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        spriteRenderer.sprite = clickedSprite;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerDown)
        {
            isPointerDown = false;
            spriteRenderer.sprite = originalSprite;

            // 0.1초 후 실행
            Invoke(nameof(stage.Choice), 0.1f);
            // 유니티에서 Choice 로직 호출
        }
    }
}
