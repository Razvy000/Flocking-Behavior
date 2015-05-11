using UnityEngine;
using System.Collections;

public class AgentConfig : MonoBehaviour
{

    public float Rc;        // cohesion radius
    public float Rs;        // separation radius
    public float Ra;        // alignment radius
    public float Ravoid;    // avoidance radius


    public float Kc;        // cohesion coeficient
    public float Ks;        // separation coeficient
    public float Ka;        // alignment coeficient
    public float Kw;        // wander coeficient
    public float Kavoid;    // avoidance coeficient

    public float MaxFieldOfViewAngle = 180;     // maximum field of view

    public float WanderJitter;      // size of jitter for wander
    public float WanderRadius;      // wander radius
    public float WanderDistance;    // wander distance from center of agent

    public float maxA;      // maximum acceleration
    public float maxV;      // maximum velocity
}
