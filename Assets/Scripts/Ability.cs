using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.

[System.Serializable]
public class Ability {

	public int id = 0;
	public string text = "DEFAULT ABILITY TEXT";
	public AbilityTargets validTargets = AbilityTargets.AllTargets;
	public TriggerType trigger = TriggerType.Passive;
	public List<int> nextAbilityId = new List<int>(){0, -1};
	public bool nextAbilityPlayerChoice = false;
	public int abilityQuantity = 0;
	public int targetquantity =0;
	public int radius = 0;
	public bool lineOfSight = false;
	public bool upTo = true;
	public Attack attack;
}

[Flags]
public enum AbilityTargets{
	Self = 1,
	AllTargets = 1<<1,
	AllEnemyTargets = 1<<2,
	AllAllyTargets = 1<<3,
	QuantityEnemyTargets = 1<<4,
	QuantityAllyTargets = 1<<5
}

public enum TriggerType{
	Activatable,
	Surge,
	Reset,
	Passive,
	AbilityChain
}

public enum AbilityType{
	CharacterSpecific,
	Attack,
	Move,
	Sprint,
	AwardToken,
	RemoveToken,
	Push,
	AddAttackDice,
	AddDefenseDice,
	AddMaxPowerCardSlots
}

public enum TokenType{
	Aim,
	Dodge,
	//[Display(Name = "Attack+1")]
	AttackPlus,
	//[Display(Name = "Attack-1")]
	AttackMinus,
	//[Display(Name = "Defense+1")]
	DefensePlus,
	//[Display(Name = "Defense-1")]
	DefenseMinus
}