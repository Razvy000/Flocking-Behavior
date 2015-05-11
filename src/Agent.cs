using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour
{

    public Vector3 x;           // position
    public Vector3 v;           // velocity
    public Vector3 a;           // acceleration
    public World world;         // world reference
    public AgentConfig conf;    // agent config


    void Start()
    {
        // init
        world = FindObjectOfType<World>();

        conf = FindObjectOfType<AgentConfig>();

        x = transform.position;
        v = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));

        //debugWanderCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    void Update()
    {

        float t = Time.deltaTime;

        // combine all accelerations
        a = combine();

        a = Vector3.ClampMagnitude(a, conf.maxA);

        // Euler forward integration
        v = v + a * t;
        v = Vector3.ClampMagnitude(v, conf.maxV);

        x = x + v * t;

        // keep agent inside world bounds by wrapping around
        wrapAround(ref x, -world.bound, world.bound);

        transform.position = x;

        if (v.magnitude > 0)
            transform.LookAt(x + v);
    }


    Vector3 cohesion()
    {
        // cohesion behavior

        Vector3 r = new Vector3();
        int countAgents = 0;

        // get all my nearby neighbors inside radius Rc of this current agent
        var neighs = world.getNeigh(this, conf.Rc);

        // no neighbors means no cohesion desire
        if (neighs.Count == 0)
            return r;

        // find the center of mass of all neighbors
        foreach (var agent in neighs)
        {
            if (isInFieldOfView(agent.x))
            {
                r += agent.x;
                countAgents++;
            }
        }
        if (countAgents == 0)
            return r;

        r /= countAgents;

        // a vector from our position x towards the COM r
        r = r - this.x;

        r = Vector3.Normalize(r);

        return r;
    }

    Vector3 separation()
    {
        // separation behavior
        // steer in the oposite direction from each of our nearby neighbors

        Vector3 r = new Vector3();

        // get all neighbors
        var agents = world.getNeigh(this, conf.Rs);

        // no neighbors no separation desire
        if (agents.Count == 0)
            return r;

        // add the contribution of each neighbor towards me
        foreach (var agent in agents)
        {

            if (isInFieldOfView(agent.x))
            {
                Vector3 towardsMe = this.x - agent.x;

                // force contribution will vary inversly proportional to distance
                if (towardsMe.magnitude > 0)
                {
                    r += towardsMe.normalized / towardsMe.magnitude;
                }
            }
        }

        return r.normalized;
    }


    Vector3 alignment()
    {
        // alighment behavior
        // steer agent to match the direction and speed of neighbors

        Vector3 r = new Vector3();

        // get all neighbors
        var agents = world.getNeigh(this, conf.Ra);

        // no neighbors means no one to align to
        if (agents.Count == 0)
            return r;

        // match direction and speed == match velocity
        // do this for all neighbors
        foreach (var agent in agents)
            if (isInFieldOfView(agent.x))
                r += agent.v;

        return r.normalized;
    }

    virtual protected Vector3 combine()
    {
        // combine all desires
        // weighted sum
        Vector3 r = conf.Kc * cohesion() + conf.Ks * separation() + conf.Ka * alignment()
                + conf.Kw * wander()
                + conf.Kavoid * avoidEnemies();
        return r;
    }

    void wrapAround(ref Vector3 v, float min, float max)
    {
        v.x = wrapAroundFloat(v.x, min, max);
        v.y = wrapAroundFloat(v.y, min, max);
        v.z = wrapAroundFloat(v.z, min, max);
    }

    float wrapAroundFloat(float value, float min, float max)
    {

        //        min ------value-------- max
        if (value > max)
            value = min;
        else if (value < min)
            value = max;
        return value;
    }

    bool isInFieldOfView(Vector3 stuff)
    {
        return Vector3.Angle(this.v, stuff - this.x) <= conf.MaxFieldOfViewAngle;
    }

    Vector3 wanderTarget;
    GameObject debugWanderCube;

    protected Vector3 wander()
    {
        // wander steer behavior that looks purposeful

        float jitter = conf.WanderJitter * Time.deltaTime;

        // add a small random vector to the target's position
        wanderTarget += new Vector3(RandomBinomial() * jitter, 0, RandomBinomial() * jitter);

        // reproject the vector back to unit circle
        wanderTarget = wanderTarget.normalized;

        // increase length to be the same as the radius of the wander circle
        wanderTarget *= conf.WanderRadius;

        // position the target in front of the agent
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(0, 0, conf.WanderDistance);

        // project the target from local space to world space
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);

        //debugWanderCube.transform.position = targetInWorldSpace;

        // steer towards it
        targetInWorldSpace -= this.x;

        return targetInWorldSpace.normalized;
    }

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    Vector3 avoidEnemies()
    {
        // flee from all enemies behavior
        Vector3 r = new Vector3();

        var enemies = world.getPredators(this, conf.Ravoid);

        if (enemies.Count == 0)
            return r;

        foreach (var enemy in enemies)
        {
            r += flee(enemy.x);
        }

        return r.normalized;
    }

    Vector3 flee(Vector3 target)
    {
        // run the opposite direction from the target
        Vector3 desiredVel = (x - target).normalized * conf.maxV;

        // steer our velocity
        return desiredVel - v;
    }
}
