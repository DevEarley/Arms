using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
//[ExecuteInEditMode]
public class Gork : MonoBehaviour
{
    public Animator animator;
    public Animator bob_animator;
    public LayerMask EnvironmentLayerMask;
    public GameObject raycast_rig;
    public GameObject gork_rig;
    public GameObject left_leg_raycast_origin;
    public GameObject right_leg_raycast_origin;
    public GameObject left_leg_IK_target;
    public GameObject right_leg_IK_target;
    public GameObject left_leg_marker;
    public GameObject right_leg_marker;
    private Vector3 left_leg_raycast_hit;
    private Vector3 right_leg_raycast_hit;
    private Vector3 left_leg_last_position;
    private Vector3 right_leg_last_position;
    private bool left_leg_moving;
    private float time_spent_lerping = 0.0f;
    private float leg_speed = 0.0f;
    private float current_speed = 0.0f;
    private float rig_distance_speed_ratio = 0.5f;
    private Vector3 last_position = Vector3.zero;
    private float foot_offset = 0.05f;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        right_leg_last_position = right_leg_IK_target.transform.position;
        left_leg_last_position = left_leg_IK_target.transform.position;
        last_position = gork_rig.transform.position;
        animator = gameObject.GetComponent<Animator>();
        bob_animator = gork_rig.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        var dist_vect = (gork_rig.transform.position - last_position);
        var dist = dist_vect.magnitude;
        velocity = dist_vect / Time.deltaTime;
        current_speed = (dist / Time.deltaTime);
        //TODO figure out how to detect if they are backing up and move the rig behind them.
        raycast_rig.transform.localPosition = new Vector3(-current_speed * rig_distance_speed_ratio, 0, 0);
        last_position = gork_rig.transform.position;
        bob_animator.speed = Mathf.Clamp01(current_speed * 40.0f) * 2.0f;
        leg_speed = Mathf.Min(0.5f,Mathf.Max(1.0f/(current_speed*10.0f), 0.2f));
        if(leg_speed == 0.1f)
        {
            leg_speed = 0.0f;
        }
    }
    private void Update()
    {


        time_spent_lerping += Time.deltaTime;
        if (time_spent_lerping > leg_speed)
        {

            time_spent_lerping = 0;
            if (left_leg_moving)
            {
                right_leg_last_position = right_leg_IK_target.transform.position;
                _get_next_right_leg_postion();
                left_leg_moving = false;
            }
            else
            {
                left_leg_last_position = left_leg_IK_target.transform.position;
                _get_next_left_leg_postion();
                left_leg_moving = true;
            }

        }
        if (left_leg_moving)
        {
            _lerp_left_leg();
        }
        else
        {
            _lerp_right_leg();
        }
    }

    private void _get_next_left_leg_postion()
    {
        left_leg_raycast_hit = _get_next_leg_position(left_leg_raycast_origin);
        left_leg_marker.transform.position = left_leg_raycast_hit;
    }

    private void _get_next_right_leg_postion()
    {
        right_leg_raycast_hit = _get_next_leg_position(right_leg_raycast_origin);
        right_leg_marker.transform.position = right_leg_raycast_hit;

    }


    private Vector3 _get_next_leg_position(GameObject raycast_origin)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(raycast_origin.transform.position, Vector3.down, out hit, 9999.0f, EnvironmentLayerMask))
        {
            var vect = hit.point;
            vect.y += foot_offset;
            return vect;
        }
        return raycast_origin.transform.position;
    }

    private void _lerp_left_leg()
    {
        var pos1 = left_leg_last_position;
        var pos2 = left_leg_raycast_hit;
        var new_pos = _lerp_y(_lerp_xz(pos1, pos2));
        if(new_pos.x != float.NaN)
         left_leg_IK_target.transform.position = new_pos;
    }

    private void _lerp_right_leg()
    {
        var pos1 = right_leg_last_position;
        var pos2 = right_leg_raycast_hit;
        var new_pos = _lerp_y(_lerp_xz(pos1, pos2));
        if (new_pos.x != float.NaN)
            right_leg_IK_target.transform.position = new_pos;
    }

    private float _lerp_y_offset = 0.08f;
    private float _lerp_y_scale = 0.3f;

    private Vector3 _lerp_y(Vector3 pos)
    {
        //GOOD 3D plot  [- ((2*n* x - (n))^2) + n^2 ]  x = 0 to 1, n = 0.0 to 2.0
        // -(2 x - 1)^2 + 1 
        // x = (2 * lerp_amount -1)
        // y = -(x)^2 + 1
        if (leg_speed == 0) return pos;
        var lerp_amount = Mathf.Clamp01( time_spent_lerping / leg_speed);
        float x = (2.0f * current_speed * lerp_amount) - (current_speed);
        float y = -1.0f * Mathf.Pow(x, 2.0f) + Mathf.Pow(current_speed, 2.0f);
        y += _lerp_y_offset;
        y *= _lerp_y_scale;
        var target_pos = new Vector3(pos.x, pos.y + y, pos.z);
        return target_pos;
    }

    private Vector3 _lerp_xz(Vector3 pos1, Vector3 pos2)
    {
  
        if (leg_speed == 0) return pos1;

        var lerp_amount =  time_spent_lerping / leg_speed;
        return Vector3.Lerp(pos1, pos2, EaseInOut(lerp_amount));
    }

    static float Flip(float x)
    {
        return 1 - x;
    }

    public static float EaseIn(float t)
    {
        return t * t;
    }
    public static float EaseOut(float t)
    {
        return Flip(EaseIn(Flip(t)));
    }

    public static float EaseInOut(float t)
    {
        return Lerp(EaseIn(t), EaseOut(t), t);
    }
    static float Lerp(float start_value, float end_value, float pct)
    {
        return (start_value + (end_value - start_value) * Mathf.Clamp01(pct));
    }

}
