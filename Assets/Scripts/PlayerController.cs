using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 플레이어 컨트롤러
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Vector2 movementInput;

    [SerializeField] private int money;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        money = 9999999;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Normalize to prevent faster diagonal movement
        movementInput.Normalize();
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}
