﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


abstract public class Events
{
    public abstract void ThrowEvent();
    public abstract GameObject GetEventCard();
    public abstract void SetEventCard(GameObject eventCardPrefab);
    public abstract string GetName();
}
