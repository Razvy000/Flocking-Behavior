using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{

    public Transform agentPrefab;       // agent template
    public int nAgents;                 // number of desired agents
    public List<Agent> agents;          // reference to all agents
    public List<Predator> predators;    // reference to all predators
    public float bound;                 // world bounds
    public float spawnR;                // spawning radius


    void Start()
    {
        // init

        agents = new List<Agent>();
        spawn(agentPrefab, nAgents);

        agents.AddRange(FindObjectsOfType<Agent>());
        predators.AddRange(FindObjectsOfType<Predator>());

    }

    void spawn(Transform prefab, int n)
    {
        for (int i = 0; i < n; i++)
        {

            Instantiate(prefab,
                        new Vector3(Random.Range(-spawnR, spawnR), 0, Random.Range(-spawnR, spawnR)),
                          Quaternion.identity);
        }
    }

    public List<Agent> getNeigh(Agent agent, float radius)
    {
        // get all neighbours of agent inside a given radius

        // iterating all agents and doing distance checks is ineficient
        // for eficiency you should partition the space!

        List<Agent> r = new List<Agent>();

        foreach (var otherAgent in agents)
        {

            if (otherAgent == agent)
                continue;

            if (Vector3.Distance(agent.x, otherAgent.x) <= radius)
            {
                r.Add(otherAgent);
            }
        }

        return r;
    }

    public List<Predator> getPredators(Agent agent, float radius)
    {
        // get all predators of agent inside a given radius

        List<Predator> r = new List<Predator>();

        foreach (var predator in predators)
        {

            if (Vector3.Distance(agent.x, predator.x) <= radius)
            {
                r.Add(predator);
            }
        }

        return r;
    }
}
