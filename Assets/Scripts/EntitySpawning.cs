using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// who is the parent init of this?
  
// this script spawns entities from spawnpoints....are tags?

public class EntitySpawning : MainGameInit{
    // public MainGameManager mainGameManager this should inherit from teh MainGameInit

    void Awake()
    {
       // mainMan = GetComponent(typeof(MainGameManager)) as MainGameManager;
    }
    /// <summary>
    /// Tobi's Entity Spawning Script;
    /// </summary>
    /// 
    // we should have a way to make this universal, tag A tag B tag C etc.
 
   public GameObject[] zombies; //should these should inherit from? what? Maybe it shouldn't care if a thing is human or zombie.
   public GameObject[] humans;
    struct PopulationData
    {
        uint m_poolSize;
        uint m_targetCount;
        float m_spawnInterval;
        float m_lastSpawnTime;
        float m_time;
        string m_factionTag;
        string m_spawnPointTag;
        GameObject[] m_entities;
        GameObject[] m_prefabs;

       

        public void setup(uint poolSize, uint targetCount, float spawnInterval, string factionTag, GameObject[] prefabs, string spawnPointTag)
        {
            print("EntitySpawning.cs setup " + this.ToString());
            m_poolSize = poolSize;
            m_targetCount = targetCount;
            m_spawnInterval = spawnInterval;
            m_factionTag = factionTag;
            m_spawnPointTag = spawnPointTag;
            m_prefabs = prefabs;
            m_lastSpawnTime = -m_spawnInterval;
            m_time = 0.0f; // BUG 7 - 27 - 2015 Object reference not set. how do we avoid that, ie, jump out if there's no object
            if (m_prefabs.Length == 0)
            {
                
                print("There are no SpawnPoint prefabs, spawning is disabled");

                m_poolSize = 0;
            }
        }

        public void update(float timeStep)
        {
            m_time += timeStep;
            if (m_factionTag == null)
            {
                print("NO FACTION TAG? " );
                m_factionTag = "Zombie";
            }
            m_entities = GameObject.FindGameObjectsWithTag(m_factionTag);

            if (m_entities.Length < m_targetCount && m_poolSize != 0u)
            {
                if (m_time - m_lastSpawnTime >= m_spawnInterval)
                {
                    m_lastSpawnTime += m_spawnInterval;
                    GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(m_spawnPointTag);
                    if (spawnPoints.Length == 0)
                    {
                        print("No spawn points with tag '" + m_spawnPointTag + "' found in scene! Cannot spawn any '" + m_factionTag + "s'!");
                    }
                    else
                    {
                        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                        Instantiate(m_prefabs[Random.Range(0, m_prefabs.Length)], spawnPoint.transform.position, spawnPoint.transform.rotation);
                        --m_poolSize;
                    }

                }
            }
        }
    }

    private PopulationData m_zombies;
    private PopulationData m_humans;


    public void Start()
    {
       
     
        if (humans.Length > 0)
        {
            // spawn zombies, whats all tha 2u, 2u stuff?
            m_humans.setup(2u, 2u, 4.0f, "Human", humans, "SpawnPoint_Human");
        }
        if (zombies.Length > 0)
        {
            m_zombies.setup(8u, 6u, 4.0f, "Zombie",zombies, "SpawnPoint_Zombie");
        }
    }

    // what does this do?
    public void Update()
    {
        m_humans.update(Time.deltaTime);
        m_zombies.update(Time.deltaTime);
    }

}
