using UnityEngine;
using UnityEngine.InputSystem;

public class ArmsController : MonoBehaviour
{
    private float rotation_x = 0;
    private float animation_speed =2.50f;
    private bool move_mode = false;
    public float look_speed = 0.50f;
    public float look_x_limit = 45.0f;

    private bool left_arm_ready_to_move = true;
    private string left_state = "center";
    private string left_direction = "center";
    private float left_timer = 0.0f;
    private float left_timer_cap =0.25f;

    private bool right_arm_ready_to_move = true;
    private string right_state = "center";
    private string right_direction = "center";
    private float right_timer = 0.0f;
    private float right_timer_cap = 0.25f;

    public Animator animator;
    private CharacterController character_controller;
    private Gamepad gamepad_controller;
    private GameObject main_camera;

    private void Start()
    {
        gamepad_controller = Gamepad.current;
        character_controller = GetComponent<CharacterController>();
        main_camera = GameObject.Find("Main Camera");
        animator.speed = animation_speed;
    }

    void Update()
    {
         var moveDirection = new Vector3(0, 0, 0);
        if (!character_controller.isGrounded)
        {
            moveDirection.y += Physics.gravity.y;
        }
        if (gamepad_controller != null)
        {
            if (gamepad_controller.buttonSouth.wasReleasedThisFrame)
            {
                _switch_modes();
            }
            if (move_mode)
            {
                _update_camera_rotation();
                moveDirection = _get_move_direction(moveDirection);
            }
            else
            {
                _update_left_stick();
                _update_right_stick();
                _update_left_hand();
                _update_right_hand();
                _update_left_arm();
                _update_right_arm();
            }
        }
        else
        {
            gamepad_controller = Gamepad.current;
        }
        character_controller.Move(moveDirection * Time.deltaTime);
    }

    private void _switch_modes()
    {
        rotation_x = 0;
        main_camera.transform.localRotation = Quaternion.Euler(rotation_x, 0, 0);
        _center_right_arm();
        animator.Play("reset", 3);
        animator.Play("reset", 4);
        _center_left_arm();
        move_mode = !move_mode;
    }

    private Vector3 _get_move_direction(Vector3 moveDirection)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        moveDirection += (forward * gamepad_controller.leftStick.value.y) + (right * gamepad_controller.leftStick.value.x);
        return moveDirection;
    }

    private void _update_camera_rotation()
    {
        var input_y = Mouse.current.delta.value.y + gamepad_controller.rightStick.value.y;
        var input_x = Mouse.current.delta.value.x + gamepad_controller.rightStick.value.x;
        rotation_x += -input_y * look_speed;
        rotation_x = Mathf.Clamp(rotation_x, -look_x_limit, look_x_limit);
        main_camera.transform.localRotation = Quaternion.Euler(rotation_x, 0, 0);
        transform.rotation *= Quaternion.Euler(0, input_x * look_speed, 0);
    }

    private void _update_left_hand()
    {
        if (gamepad_controller.leftShoulder.value > 0)
        {
            animator.Play("l-hand-1", 4);
        }
        else if (gamepad_controller.leftTrigger.value > 0)
        {
            animator.Play("l-hand-2", 4);
        }
        else
        {
            animator.Play("reset", 4);
        }
    }

    private void _update_right_hand()
    {
        if (gamepad_controller.rightShoulder.isPressed  )
        {
            animator.Play("r-hand-1", 3);
        }
        else if (gamepad_controller.rightTrigger.isPressed )
        {
            animator.Play("r-hand-2", 3);
        }
        else
        {
            animator.Play("reset", 3);
        }
    }

    private bool _should_center_left_arm()
    {
        return left_arm_ready_to_move == true &&  left_state !="center" && (left_direction == "center" || left_state!= left_direction) ;
    }

    private bool _should_animate_left_arm()
    {
        return left_state == "center" && left_direction != "center" && left_arm_ready_to_move == true;
    }

    private void _run_left_timer()
    {
        left_timer += Time.deltaTime;
        if (left_timer > left_timer_cap)
        {
            left_arm_ready_to_move = true;
            left_timer = 0.0f;
        }
    }

    private void _update_left_arm()
    {
        if (_should_center_left_arm())
        {
            left_arm_ready_to_move = false;
            Debug.Log("_should_center_left_arm == true");
            Debug.Log($"state | {left_state} | direction | {left_direction}");
            _center_left_arm();
        }
        else if (_should_animate_left_arm())
        {
            left_arm_ready_to_move = false;
            Debug.Log("_should_animate_left_arm == true");
            Debug.Log($"state | {left_state} | direction | {left_direction}");
            _animate_left_arm();
        }
        if (left_arm_ready_to_move == false)
        {
            _run_left_timer();
            Debug.Log("_should_run_left_timer == true");
        }
    }

    private bool _should_center_right_arm()
    {
        return right_arm_ready_to_move == true && right_state != "center" && (right_direction == "center" || right_state != right_direction);
    }

    private bool _should_animate_right_arm()
    {
        return right_state == "center" && right_direction != "center" && right_arm_ready_to_move == true;
    }

    private void _run_right_timer()
    {
        right_timer += Time.deltaTime;
        if (right_timer > right_timer_cap)
        {
            right_arm_ready_to_move = true;
            right_timer = 0.0f;
        }
    }

    private void _update_right_arm()
    {
        if (_should_center_right_arm())
        {
            right_arm_ready_to_move = false;
            Debug.Log("_should_center_right_arm == true");
            Debug.Log($"state | {right_state} | direction | {right_direction}");
            _center_right_arm();
        }
        else if (_should_animate_right_arm())
        {
            right_arm_ready_to_move = false;
            Debug.Log("_should_animate_right_arm == true");
            Debug.Log($"state | {right_state} | direction | {right_direction}");
            _animate_right_arm();
        }
        if (right_arm_ready_to_move == false)
        {
            _run_right_timer();
            Debug.Log("_should_run_right_timer == true");
        }
    }

    private void _update_left_stick()
    {

        if (gamepad_controller.leftStick.value.x > 0.5)
        {
            left_direction = "right";
        }
        else if (gamepad_controller.leftStick.value.x < -0.5)
        {
            left_direction = "left";
        }
        else if (gamepad_controller.leftStick.value.y < -0.5)
        {

            left_direction = "down";
        }
        else if (gamepad_controller.leftStick.value.y > 0.5)
        {
            left_direction = "up";
        }
        else
        {
            left_direction = "center";
        }
    }

    private void _animate_left_arm()
    {
        if (left_direction == "left")
        {
            left_state = "left";
            animator.Play("l-l", 2);
        }
        if (left_direction == "right")
        {
            left_state = "right";
            animator.Play("l-r", 2);
        }
        if (left_direction == "up")
        {
            left_state = "up";
            animator.Play("l-u", 2);
        }
        if (left_direction == "down")
        {
            left_state = "down";
            animator.Play("l-d", 2);
        }
    }

    private void _center_left_arm()
    {
        if (left_state == "left")
        {
            animator.Play("l-l-t", 2);
        }
        if (left_state == "right")
        {
            animator.Play("l-r-t", 2);
        }
        if (left_state == "up")
        {
            animator.Play("l-u-t", 2);
        }
        if (left_state == "down")
        {
            animator.Play("l-d-t", 2);
        }
        left_state = "center";
    }
  
    private void _update_right_stick()
    {
        if (gamepad_controller.rightStick.value.x > 0.5f)
        {
            right_direction = "right";
        }
        else if (gamepad_controller.rightStick.value.x < -0.5f)
        {
            right_direction = "left";
        }
        else if (gamepad_controller.rightStick.value.y < -0.5f)
        {
            right_direction = "down";
        }
        else if (gamepad_controller.rightStick.value.y > 0.5f)
        {
            right_direction = "up";
        }
        else
        {
            right_direction = "center";
        }
    }

    private void _animate_right_arm()
    {
        if (right_direction == "left")
        {
            right_state = "left";
            animator.Play("r-l", 1);
        }
        if (right_direction == "right")
        {
            right_state = "right";
            animator.Play("r-r", 1);
        }
        if (right_direction == "up")
        {
            right_state = "up";
            animator.Play("r-u", 1);
        }
        if (right_direction == "down")
        {
            right_state = "down";
            animator.Play("r-d", 1);
        }
    }

    private void _center_right_arm()
    {
        if (right_state == "left")
        {
            animator.Play("r-l-t", 1);
        }
        if (right_state == "right")
        {
            animator.Play("r-r-t", 1);
        }
        if (right_state == "up")
        {
            animator.Play("r-u-t", 1);
        }
        if (right_state == "down")
        {
            animator.Play("r-d-t", 1);
        }
        right_state = "center";
    }
}
