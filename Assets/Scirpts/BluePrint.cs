using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePrint
{
    public string itemName;
    public string Req1;
    public string Req2;

    public int Req1amount;
    public int Req2amount;

    public int numOfRequirements;

    public BluePrint(string itemName, int reqNUM, string Req1, int Req1amount, string Req2, int Req2amount)
    {
        this.itemName = itemName;
        this.Req1 = Req1;
        this.Req2 = Req2;
        this.Req1amount = Req1amount;
        this.Req2amount = Req2amount;
        numOfRequirements = reqNUM;
    }
}