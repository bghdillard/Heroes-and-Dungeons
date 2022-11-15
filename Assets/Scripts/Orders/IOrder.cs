using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrder //starting to think I should've made this an abstract class. Good job past me, that is indeed what you decided to do. Well, actually you went interface, but still.
{

    Vector3 GetLocation();
    string GetOrderType();
    void StartOrder();
    void CancelOrder();
    bool GetStarted();
    float GetTime();
}
