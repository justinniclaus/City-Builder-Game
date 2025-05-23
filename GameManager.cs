using UnityEngine;
using SVS;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;

    public RoadManager roadManager;

    public InputManager inputManager;

    public UIController uiController;

    public StructureManager structureManager;

    public CityManager cityManager;
    
    public PlacementManager placementManager;

    private void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnDeleteMode += DeleteModeHandler; 
    }
    private void DeleteModeHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += HandleDelete;
    }
    private void HandleDelete(Vector3Int position)
    {
        placementManager.DeleteStructureAt(position);
        roadManager.UpdateRoadAtPosition(position);
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceSpecial;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceHouse;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick +=  roadManager.PlaceRoad;
        inputManager.OnMouseHold +=  roadManager.PlaceRoad;
        inputManager.OnMouseUp +=  roadManager.FinishPlacingRoad;
    }

    private void ClearInputActions()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x,0,
            inputManager.CameraMovementVector.y));
    }
}
