using UnityEngine;

public abstract class Item : ScriptableObject
{
    public int ID => GetInstanceID();

    [field: SerializeField] public bool IsStackAble { get; set; }
    [field: SerializeField] public int MaxStackSize { get; set; } = 1;
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string Type { get; set; }
    [field: SerializeField] [field: TextArea] public string Description { get; set; }
    [field: SerializeField] public int Attack {  get; set; }
    [field: SerializeField] public int Defense { get; set; }
    [field: SerializeField] public int Speed { get; set; }
    [field: SerializeField] public int Heal { get; set; }
    [field: SerializeField] public int Mana { get; set; }
    [field: SerializeField] public Sprite Image { get; set; }

}
