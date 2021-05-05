using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnOrientationFlipped()
    {
        m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
    }
}
