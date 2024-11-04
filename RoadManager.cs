using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using SVS;

public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;

    public List<Vector3Int> temporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;
    private CityManager cityManager;
    public RoadFixer roadFixer;

    [Obsolete]
    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();
        cityManager = FindObjectOfType<CityManager>();
    }

    public void PlaceRoad(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            return;
        }
        if (placementManager.CheckIfPositionIsFree(position) == false)
        {
            return;
        }

        if (placementMode == false)
        {
            // Starting a new road
            if (!cityManager.CanAffordRoad())
            {
                Debug.Log("Not enough money to start road!");
                return;
            }

            temporaryPlacementPositions.Clear();
            roadPositionsToRecheck.Clear();

            placementMode = true;
            startPosition = position;
            
            temporaryPlacementPositions.Add(position);
            placementManager.PlaceTemporaryStructure(position, roadFixer.deadEnd, CellType.Road);
            cityManager.ChargeForRoad(); // Charge for first piece
        }
        else
        {
            // Calculate path to new position
            List<Vector3Int> newRoadPositions = placementManager.GetPathBetween(startPosition, position);
            
            // Check if we can afford all new road pieces
            int newPieces = newRoadPositions.Count - temporaryPlacementPositions.Count;
            int totalCost = newPieces * 20; // Assuming road cost is 20, you might want to get this from CityManager

            if (!cityManager.CanAffordRoad() && newPieces > 0)
            {
                Debug.Log("Not enough money to extend road!");
                return;
            }

            placementManager.RemoveAllTemporaryStructures();
            temporaryPlacementPositions.Clear();

            foreach (var positionToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, positionToFix);
            }
            roadPositionsToRecheck.Clear();

            temporaryPlacementPositions = newRoadPositions;

            foreach (var temporaryPosition in temporaryPlacementPositions)
            {
                if (placementManager.CheckIfPositionIsFree(temporaryPosition) == false)
                {
                    roadPositionsToRecheck.Add(temporaryPosition);
                    continue;
                }
                placementManager.PlaceTemporaryStructure(temporaryPosition, roadFixer.deadEnd, CellType.Road);
                if (!temporaryPosition.Equals(startPosition)) // Don't charge again for the start position
                {
                    cityManager.ChargeForRoad();
                }
            }
        }
        FixRoadPrefabs();
    }

    public void FinishPlacingRoad()
    {
        placementMode = false;
        placementManager.AddTemporaryStructuresToStructureDictionary();
        
        if (temporaryPlacementPositions.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        temporaryPlacementPositions.Clear();
        startPosition = Vector3Int.zero;
    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in temporaryPlacementPositions)
        {
            roadFixer.FixRoadAtPosition(placementManager, temporaryPosition);
            var neighbors = placementManager.GetNeighborsOfTypeFor(temporaryPosition, CellType.Road);
            
            foreach (var roadPosition in neighbors)
            {
                if (roadPositionsToRecheck.Contains(roadPosition) == false)
                {
                    roadPositionsToRecheck.Add(roadPosition);
                }
            }
        }

        foreach (var positionToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager, positionToFix);
        }
    }

    public void UpdateRoadAtPosition(Vector3Int position)
    {
        var neighbors = placementManager.GetNeighborsOfTypeFor(position, CellType.Road);
        foreach (var roadPosition in neighbors)
        {
            roadFixer.FixRoadAtPosition(placementManager, roadPosition);
        }
    }
}
