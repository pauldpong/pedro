using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool m_isMoving = false;
    private Vector3 m_originalPosition, m_targetPosition;
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
        if (m_isMoving) return;

        if (Input.GetKey(KeyCode.A))
        {
            StartCoroutine(MovePlayer(Vector3.left));
        }
        if (Input.GetKey(KeyCode.D))
        {
            StartCoroutine(MovePlayer(Vector3.right));
        }
        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(MovePlayer(Vector3.down));
        }
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        if (!canMove(direction)) yield break;

        m_isMoving = true;
        startedMovingEvent.Invoke();

        // Check & send orientation changed event
        if (direction == Vector3.left && m_isFacingRight)
        {
            m_isFacingRight = false;
            orientationFlippedEvent.Invoke();
        }
        if (direction == Vector3.right && !m_isFacingRight)
        {
            m_isFacingRight = true;
            orientationFlippedEvent.Invoke();
        }

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

        m_isMoving = false;
        stoppedMovingEvent.Invoke();
    }

    private bool canMove(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, 0.25f, m_collisionLayer);
        if (collider)
        {
            //TODO Continue doing block collision & dig animations
            return false;
        }
        
        return true;
    }
}
