using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmsController : MonoBehaviour
{
    public Animator AnimationController;
    private string left_state = "center";
    private string right_state = "center";
    private float left_timer = 0.0f;
    private float right_timer = 0.0f;
    private float left_timer_cap =0.25f;
    private float animation_speed =2.50f;
    private float right_timer_cap = 0.25f;
    private bool left_arm_ready_to_move = true;
    private bool right_arm_ready_to_move = true;
    private string left_direction = "center";
    private string right_direction = "center";
    private Gamepad controller;
    private void Start()
    {
        controller = Gamepad.current;
        AnimationController.speed = animation_speed;
    }
    void Update()
    {
        _update_left_stick();
        _update_right_stick();
        if (controller != null)
        {
            _process_left_arm();
            _process_right_arm();
        }
        else
        {
            controller = Gamepad.current;
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

    private void _process_left_arm()
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

    private void _process_right_arm()
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
        bool left_stick_changed;
        if (controller.leftStick.value.x > 0.5)
        {
            left_stick_changed = left_direction != "right";
            left_direction = "right";
        }
        else if (controller.leftStick.value.x < -0.5)
        {
            left_stick_changed = left_direction != "left";
            left_direction = "left";
        }
        else if (controller.leftStick.value.y < -0.5)
        {

            left_stick_changed = left_direction != "down";
            left_direction = "down";
        }
        else if (controller.leftStick.value.y > 0.5)
        {
            left_stick_changed = left_direction != "up";
            left_direction = "up";
        }
        else
        {
            left_stick_changed = true;
            left_direction = "center";
        }
    }

    private void _animate_left_arm()
    {
        if (left_direction == "left")
        {
            left_state = "left";
            AnimationController.Play("l-l", 2);
        }
        if (left_direction == "right")
        {
            left_state = "right";
            AnimationController.Play("l-r", 2);
        }
        if (left_direction == "up")
        {
            left_state = "up";
            AnimationController.Play("l-u", 2);
        }
        if (left_direction == "down")
        {
            left_state = "down";
            AnimationController.Play("l-d", 2);
        }
    }

    private void _center_left_arm()
    {
        if (left_state == "left")
        {
            AnimationController.Play("l-l-t", 2);
        }
        if (left_state == "right")
        {
            AnimationController.Play("l-r-t", 2);
        }
        if (left_state == "up")
        {
            AnimationController.Play("l-u-t", 2);
        }
        if (left_state == "down")
        {
            AnimationController.Play("l-d-t", 2);
        }
        left_state = "center";
    }
  
    private void _update_right_stick()
    {
        bool right_stick_changed;
        if (controller.rightStick.value.x > 0.5f)
        {
            right_stick_changed = right_direction != "right";
            right_direction = "right";
        }
        else if (controller.rightStick.value.x < -0.5f)
        {
            right_stick_changed = right_direction != "left";
            right_direction = "left";
        }
        else if (controller.rightStick.value.y < -0.5f)
        {
            right_stick_changed = right_direction != "down";
            right_direction = "down";
        }
        else if (controller.rightStick.value.y > 0.5f)
        {
            right_stick_changed = right_direction != "up";
            right_direction = "up";
        }
        else
        {
            right_stick_changed = true;
            right_direction = "center";
        }
    }

    private void _animate_right_arm()
    {
        if (right_direction == "left")
        {
            right_state = "left";
            AnimationController.Play("r-l", 1);
        }
        if (right_direction == "right")
        {
            right_state = "right";
            AnimationController.Play("r-r", 1);
        }
        if (right_direction == "up")
        {
            right_state = "up";
            AnimationController.Play("r-u", 1);
        }
        if (right_direction == "down")
        {
            right_state = "down";
            AnimationController.Play("r-d", 1);
        }
    }

    private void _center_right_arm()
    {
        if (right_state == "left")
        {
            AnimationController.Play("r-l-t", 1);
        }
        if (right_state == "right")
        {
            AnimationController.Play("r-r-t", 1);
        }
        if (right_state == "up")
        {
            AnimationController.Play("r-u-t", 1);
        }
        if (right_state == "down")
        {
            AnimationController.Play("r-d-t", 1);
        }
        right_state = "center";
    }
}
