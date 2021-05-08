using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_animator;

    [SerializeField]
    private AnimationClip digClip;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OnStartedMoving()
    {
        m_animator.SetBool("IsMoving", true);
    }

    public void OnStoppedMoving()
    {
        m_animator.SetBool("IsMoving", false);
    }

    public void Dig()
    {
        m_animator.SetTrigger("Dig");
    }

    public float GetDigAnimationLength()
    {
        return digClip.length;
    }

    public void OnFall()
    {
        m_animator.SetTrigger("Fall");
    }

    public void OnLand()
    {
        m_animator.SetTrigger("Land");
    }
}