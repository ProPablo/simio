using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class BehaviourManager : MonoBehaviour
{
    public static BehaviourManager i;
    public bool enableTime = true;
    public float tickDur = 0.5f;
    float timer;
    public float slerpCentre = 0.25f;
    public Actor[] actorSpawns;
    public List<Actor> currentActors = new List<Actor>();
    public Actor corpseActor, meatActor;
    public Image pieCirclePrefab;
    private List<Image> pieCircles = new List<Image>();
    public int totalTicks = 0;

    public Actor samplePrefab;
    private BehaviourComponent[] systemBehaviors;

    public float startSpawnWeight = 0.15f;

    private void Awake()
    {
        i = this;
        systemBehaviors = GetComponents<BehaviourComponent>();
    }
    private void Start()
    {
        foreach (var item in HexGrid.i.cells)
        {
            if (Random.value < startSpawnWeight)
            {
                Actor spawn = actorSpawns[Random.Range(0, actorSpawns.Length)];
                if (spawn.walkable.Contains(item.cellType))
                    SpawnActor(spawn, item);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            //SpawnActor(actorSpawns[Random.Range(0, actorSpawns.Length)]);
            SpawnRandomActor();
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SpawnActor(samplePrefab, HexGrid.i.currentlySelected);
        }

        if (enableTime)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = tickDur;
                RunTick();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            RunTick();
        }
    }

    void RunTick()
    {
        totalTicks++;
        var currentActorCount = currentActors.Count;
        for (int j = 0; j < currentActorCount; j++)
        {
            var actor = currentActors[j];
            bool isTicked = false;
            foreach (BehaviourComponent behaviour in actor.behaviours)
            {
                if (isTicked)
                {
                    behaviour.ticks = 0;
                }
                else if (behaviour.OnTick())
                {
                    isTicked = true;
                    behaviour.ticks = 0;
                    actor.currentBehaviour = behaviour.Name;
                }
            }
        }


        bool isSysTicked = false;
        foreach (BehaviourComponent behaviour in systemBehaviors)
        {
            if (isSysTicked)
            {
                behaviour.ticks = 0;
            }
            else if (behaviour.OnTick())
            {
                isSysTicked = true;
                behaviour.ticks = 0;
            }
        }

        // foreach (Actor actor in currentActors) {
        //     bool isTicked = false;
        //     foreach (BehaviourComponent behaviour in actor.behaviours)
        //     {
        //         if (isTicked)
        //         {
        //             behaviour.ticks = 0;
        //         }
        //         else if (behaviour.OnTick())
        //         {
        //             isTicked = true;
        //             behaviour.ticks = 0;
        //             actor.currentBehaviour = behaviour.Name;
        //         }
        //     }
        // }
    }

    // private void TickBehaviours(IEnumerable<BehaviourComponent> behaviours)
    // {
    // }


    public void SpawnActor(Actor actorToSpawn, HexCell spawnPos)
    {
        Actor spawned = Instantiate(actorToSpawn, spawnPos.transform.position, Quaternion.identity);
        currentActors.Add(spawned);
        spawned.currentTile = spawnPos;
        spawned.currentTile.JoinCell(spawned);

        spawned.deathEvent += DeathEvent;
    }

    private void SetPieChart()
    {
    }

    public void SpawnRandomActor(HexCell selectedTile = null)
    {
        if (selectedTile == null)
            selectedTile = HexGrid.i.currentlySelected;
        var spawnByTile = actorSpawns.Where(a => (a.walkable.Contains(selectedTile.cellType))).ToList().RandomElement();
        if (spawnByTile == null) return;
        SpawnActor(spawnByTile, selectedTile);
    }

    public void DeathEvent(StateMachine sm)
    {
        currentActors.Remove(sm as Actor);
        sm.deathEvent -= DeathEvent;
    }

    public void SpawnCorpse(HexCell tile)
    {
        SpawnActor(corpseActor, tile);
        SpawnActor(meatActor, tile);
    }
}