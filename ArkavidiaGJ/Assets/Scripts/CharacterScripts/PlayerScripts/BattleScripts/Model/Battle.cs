using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Battle", menuName = "Scriptable Objects/Battle")]
public class Battle : ScriptableObject
{
    [System.Serializable]
    public class TeamMember
    {
        public GameObject _characterPrefab;
        public Characters _characterData;
    }

    public List<TeamMember> _playerTeam;
    public List<TeamMember> _enemyTeam;
}
