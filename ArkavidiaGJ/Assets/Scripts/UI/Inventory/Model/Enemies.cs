using UnityEngine;

[CreateAssetMenu]
public class Enemies : ScriptableObject
{
    public int ID => GetInstanceID();
    [field: SerializeField] public string Type { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Health { get; set; }
    [field: SerializeField] public int Mana { get; set; }
    [field: SerializeField] public int Attack { get; set; }
    [field: SerializeField] public int Defense { get; set; }
    [field: SerializeField] public int Speed { get; set; }
    [field: SerializeField] public Sprite Image { get; set; }
    [field: SerializeField] public GameObject Object { get; set; }
}
