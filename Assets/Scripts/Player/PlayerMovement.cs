using System.Collections;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerMovement : MonoBehaviour
{
    private bool m_attemptingMove = false;
    private bool m_isFacingRight = true;
    private bool m_grounded = true;

    [SerializeField]
    private UnityEvent startedMovingEvent;
    [SerializeField]
    private UnityEvent stoppedMovingEvent;
    [SerializeField]
    private UnityEvent fallEvent;
    [SerializeField]
    private UnityEvent landEvent;
    [SerializeField]
    private UnityEvent orientationFlippedEvent;
    [SerializeField]
    private Transform m_groundCheck;

    [SerializeField]
    private float m_timeToMove = 0.2f;
    [SerializeField]
    private LayerMask m_collisionLayer;

    const float k_GroundedRadius = 0.25f;
    const float k_CollisionRadius = 0.25f;

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
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, k_CollisionRadius, m_collisionLayer);
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

        bool wasAirborne = !m_grounded;
        m_grounded = false;
        Collider2D groundCollider = Physics2D.OverlapCircle(m_groundCheck.position, k_GroundedRadius, m_collisionLayer);
        if (groundCollider != null)
        {
            m_grounded = true;
        }

        if (!wasAirborne && !m_grounded)
        {
            fallEvent.Invoke();
        }
        else if (wasAirborne && m_grounded)
        {
            landEvent.Invoke();
        }

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

    private void FixedUpdate()
    {
        if (!m_grounded)
        {
            StartCoroutine(MovePlayer(Vector2.down));
        }
    }
}
