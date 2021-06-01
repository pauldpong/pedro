using UnityEngine;

public class BlockRandomVariation : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_SpriteList;
    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        int randomIndex = Random.Range(0, m_SpriteList.Length);
        m_SpriteRenderer.sprite = m_SpriteList[randomIndex];
    }
}
