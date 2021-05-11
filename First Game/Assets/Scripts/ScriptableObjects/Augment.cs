using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Augment : ScriptableObject
{
    public new string name;
    public string description;
    public int slot;

    public Sprite icon;
    public Sprite highlightedIcon;

}
