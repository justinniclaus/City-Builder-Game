using SVS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    private CityManager cityManager;
    public StructurePrefabWeighted[] housesPrefabs, specialPrefabs;
    public PlacementManager placementManager;

    public event Action<Vector3Int> OnHousePlaced;
    public event Action<Vector3Int> OnSpecialPlaced;

    private float[] houseWeights, specialWeights;

    private void Start()
    {
        houseWeights = housesPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        cityManager = FindAnyObjectByType<CityManager>();    
    }

    public void PlaceHouse(Vector3Int position)
    {
        if (!cityManager.CanAffordHouse())
        {
            Debug.Log("Not enough money to place house!");
            return;
        }

        if(CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(houseWeights);
            placementManager.PlaceObjectOnTheMap(position, housesPrefabs[randomIndex].prefab, CellType.Structure);
            cityManager.ChargeForHouse();
            OnHousePlaced?.Invoke(position);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if(!cityManager.CanAffordSpecial())
        {
            Debug.Log("Not enough money to place special building!");
            return;
        }

        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(specialWeights);
            placementManager.PlaceObjectOnTheMap(position, specialPrefabs[randomIndex].prefab, CellType.Structure);
            cityManager.ChargeForSpecial();
            OnSpecialPlaced?.Invoke(position);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("This position is out of bounds.");
            return false;
        }
        if (placementManager.CheckIfPositionIsFree(position) == false)
        {
            Debug.Log("This position is already taken.");
            return false;
        }
        if (placementManager.GetNeighborsOfTypeFor(position, CellType.Road).Count <= 0)
        {
            Debug.Log("Must be placed near a road.");
            return false;
        }
        return true;
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0, sum);
        float tempSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if(randomValue >= tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0;
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
     public GameObject prefab;
     [Range(0,1)]
     public float weight;
}