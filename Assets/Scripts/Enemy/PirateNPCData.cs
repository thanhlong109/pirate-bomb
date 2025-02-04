using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pirate Data", menuName = "NPC/Pirate")]
public class PirateNPCData : ScriptableObject
{
    [Header("Basic info")]
    public string Name;
    public Sprite Avatar;
    public int Health;
    public float Speed;
    public float JumpForce;

    [Header("Action")]
    public bool canAttack;
    public int AttackDamage;


    [Header("Sound & Effect")]
    public AudioClip AttackSound;
}
