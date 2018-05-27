using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player{
    private string name;
    private int money;
    private List<Utilities.Instrument> specialities;

	public Player(string name)
    {
        money = 0;
        specialities = new List<Utilities.Instrument>();
    }
}
