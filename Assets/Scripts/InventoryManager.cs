﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class InventoryManager : Singleton<InventoryManager>
{
	[Range(1, 10)] // max of 10 since input currently uses number bar
	public int MaxHacks = 10;
	public List<Hack> AvailableHacks, BuyableHacks;
	public HashSet<Hack> HacksToBePatched;
	public int Credits;

	public bool PatchesReady => HacksToBePatched != null && HacksToBePatched.Count > 0;
	
	void Awake ()
	{
		if (SingletonGetInstance() != null)
		{
			Destroy(gameObject);
		}
		else
		{
			SingletonSetInstance(this, false);
			transform.parent = null;
			DontDestroyOnLoad(gameObject);
		}
	}

	void Start ()
	{
		if (HacksToBePatched == null)
		{
			HacksToBePatched = new HashSet<Hack>();
		}
	}

	public void PatchAll ()
	{
		foreach (var hack in HacksToBePatched)
		{
			AvailableHacks.Remove(hack);
			BuyableHacks.Remove(hack);
		}
		HacksToBePatched.Clear();
	}

	public List<Hack> GetShopListing (int number)
	{
		return BuyableHacks
			.OrderBy(x => System.Guid.NewGuid())
			.Take(number)
			.ToList();
	}

	public void AppraiseInventory ()
	{
		foreach (var hack in AvailableHacks)
		{
			hack.NewMarketPrice();
		}
	}

	// assumes we have enough credits
	public void BuyHack (Hack hack)
	{
		Credits -= hack.CurrentMarketPrice;
		BuyableHacks.Remove(hack);
		AvailableHacks.Add(hack);
	}

	public void SellHack (Hack hack)
	{
		if (AvailableHacks.Contains(hack))
		{
			Credits += hack.CurrentMarketPrice;
			BuyableHacks.Add(hack);
			AvailableHacks.Remove(hack);
		}
	}

	public bool CanBuyHack (Hack hack)
	{
		return hack.CurrentMarketPrice <= Credits;
	}
}
