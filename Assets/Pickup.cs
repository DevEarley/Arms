using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PickupType
{
    Hammer
}

public class Pickup : MonoBehaviour
{
    public GameObject LightCone;
    public GameObject Instructions;
    public MeshRenderer MeshRenderer;
    private ArmsController Player;
    public PickupType PickupType;

    private void Start()
    {
        Player = GameObject.Find("player").GetComponent<ArmsController>();
      
    }

    public void _on_look_at()
    {
        Instructions.SetActive(true);
    }

    public void _on_look_away()
    {
        Instructions.SetActive(false);
    }

    public void _on_pickup()
    {
        MeshRenderer.enabled = false;
        LightCone.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player._on_enter_pickup(this);
    }

    private void OnTriggerExit(Collider other)
    {
        Player._on_exit_pickup(this);
    }
}
