using UnityEngine;

public class BlockRandomVariation : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_spriteList;
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        int randomIndex = Random.Range(0, m_spriteList.Length);
        m_spriteRenderer.sprite = m_spriteList[randomIndex];
    }
}
