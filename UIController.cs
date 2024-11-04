using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement;
    public Action OnDeleteMode; // New action for delete mode
    public Button placeRoadButton, placeHouseButton, placeSpecialButton, deleteButton; // Added delete button

    public Color outlineColor = Color.white;
    List<Button> buttonList;
    private Dictionary<Button, Outline> buttonOutlines = new Dictionary<Button, Outline>();

    private void Start()
    {
        InitializeButtons();
        SetupButtonListeners();
    }

    private void InitializeButtons()
    {
        // Initialize button list with non-null buttons
        buttonList = new List<Button>();
        
        if (placeRoadButton != null)
        {
            buttonList.Add(placeRoadButton);
            EnsureButtonHasOutline(placeRoadButton);
        }
        
        if (placeHouseButton != null)
        {
            buttonList.Add(placeHouseButton);
            EnsureButtonHasOutline(placeHouseButton);
        }
        
        if (placeSpecialButton != null)
        {
            buttonList.Add(placeSpecialButton);
            EnsureButtonHasOutline(placeSpecialButton);
        }
        
        if (deleteButton != null)
        {
            buttonList.Add(deleteButton);
            EnsureButtonHasOutline(deleteButton);
        }
    }

    private void EnsureButtonHasOutline(Button button)
    {
        Outline outline = button.GetComponent<Outline>();
        if (outline == null)
        {
            outline = button.gameObject.AddComponent<Outline>();
            outline.effectColor = outlineColor;
        }
        buttonOutlines[button] = outline;
        outline.enabled = false;
    }

    private void SetupButtonListeners()
    {
        if (placeRoadButton != null)
        {
            placeRoadButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeRoadButton);
                OnRoadPlacement?.Invoke();
            });
        }

        if (placeHouseButton != null)
        {
            placeHouseButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeHouseButton);
                OnHousePlacement?.Invoke();
            });
        }

        if (placeSpecialButton != null)
        {
            placeSpecialButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeSpecialButton);
                OnSpecialPlacement?.Invoke();
            });
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(deleteButton);
                OnDeleteMode?.Invoke();
            });
        }
    }

    private void ModifyOutline(Button button)
    {
        if (buttonOutlines.TryGetValue(button, out Outline outline))
        {
            outline.enabled = true;
        }
    }

    private void ResetButtonColor()
    {
        foreach (var button in buttonList)
        {
            if (buttonOutlines.TryGetValue(button, out Outline outline))
            {
                outline.enabled = false;
            }
        }
    }

    private void OnValidate()
    {
        // Optional: Add outlines in the editor
        if (placeRoadButton != null && placeRoadButton.GetComponent<Outline>() == null)
            placeRoadButton.gameObject.AddComponent<Outline>();
        if (placeHouseButton != null && placeHouseButton.GetComponent<Outline>() == null)
            placeHouseButton.gameObject.AddComponent<Outline>();
        if (placeSpecialButton != null && placeSpecialButton.GetComponent<Outline>() == null)
            placeSpecialButton.gameObject.AddComponent<Outline>();
        if (deleteButton != null && deleteButton.GetComponent<Outline>() == null)
            deleteButton.gameObject.AddComponent<Outline>();
    }
}