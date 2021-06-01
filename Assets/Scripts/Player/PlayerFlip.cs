using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnOrientationFlipped()
    {
        m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;
    }
}
