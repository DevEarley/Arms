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

    private void Start()
    {
        Player = GameObject.Find("player").GetComponent<ArmsController>();

    }

    private void OnTriggerEnter(Collider other)
    {
        Player._on_enter_magic_square(this.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        Player._on_exit_magic_square(this.gameObject);

    }
}