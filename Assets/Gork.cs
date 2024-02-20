using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class Gork : MonoBehaviour
{
    public Animator animator;
    public LayerMask EnvironmentLayerMask;
    public GameObject raycast_rig;
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
    private float leg_speed =1.0f;
    private float leg_acceleration =1.0f;
    private float current_speed = 0.0f;
    private float rig_distance_speed_ratio = 1.0f;
    private Vector3 last_position = Vector3.zero;


    private void Start()
    {
        right_leg_last_position = right_leg_IK_target.transform.position;
        left_leg_last_position = left_leg_IK_target.transform.position;

        animator = gameObject.GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        var dist = Vector3.Distance(transform.position, last_position);
        last_position = transform.position;
        current_speed = dist / Time.fixedDeltaTime;
        var rig_distance = current_speed * rig_distance_speed_ratio;
        raycast_rig.transform.localPosition = new Vector3(-rig_distance, 0, 0);
        
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
            return hit.point;
        }
        return raycast_origin.transform.position;
    }

    private void _lerp_left_leg()
    {
        var pos1 = left_leg_last_position;
        var pos2 = left_leg_raycast_hit;
        left_leg_IK_target.transform.position = (_lerp_xz(pos1, pos2));
    }

    private void _lerp_right_leg()
    {
        var pos1 = right_leg_last_position;
        var pos2 = right_leg_raycast_hit;
        right_leg_IK_target.transform.position = (_lerp_xz(pos1, pos2));
    }

    private Vector3 _lerp_y(Vector3 pos1)
    {
        var y = -(Mathf.Pow((time_spent_lerping - leg_speed), 2)) + leg_speed;
        var target_pos = pos1;

        target_pos.y += y;
        return target_pos;
    }

    private Vector3 _lerp_xz(Vector3 pos1, Vector3 pos2)
    {
        var lerp_amount = leg_acceleration* time_spent_lerping / leg_speed;
        return Vector3.Lerp(pos1, pos2, EaseOut(lerp_amount));
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
