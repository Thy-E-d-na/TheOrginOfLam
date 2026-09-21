using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public enum SoulType
{
    Good,
    Neutral,
    Bad,
}
public enum EndingType
{
    BadEnding,
    TrueEnding,
    NoFishingRod,
    Deadlock,
    CaughtByStaff,
}
[Serializable]
public class Item
{
    public int id;
    public string name;
    //public GameObject itemModel; => save as Json file then no need to save model in the scriptable object
    public Item(int i)
    {
        this.id = i;
    }
}
public class CoreMngt : MonoBehaviour
{
    public static CoreMngt CoreInstance { get; private set; }

    #region ===================== PROGRESS  =====================
    [Header("Progress variables")]
    [SerializeField] private int coins;
    [SerializeField] private int curentDay = 1;
    [SerializeField] private int fishingRod = 2;
    [SerializeField] private int fishingRodPrice = 11; // rise price after each purchase

    //Trigger Ending A
    [SerializeField] private int limitDay = 7;
    [SerializeField] private int finishDay = 0;
    [SerializeField] private int minRod = 0;

    //Trigger Ending B
    [SerializeField] private int interactionCount = 0;
    [SerializeField] private int interactionLimit = 3;

    #endregion


    #region ===================== DAILY =====================
    [SerializeField] private int soulCollected = 0; // R1
    [SerializeField] private int soulsClassified = 0; //R3
    [SerializeField] private int dailyCoin = 0;
    [SerializeField] private int deadlockCount = 0; //R3

    #endregion


    #region ===================== EVENTS =====================

    public event Action BadEnd; // pass 7 days, < 3 interactions
    public event Action TrueEnd;
    public event Action OnFinalEvent; // interactionCount == 3
    public event Action NotEnoughCoins;
    public event Action<EndingType> OnGameOver;
    // all fishing rod destroyed & not enough money to buy new one b4 new day
    // deadlockCount >= 3 
    // being catched by the staff at final encounter
    #endregion

    #region ===================== GENERAL =====================
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    [SerializeField] private int soulPrice = 10;

    [SerializeField] private List<Item> myItems = new List<Item>();
    #endregion

    private void Awake()
    {
        if (CoreInstance == null)
        {
            CoreInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Check fishing rod after closed Shop UI, if fishing rod = 0, trigger GameOver1 event
    /// </summary>
    /// 
    public void BrokenRod()
    {
        if (fishingRod <= 0) return;
        else fishingRod--;
    }
    public void CollectSoul() => soulCollected++;


    /// <summary>
    /// Check if over intereaction limit, trigger Ending B event
    /// </summary>
    public void InteractionCount()
    {
        if (isGameOver) return;
        interactionCount++;
        if (interactionCount >= interactionLimit)
        {
            OnFinalEvent?.Invoke();
        }
    }

    // ===================== DAILY CHECK =====================

    //daily sum: calculate coins > shop > finish day > reset daily variable


    public int GetDailyWage()
    {
        dailyCoin = soulPrice * soulsClassified;
        return coins += dailyCoin;
    }

    public int Buying(int idItem, int pricetag)
    {

        if (coins < pricetag)
        {
            NotEnoughCoins?.Invoke();
        }
        else
        {
            myItems.Add(new Item(idItem));
            coins -= pricetag;
        }
        return coins;
    }
    public int SoulClassified() => soulsClassified++;

    public void ResetDailyVariable()
    {
        if (isGameOver) return;
        soulCollected = 0;
        soulsClassified = 0;
        dailyCoin = 0;
        deadlockCount = 0;
        if (fishingRod <= minRod && coins <= fishingRodPrice)
        {
            EndGame();
            OnGameOver?.Invoke(EndingType.NoFishingRod);
        }
    }
    public void ResetGame()
    {
        isGameOver = false;
        coins = 0;
        curentDay = 1;
        fishingRod = 2;
        fishingRodPrice = 11;
        finishDay = 0;
        minRod = 0;
        interactionCount = 0;
        soulCollected = 0;
        soulsClassified = 0;
        dailyCoin = 0;
        deadlockCount = 0;
    }
    // ===================== GENERAL =====================


    public void Deadlock()
    {
        deadlockCount++;
        if (deadlockCount >= 3)
        {
            EndGame();
            OnGameOver?.Invoke(EndingType.Deadlock);
        }
    }
    public void TrueEnding()
    {
        if (isGameOver) return;
        EndGame();
        TrueEnd?.Invoke();
    }
    public void CaughtByStaff()
    {
        if (isGameOver) return;
        EndGame();
        OnGameOver?.Invoke(EndingType.CaughtByStaff);
    }
    public void FinishDayCount()
    {
        if (isGameOver) return;
        finishDay++;
        if (finishDay >= limitDay)
        {

            EndGame();
            BadEnd?.Invoke();
        }
    }
    public void EndGame() => isGameOver = true;

    #region PackData

    //public void ExportState(SaveData data)
    //{
    //    data._coins = coins;
    //    data._curentDay = curentDay;
    //    data._fishingRod = fishingRod;
    //    data._fishingRodPrice = fishingRodPrice;
    //    data._interactionCount = interactionCount;
    //    data.myProps = new List<Item>(myItems);
    //}

    //public void ImportState(SaveData source)
    //{
    //    coins = source._coins;
    //    curentDay = source._curentDay;
    //    fishingRod = source._fishingRod;
    //    fishingRodPrice = source._fishingRodPrice;
    //    interactionCount = source._interactionCount;
    //    myItems = source.myProps != null ? new List<Item>(source.myProps) : new List<Item>();

    //    ResetDailyVariable();
    //}
    #endregion

}

#region


#endregion
