using UnityEngine;
using System.Collections;

public class Predator : Agent
{

    override protected Vector3 combine()
    {

        return conf.Kw * wander();
    }
}
