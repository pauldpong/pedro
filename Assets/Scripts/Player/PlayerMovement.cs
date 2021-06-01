using System.Collections;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerMovement : MonoBehaviour
{
    private bool m_AttemptingMove = false;
    private bool m_IsFacingRight = true;
    private bool m_Grounded = true;

    /* Events */
    [SerializeField]
    private UnityEvent StartedMovingEvent;
    [SerializeField]
    private UnityEvent StoppedMovingEvent;
    [SerializeField]
    private UnityEvent FallEvent;
    [SerializeField]
    private UnityEvent LandEvent;
    [SerializeField]
    private UnityEvent OrientationFlippedEvent;

    /* Parameters */
    [SerializeField]
    private Transform m_GroundCheck;
    [SerializeField]
    private float m_TimeToMove = 0.2f;
    [SerializeField]
    private LayerMask m_CollisionLayer;

    /* Constants */
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
        if (m_AttemptingMove) yield break;

        // No air movement
        if (!m_Grounded && direction != Vector2.down) yield break;

        m_AttemptingMove = true;

        CheckAndSendOrientationEvent(direction);

        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = originalPosition + direction;

        // Check if anything blocking player
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, k_CollisionRadius, m_CollisionLayer);
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
                m_AttemptingMove = false;
                yield break;
            }
        }

        StartedMovingEvent.Invoke();

        float elapsedTime = 0.0f;
        while (elapsedTime < m_TimeToMove)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / m_TimeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure player is at the target position (while loop can terminate just before we reach exact location)
        transform.position = targetPosition;

        // Ground detection
        bool wasAirborne = !m_Grounded;
        m_Grounded = false;
        Collider2D groundCollider = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_CollisionLayer);
        if (groundCollider != null)
        {
            m_Grounded = true;
        }

        if (!wasAirborne && !m_Grounded)
        {
            FallEvent.Invoke();
        }
        else if (wasAirborne && m_Grounded)
        {
            LandEvent.Invoke();
        }

        StoppedMovingEvent.Invoke();
        m_AttemptingMove = false;
    }

    private void CheckAndSendOrientationEvent(Vector2 direction)
    {
        if (direction == Vector2.left && m_IsFacingRight)
        {
            m_IsFacingRight = false;
            OrientationFlippedEvent.Invoke();
        }
        if (direction == Vector2.right && !m_IsFacingRight)
        {
            m_IsFacingRight = true;
            OrientationFlippedEvent.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (!m_Grounded)
        {
            StartCoroutine(MovePlayer(Vector2.down));
        }
    }
}
