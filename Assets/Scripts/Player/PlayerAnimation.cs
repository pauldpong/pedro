using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OnStartedMoving()
    {
        m_animator.SetBool("isMoving", true);
    }

    public void OnStoppedMoving()
    {
        m_animator.SetBool("isMoving", false);
    }
}