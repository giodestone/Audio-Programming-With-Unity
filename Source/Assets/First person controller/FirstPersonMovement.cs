using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;
    Vector2 velocity;

    /// <summary>
    /// Whether the user is giving input.
    /// </summary>
    /// <value></value>
    public bool IsMoving { get; private set; } // Added by me!


    void FixedUpdate()
    {
        velocity.y = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        velocity.x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(velocity.x, 0, velocity.y);
        IsMoving = velocity.sqrMagnitude > 0; // Added by Me!
    }
}
