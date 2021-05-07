using System.Collections;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerMovement : MonoBehaviour
{
    private bool m_attemptingMove = false;
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

        CheckAndSendOrientationEvent(direction);

        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = originalPosition + direction;

        // Check if anything blocking player
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, 0.25f, m_collisionLayer);
        if (collider != null)
        {
            // Play dig animation
            PlayerAnimation playerAnimation = GetComponent<PlayerAnimation>();
            if (playerAnimation != null)
            {
                playerAnimation.Dig();
            }

            // Let dig animation play until pick hits the block
            yield return new WaitForSeconds(playerAnimation.GetDigAnimationLength() / 2);

            IBreakable breakable = collider.gameObject.GetComponent<IBreakable>();
            if (breakable != null)
            {
                breakable.OnHit();
            }
            else
            {
                // Unbreakable gameObject, break out of coroutine
                m_attemptingMove = false;
                yield break;
            }
        }

        startedMovingEvent.Invoke();

        float elapsedTime = 0.0f;
        while (elapsedTime < m_timeToMove)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / m_timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure player is at the target position (while loop can terminate just before we reach exact location)
        transform.position = targetPosition;

        stoppedMovingEvent.Invoke();
        m_attemptingMove = false;
    }

    private void CheckAndSendOrientationEvent(Vector2 direction)
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
}
