using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CityManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI populationText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI currentGoalText;
    [SerializeField] private TextMeshProUGUI incomeText;

    [Header("City Settings")]
    [SerializeField] private int startingMoney = 1000;
    [SerializeField] private int populationPerHouse = 4;
    [SerializeField] private int populationPerSpecialBuilding = 2;

    [Header("Building Costs")]
    [SerializeField] private int houseCost = 100;
    [SerializeField] private int roadCost = 20;
    [SerializeField] private int specialBuildingCost = 300;

    [Header("Income Settings")]
    [SerializeField] private float incomeInterval = 3f;
    [SerializeField] private int incomePerResident = 2;
    [SerializeField] private int incomePerHouse = 10;
    [SerializeField] private int incomePerSpecial = 50;

    [Header("Refund Settings")]
    [SerializeField] private float refundRate = 0.7f;

    [Header("Goals")]
    [SerializeField] private CityGoalData[] goals = new CityGoalData[3];

    [Header("Audio")]
    [SerializeField] private AudioClip goalCompleteSound;
    [SerializeField] private AudioSource audioSource;

    private int currentMoney;
    private int currentPopulation;
    private int currentGoalIndex;
    private float incomeTimer;
    private Dictionary<Vector3Int, BuildingType> buildingTracker = new Dictionary<Vector3Int, BuildingType>();
    private StructureManager structureManager;

    [System.Serializable]
    public class CityGoalData
    {
        public int requiredPopulation;
        public int requiredSpecialBuildings;
        public int moneyReward;
        public string description;
        public bool isCompleted;
    }

    private void Start()
    {
        if (goals.Length == 0)
        {
            goals = new CityGoalData[]
            {
                new CityGoalData 
                { 
                    requiredPopulation = 20, 
                    requiredSpecialBuildings = 1, 
                    moneyReward = 500,
                    description = "Reach 20 population and build 1 special building"
                },
                new CityGoalData 
                { 
                    requiredPopulation = 50, 
                    requiredSpecialBuildings = 2, 
                    moneyReward = 1000,
                    description = "Reach 50 population and build 2 special buildings"
                },
                new CityGoalData 
                { 
                    requiredPopulation = 100, 
                    requiredSpecialBuildings = 4, 
                    moneyReward = 2000,
                    description = "Reach 100 population and build 4 special buildings"
                }
            };
        }

        currentMoney = startingMoney;
        currentPopulation = 0;
        incomeTimer = incomeInterval;
        currentGoalIndex = 0;
        structureManager = FindAnyObjectByType<StructureManager>();

        if (structureManager != null)
        {
            structureManager.OnHousePlaced += HandleHousePlacement;
            structureManager.OnSpecialPlaced += HandleSpecialPlacement;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        UpdateUI();
    }

    private void Update()
    {
        incomeTimer -= Time.deltaTime;
        if (incomeTimer <= 0)
        {
            GenerateIncome();
            incomeTimer = incomeInterval;
        }
    }

    private void CheckGoalProgress()
    {
        if (currentGoalIndex >= goals.Length) return;

        var currentGoal = goals[currentGoalIndex];
        if (currentGoal.isCompleted) return;

        int specialBuildingCount = buildingTracker.Count(x => x.Value == BuildingType.Special);

        if (currentPopulation >= currentGoal.requiredPopulation && 
            specialBuildingCount >= currentGoal.requiredSpecialBuildings)
        {
            CompleteCurrentGoal();
        }
    }

    private void CompleteCurrentGoal()
    {
        if (currentGoalIndex >= goals.Length) return;

        int rewardAmount = goals[currentGoalIndex].moneyReward;
        
        currentMoney += rewardAmount;
        goals[currentGoalIndex].isCompleted = true;

        if (goalCompleteSound != null)
        {
            Debug.Log("Playing goal complete sound"); 
            audioSource.PlayOneShot(goalCompleteSound);
        }
        else
        {
            Debug.LogWarning("Goal complete sound is not assigned!"); 
        }

        Debug.Log($"Goal Completed! Earned ${rewardAmount}");
        
        currentGoalIndex++;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${currentMoney}";
        }

        if (populationText != null)
        {
            populationText.text = $"Population: {currentPopulation}";
        }

        if (incomeText != null)
        {
            int currentIncome = CalculateCurrentIncome();
            incomeText.text = $"Income: ${currentIncome} / {incomeInterval}s";
        }

        if (currentGoalText != null)
        {
            if (currentGoalIndex < goals.Length)
            {
                var goal = goals[currentGoalIndex];
                currentGoalText.text = $"Goal: {goal.description}\n" +
                                     $"Required Population: {goal.requiredPopulation}\n" +
                                     $"Required Special Buildings: {goal.requiredSpecialBuildings}\n" +
                                     $"Reward: ${goal.moneyReward}";
            }
            else
            {
                currentGoalText.text = "All goals completed!";
            }
        }
    }

    // Add all the money management methods from before
    public bool CanAffordHouse() => currentMoney >= houseCost;
    public bool CanAffordRoad() => currentMoney >= roadCost;
    public bool CanAffordSpecial() => currentMoney >= specialBuildingCost;

    public void ChargeForHouse() { currentMoney -= houseCost; UpdateUI(); }
    public void ChargeForRoad() { currentMoney -= roadCost; UpdateUI(); }
    public void ChargeForSpecial() { currentMoney -= specialBuildingCost; UpdateUI(); }

    public void RefundHouse() { currentMoney += Mathf.RoundToInt(houseCost * refundRate); UpdateUI(); }
    public void RefundRoad() { currentMoney += Mathf.RoundToInt(roadCost * refundRate); UpdateUI(); }
    public void RefundSpecial() { currentMoney += Mathf.RoundToInt(specialBuildingCost * refundRate); UpdateUI(); }

    private void GenerateIncome()
    {
        int income = CalculateCurrentIncome();
        currentMoney += income;
        UpdateUI();
    }

    private int CalculateCurrentIncome()
    {
        int income = 0;
        income += currentPopulation * incomePerResident;
        income += buildingTracker.Count(x => x.Value == BuildingType.House) * incomePerHouse;
        income += buildingTracker.Count(x => x.Value == BuildingType.Special) * incomePerSpecial;
        return income;
    }

    private void HandleHousePlacement(Vector3Int position)
    {
        if (CanAffordHouse())
        {
            ChargeForHouse();
            buildingTracker[position] = BuildingType.House;
            currentPopulation += populationPerHouse;
            CheckGoalProgress();
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money to place house!");
        }
    }

    private void HandleSpecialPlacement(Vector3Int position)
    {
        if (CanAffordSpecial())
        {
            ChargeForSpecial();
            buildingTracker[position] = BuildingType.Special;
            
            var nearbyHouses = GetNearbyHouses(position);
            currentPopulation += populationPerSpecialBuilding;
            
            CheckGoalProgress();
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money to place special building!");
        }
    }

    private int GetNearbyHouses(Vector3Int position)
    {
        int count = 0;
        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                Vector3Int checkPos = new Vector3Int(position.x + x, 0, position.z + z);
                if (buildingTracker.ContainsKey(checkPos) && buildingTracker[checkPos] == BuildingType.House)
                {
                    count++;
                }
            }
        }
        return count;
    }
}

public enum BuildingType
{
    House,
    Special
}