using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ArmsControllerSymbols
{
    Up, Down, Left, Right, Gun, Peace, Default, Love
}

public class ArmsController : MonoBehaviour
{
    public LayerMask EnvironmentLayerMask;
    public LayerMask DoorTriggerLayerMask;
    public LayerMask PickupTriggerLayerMask;
    private float rotation_x = 0;
    private float animation_speed = 2.50f;
    private bool move_mode = true;
    public float look_speed = 0.750f;
    public float look_x_limit = 45.0f;

    private bool left_arm_ready_to_move = true;
    private string left_state = "center";
    private string left_direction = "center";
    private float left_timer = 0.0f;
    private float left_timer_cap = 0.25f;

    private bool right_arm_ready_to_move = true;
    private string right_state = "center";
    private string right_direction = "center";
    private float right_timer = 0.0f;
    private float right_timer_cap = 0.25f;

    private GameObject left_hand_target;
    private GameObject right_hand_target;
    public GameObject left_hand_raycast_origin;
    public GameObject right_hand_raycast_origin;
    private GameObject left_hand_hint;
    private GameObject right_hand_hint;

    private Animator animator;
    private CharacterController character_controller;
    private Gamepad gamepad_controller;
    private GameObject main_camera;

 
    private MeshRenderer player_hammer;

    private bool in_magic_square = false;
    private bool in_pickup_trigger = false;
    private bool looking_at_pickup_trigger = false;
    private bool holding_hammer = false;
    private float pickup_min_distance = 2.0f;
    private Pickup current_pickup_in_focus;
    private bool pickup_close_enough;
    private float hammer_pickup_grab_time = 0.5f;
    private float hammer_pickup_grab_time_left = 0.5f;
    private float move_speed = 200.5f;

    private void Start()
    {
        gamepad_controller = Gamepad.current;
        animator = GameObject.Find("arm-rig").GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        main_camera = GameObject.Find("Main Camera");
        animator.speed = animation_speed;
        player_hammer = GameObject.Find("player-hammer").GetComponent<MeshRenderer>();
        _init_rig();
 
    }

    void Update()
    {
        var moveDirection = new Vector3(0, 0, 0);

        moveDirection = _apply_gravity(moveDirection);

        _update_pickup_process();

        _update_raycast_for_walls();

        if (gamepad_controller != null)
        {
            _look_for_pickups();

            if (_should_pickup_hammer())
            {
                StartCoroutine(_pickup_hammer());
            }
            if (gamepad_controller.buttonSouth.wasReleasedThisFrame && in_magic_square)
            {
                _switch_modes();
            }
            if (move_mode)
            {
                _update_camera_rotation();
                moveDirection = _get_move_direction(moveDirection);
                moveDirection *= move_speed * Time.deltaTime;
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

    private void _update_raycast_for_walls()
    {
        point_in_front_of_left_hand = _raycast_in_front_of_hand(left_hand_raycast_origin);
        point_in_front_of_right_hand = _raycast_in_front_of_hand(right_hand_raycast_origin);
        something_in_front_of_left_hand = point_in_front_of_left_hand != null;
        something_in_front_of_right_hand = point_in_front_of_right_hand != null;
        if (something_in_front_of_left_hand)
        {
            animator.Play("l-t-pose", 0);
            left_hand_target.transform.position = point_in_front_of_left_hand.Value;
        }
       else if (something_in_front_of_right_hand)
        {
            animator.Play("r-t-pose", 0);
            right_hand_target.transform.position = point_in_front_of_right_hand.Value;
        }

    }
    private bool something_in_front_of_left_hand;
    private bool something_in_front_of_right_hand;
    private Vector3? point_in_front_of_left_hand;
    private Vector3? point_in_front_of_right_hand;
    private Vector3? _raycast_in_front_of_hand(GameObject origin)
    {
        var RaycastHit = new RaycastHit();
        if (Physics.Raycast(origin.transform.position, origin.transform.forward, out RaycastHit, 1.0f, EnvironmentLayerMask))
        {
            return RaycastHit.point;
        }
        return null;
    }

    private void _update_pickup_process()
    {
        if (animating_pickup && current_pickup_in_focus != null)
        {
            hammer_pickup_grab_time_left -= Time.deltaTime;
            var delta = hammer_pickup_grab_time_left / hammer_pickup_grab_time;
            delta = 1.0f - delta;
            right_hand_target.transform.position = Vector3.Lerp(right_hand_target.transform.position, current_pickup_in_focus.transform.position, delta);
        }
        else if (animating_pickup && current_pickup_in_focus != null)
        {
            hammer_pickup_grab_time_left = hammer_pickup_grab_time;
            animating_pickup = false;
        }
    }

    private Vector3 _apply_gravity(Vector3 moveDirection)
    {
        if (!character_controller.isGrounded)
        {
            moveDirection.y += Physics.gravity.y;
        }

        return moveDirection;
    }

    private bool _should_pickup_hammer()
    {
        return pickup_close_enough && move_mode && looking_at_pickup_trigger && gamepad_controller.buttonEast.wasReleasedThisFrame && holding_hammer == false;
    }
    
    private void _init_rig()
    {
        left_hand_target = GameObject.Find("LeftArmIK_target");
        right_hand_target = GameObject.Find("RightArmIK_target");
        left_hand_hint = GameObject.Find("LeftArmIK_hint");
        right_hand_hint = GameObject.Find("RightArmIK_hint");
    }

    private bool animating_pickup = false;

    private IEnumerator _pickup_hammer()
    {
        Debug.Log("Pickup Hammer");
        animator.Play("r-t-pose", 0);
        animator.Play("r-hand-hammer-swing", 5);

        animating_pickup = true;
        yield return new WaitForSeconds(hammer_pickup_grab_time);
        if(current_pickup_in_focus == null)
        {
            animating_pickup = false;   
        }
        else
        {
            current_pickup_in_focus._on_pickup();
            holding_hammer = true;
            animating_pickup = false;
            animator.Play("t-pose", 0);
            player_hammer.enabled = true;
            Debug.Log("Pickup Hammer - DONE");
        }
    }


    private void _look_for_pickups()
    {
        var hit = new RaycastHit();
        if(Physics.Raycast(transform.position, main_camera.transform.forward, out hit, 9999.0f, PickupTriggerLayerMask))
        {
            if(Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) < pickup_min_distance)
            {
                pickup_close_enough = true;
                current_pickup_in_focus = hit.collider.gameObject.GetComponent<Pickup>();
                current_pickup_in_focus._on_look_at();
                looking_at_pickup_trigger = true;
                return;
            }
  
        }
        if(current_pickup_in_focus != null)
        {
            current_pickup_in_focus._on_look_away();
            current_pickup_in_focus = null;
        }
        pickup_close_enough = false;
        looking_at_pickup_trigger = false;
    }

    private void _switch_modes()
    {
        Debug.Log("_switch_modes");

        rotation_x = 0;
       // main_camera.transform.localRotation = Quaternion.Euler(rotation_x, 0, 0);
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
        if (gamepad_controller.leftShoulder.value > 0.0 && gamepad_controller.leftTrigger.value == 0.0)
        {
            animator.Play("l-hand-1", 4);
        }
        else if (gamepad_controller.leftTrigger.value > 0 && gamepad_controller.leftShoulder.value == 0.0)
        {
            animator.Play("l-hand-2", 4);
        }
        else if (gamepad_controller.leftTrigger.value > 0 && gamepad_controller.leftShoulder.value > 0.0)
        {
            animator.Play("l-hand-3", 4);
        }
        else
        {
            animator.Play("reset", 4);
        }
    }

    private void _update_right_hand()
    {
        if (gamepad_controller.rightShoulder.value > 0.0 && gamepad_controller.rightTrigger.value == 0.0)
        {
            animator.Play("r-hand-1", 3);
        }
        else if (gamepad_controller.rightTrigger.value > 0 && gamepad_controller.rightShoulder.value == 0.0)
        {
            animator.Play("r-hand-2", 3);
        }
        else if (gamepad_controller.rightTrigger.value > 0 && gamepad_controller.rightShoulder.value > 0.0)
        {
            animator.Play("r-hand-3", 3);
        }
        else
        {
            animator.Play("reset", 3);
        }
    }

    private bool _should_center_left_arm()
    {
        return left_arm_ready_to_move == true && left_state != "center" && (left_direction == "center" || left_state != left_direction);
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

            _center_left_arm();
        }
        else if (_should_animate_left_arm())
        {
            left_arm_ready_to_move = false;
            _animate_left_arm();
        }
        if (left_arm_ready_to_move == false)
        {
            _run_left_timer();
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
            _center_right_arm();
        }
        else if (_should_animate_right_arm())
        {
            right_arm_ready_to_move = false;
            _animate_right_arm();
        }
        if (right_arm_ready_to_move == false)
        {
            _run_right_timer();
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

    public void _on_enter_magic_square(GameObject other)
    {
        Debug.Log("_OnTriggerEnter");
        in_magic_square = true;

    }

    public void _on_exit_magic_square(GameObject other)
    {
        Debug.Log("_OnTriggerExit");
        in_magic_square = false;
    }

    public void _on_enter_pickup(Pickup pickup)
    {
        in_pickup_trigger = true;
    }

    public void _on_exit_pickup(Pickup pickup)
    {
        in_pickup_trigger = false;
    }
}
