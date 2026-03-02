using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCardCollection", menuName = "Cards/Player Collection")]
public class PlayerCardCollection : ScriptableObject
{
    public List<CardData> unlockedCards = new List<CardData>();
}

