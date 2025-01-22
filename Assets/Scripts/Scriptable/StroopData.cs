using UnityEngine;

[CreateAssetMenu(fileName = "StroopData", menuName = "Scriptable Objects/StroopData")]

public class StroopData : ScriptableObject
{
    public int number;
    public string text;
    public Sprite image;
}