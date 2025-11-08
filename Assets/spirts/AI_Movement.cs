using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class AI_Movement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    public float moveSpeed = 1.5f;      // tăng chút để dễ thấy
    public float turnSpeed = 360f;      // quay mượt về hướng đi (deg/s)

    private float walkTime;
    private float walkCounter;
    private float waitTime;
    private float waitCounter;

    private int walkDirection;          // 0:FWD 1:RIGHT 2:LEFT 3:BACK
    private bool isWalking;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Để physics ổn định
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;           // dùng physics để di chuyển
        rb.useGravity = true;

        walkTime = Random.Range(3f, 6f);
        waitTime = Random.Range(2f, 4f);
        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection();
    }

    void FixedUpdate()   // di chuyển bằng physics ở FixedUpdate
    {
        if (isWalking)
        {
            animator.SetBool("isRunning", true);
            walkCounter -= Time.fixedDeltaTime;

            // Chọn hướng theo WORLD, rồi quay mặt về hướng đó
            Vector3 dir = Vector3.zero;
            switch (walkDirection)
            {
                case 0: dir = Vector3.forward; break;
                case 1: dir = Vector3.right; break;
                case 2: dir = Vector3.left; break;
                case 3: dir = Vector3.back; break;
            }

            // Quay dần theo hướng
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            }

            // Di chuyển
            Vector3 nextPos = rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);

            if (walkCounter <= 0f)
            {
                isWalking = false;
                animator.SetBool("isRunning", false);
                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.fixedDeltaTime;
            if (waitCounter <= 0f)
                ChooseDirection();
        }
    }

    void ChooseDirection()
    {
        walkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }
}


