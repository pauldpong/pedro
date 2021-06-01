using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_Animator;

    [SerializeField]
    private AnimationClip m_DigClip;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void OnStartedMoving()
    {
        m_Animator.SetBool("IsMoving", true);
    }

    public void OnStoppedMoving()
    {
        m_Animator.SetBool("IsMoving", false);
    }

    public void Dig()
    {
        m_Animator.SetTrigger("Dig");
    }

    public float GetDigAnimationLength()
    {
        return m_DigClip.length;
    }

    public void OnFall()
    {
        m_Animator.SetTrigger("Fall");
    }

    public void OnLand()
    {
        m_Animator.SetTrigger("Land");
    }
}