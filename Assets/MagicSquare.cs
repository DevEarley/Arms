using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum MagicSquareDoor
{
    Left
}
public class MagicSquare : MonoBehaviour
{
    public MagicSquareDoor Door;
    private ArmsController Player;

    private GameObject left_side;
    private GameObject right_side;

    private GameObject left_door_instructions;
    public Material symbol_up_mat;
    public Material symbol_down_mat;
    public Material symbol_left_mat;
    public Material symbol_right_mat;
    public Material hand_symbol_peace_mat;
    public Material hand_symbol_love_mat;
    public Material hand_symbol_gun_mat;
    public Material hand_symbol_default_mat;
    private MeshRenderer left_door_top_directional_symbol_quad;
    private MeshRenderer left_door_middle_directional_symbol_quad;
    private MeshRenderer left_door_bottom_directional_symbol_quad;

    private MeshRenderer left_door_top_hand_symbol_quad;
    private MeshRenderer left_door_middle_hand_symbol_quad;
    private MeshRenderer left_door_bottom_hand_symbol_quad;

    private MeshRenderer left_door_top_glow_mesh;
    private MeshRenderer left_door_middle_glow_mesh;
    private MeshRenderer left_door_bottom_glow_mesh;

    private MeshRenderer right_door_top_directional_symbol_quad;
    private MeshRenderer right_door_middle_directional_symbol_quad;
    private MeshRenderer right_door_bottom_directional_symbol_quad;

    private MeshRenderer right_door_top_hand_symbol_quad;
    private MeshRenderer right_door_middle_hand_symbol_quad;
    private MeshRenderer right_door_bottom_hand_symbol_quad;

    private MeshRenderer right_door_top_glow_mesh;
    private MeshRenderer right_door_middle_glow_mesh;
    private MeshRenderer right_door_bottom_glow_mesh;

    private ArmsControllerSymbols left_door_top_directional_symbol;
    private ArmsControllerSymbols left_door_middle_directional_symbol;
    private ArmsControllerSymbols left_door_bottom_directional_symbol;

    private ArmsControllerSymbols right_door_top_directional_symbol;
    private ArmsControllerSymbols right_door_middle_directional_symbol;
    private ArmsControllerSymbols right_door_bottom_directional_symbol;

    private ArmsControllerSymbols left_door_top_hand_symbol;
    private ArmsControllerSymbols left_door_middle_hand_symbol;
    private ArmsControllerSymbols left_door_bottom_hand_symbol;

    private ArmsControllerSymbols right_door_top_hand_symbol;
    private ArmsControllerSymbols right_door_middle_hand_symbol;
    private ArmsControllerSymbols right_door_bottom_hand_symbol;


    private Material _get_material_for_direction(ArmsControllerSymbols ArmsControllerSymbol)
    {
        switch (ArmsControllerSymbol)
        {
            default:
            case ArmsControllerSymbols.Up:
                return symbol_up_mat;
            case ArmsControllerSymbols.Down:
                return symbol_down_mat;
            case ArmsControllerSymbols.Left:
                return symbol_left_mat;
            case ArmsControllerSymbols.Right:
                return symbol_right_mat;
        }
    }

    private Material _get_material_for_hand(ArmsControllerSymbols ArmsControllerSymbol)
    {
        switch (ArmsControllerSymbol)
        {
            default:
            case ArmsControllerSymbols.Default:
                return hand_symbol_default_mat;
            case ArmsControllerSymbols.Gun:
                return hand_symbol_gun_mat;
            case ArmsControllerSymbols.Peace:
                return hand_symbol_peace_mat;
            case ArmsControllerSymbols.Love:
                return hand_symbol_love_mat;
        }
    }
    private void _init_directional_symbol_quads()
    {
        left_door_top_directional_symbol_quad = GameObject.Find("left-door-top-symbol").transform.Find("door-symbol").gameObject.GetComponent<MeshRenderer>();
        left_door_middle_directional_symbol_quad = GameObject.Find("left-door-middle-symbol").transform.Find("door-symbol").gameObject.GetComponent<MeshRenderer>();
        left_door_bottom_directional_symbol_quad = GameObject.Find("left-door-bottom-symbol").transform.Find("door-symbol").gameObject.GetComponent<MeshRenderer>();

        right_door_top_directional_symbol_quad = GameObject.Find("right-door-top-symbol").transform.Find("door-symbol").gameObject.GetComponent<MeshRenderer>();
        right_door_middle_directional_symbol_quad = GameObject.Find("right-door-middle-symbol").transform.Find("door-symbol").gameObject.GetComponent<MeshRenderer>();
        right_door_bottom_directional_symbol_quad = GameObject.Find("right-door-bottom-symbol").transform.Find("door-symbol").gameObject.GetComponent<MeshRenderer>();

        left_door_top_directional_symbol_quad.material = _get_material_for_direction(ArmsControllerSymbols.Up);
        left_door_middle_directional_symbol_quad.material = _get_material_for_direction(ArmsControllerSymbols.Up);
        left_door_bottom_directional_symbol_quad.material = _get_material_for_direction(ArmsControllerSymbols.Up);

        right_door_top_directional_symbol_quad.material = _get_material_for_direction(ArmsControllerSymbols.Up);
        right_door_middle_directional_symbol_quad.material = _get_material_for_direction(ArmsControllerSymbols.Up);
        right_door_bottom_directional_symbol_quad.material = _get_material_for_direction(ArmsControllerSymbols.Up);

    }
    private void _init_hand_symbol_quads()
    {

        left_door_top_hand_symbol_quad = GameObject.Find("left-door-top-symbol").transform.Find("hand-symbol").gameObject.GetComponent<MeshRenderer>();
        left_door_middle_hand_symbol_quad = GameObject.Find("left-door-middle-symbol").transform.Find("hand-symbol").gameObject.GetComponent<MeshRenderer>();
        left_door_bottom_hand_symbol_quad = GameObject.Find("left-door-bottom-symbol").transform.Find("hand-symbol").gameObject.GetComponent<MeshRenderer>();

        right_door_top_hand_symbol_quad = GameObject.Find("right-door-top-symbol").transform.Find("hand-symbol").gameObject.GetComponent<MeshRenderer>();
        right_door_middle_hand_symbol_quad = GameObject.Find("right-door-middle-symbol").transform.Find("hand-symbol").gameObject.GetComponent<MeshRenderer>();
        right_door_bottom_hand_symbol_quad = GameObject.Find("right-door-bottom-symbol").transform.Find("hand-symbol").gameObject.GetComponent<MeshRenderer>();

        left_door_top_hand_symbol_quad.material = _get_material_for_hand(ArmsControllerSymbols.Default);
        left_door_middle_hand_symbol_quad.material = _get_material_for_hand(ArmsControllerSymbols.Default);
        left_door_bottom_hand_symbol_quad.material = _get_material_for_hand(ArmsControllerSymbols.Default);

        right_door_top_hand_symbol_quad.material = _get_material_for_hand(ArmsControllerSymbols.Default);
        right_door_middle_hand_symbol_quad.material = _get_material_for_hand(ArmsControllerSymbols.Default);
        right_door_bottom_hand_symbol_quad.material = _get_material_for_hand(ArmsControllerSymbols.Default);

    }


    private void _init_glow_meshes()
    {
        left_door_top_glow_mesh = GameObject.Find("left-door-top-symbol").GetComponent<MeshRenderer>();
        left_door_middle_glow_mesh = GameObject.Find("left-door-middle-symbol").GetComponent<MeshRenderer>();
        left_door_bottom_glow_mesh = GameObject.Find("left-door-bottom-symbol").GetComponent<MeshRenderer>();

        right_door_top_glow_mesh = GameObject.Find("right-door-top-symbol").GetComponent<MeshRenderer>();
        right_door_middle_glow_mesh = GameObject.Find("right-door-middle-symbol").GetComponent<MeshRenderer>();
        right_door_bottom_glow_mesh = GameObject.Find("right-door-bottom-symbol").GetComponent<MeshRenderer>();

    }
    private void Start()
    {
        Player = GameObject.Find("player").GetComponent<ArmsController>();
        left_side = GameObject.Find("left-door");
        right_side = GameObject.Find("right-door");
        left_door_instructions = GameObject.Find("left-door-magic-square").transform.Find("instructions").gameObject;
        _init_glow_meshes();
        _init_hand_symbol_quads();
        _init_directional_symbol_quads();
        left_side.SetActive(false);
        left_door_instructions.SetActive(false);
        right_side.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player._on_enter_magic_square(this.gameObject);
        _on_enter_magic_square();
    }
    private void OnTriggerExit(Collider other)
    {
        Player._on_exit_magic_square(this.gameObject);
        _on_exit_magic_square();
    }

    public void _on_enter_magic_square()
    {
        //if (move_mode)
        //{
        //    left_side.SetActive(false);
        //    right_side.SetActive(false);
        //    if (in_left_magic_square)
        //        left_door_instructions.SetActive(true);
        //}
        //else
        //{
        //    if (in_left_magic_square)
        //    {
        //        Debug.Log("_switch_modes | in_left_magic_square");
        //        left_side.SetActive(true);
        //        left_door_instructions.SetActive(false);
        //        right_side.SetActive(true);
        //    }
        //}
        Debug.Log("_OnTriggerEnter");
        left_door_instructions.SetActive(true);
        //in_left_magic_square = true;
        ////right_door_instructions.SetActive(true);
        //in_right_magic_square = true;
    }

    public void _on_exit_magic_square()
    {
        Debug.Log("_OnTriggerExit");
        left_side.SetActive(false);
        left_door_instructions.SetActive(false);
        //in_left_magic_square = false;
        right_side.SetActive(false);
        //  right_door_instructions.SetActive(false);
        //in_right_magic_square = false;
    }
}