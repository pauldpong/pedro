using System.Collections;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerMovement : MonoBehaviour
{
    private bool m_attemptingMove = false;
    private Vector2 m_originalPosition, m_targetPosition;
    private bool m_isFacingRight = true;

    [SerializeField]
    private UnityEvent startedMovingEvent;
    [SerializeField]
    private UnityEvent stoppedMovingEvent;
    [SerializeField]
    private UnityEvent orientationFlippedEvent;

    [SerializeField]
    private float m_timeToMove = 0.2f;
    [SerializeField]
    private LayerMask m_collisionLayer;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            StartCoroutine(MovePlayer(Vector2.left));
        }
        if (Input.GetKey(KeyCode.D))
        {
            StartCoroutine(MovePlayer(Vector2.right));
        }
        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(MovePlayer(Vector2.down));
        }
    }

    private IEnumerator MovePlayer(Vector2 direction)
    {
        if (m_attemptingMove) yield break;

        m_attemptingMove = true;

        //TODO yield break when cant move need to reset attempingMove bool
        CheckAndSendOrientation(direction);
        if (!CanMove(direction)) yield break;

        startedMovingEvent.Invoke();

        float elapsedTime = 0.0f;
        m_originalPosition = transform.position;
        m_targetPosition = m_originalPosition + direction;

        while (elapsedTime < m_timeToMove)
        {
            transform.position = Vector3.Lerp(m_originalPosition, m_targetPosition, (elapsedTime / m_timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = m_targetPosition;

        stoppedMovingEvent.Invoke();
        m_attemptingMove = false;
    }

    private void CheckAndSendOrientation(Vector2 direction)
    {
        if (direction == Vector2.left && m_isFacingRight)
        {
            m_isFacingRight = false;
            orientationFlippedEvent.Invoke();
        }
        if (direction == Vector2.right && !m_isFacingRight)
        {
            m_isFacingRight = true;
            orientationFlippedEvent.Invoke();
        }
    }

    private bool CanMove(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, 0.25f, m_collisionLayer);
        if (collider != null)
        {
            // Play dig animation
            PlayerAnimation playerAnimation = GetComponent<PlayerAnimation>();
            if (playerAnimation != null)
            {
                playerAnimation.Dig();
            }

            IBreakable breakable = collider.gameObject.GetComponent<IBreakable>();
            if (breakable != null)
            {
                breakable.OnHit();
            }

            return false;
        }
        else
        {
            return true;
        }
    }
}
