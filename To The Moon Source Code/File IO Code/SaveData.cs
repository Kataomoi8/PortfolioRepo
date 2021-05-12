using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveData : MonoBehaviour
{
    [Header("File Name/File Header")]
    [SerializeField] string fileName = "Test.txt";
    [SerializeField] string header = "----Ship Save Data----";

    [Header("Debug Variables For Editor")]
    [SerializeField] int readInColor = 0;
    [SerializeField] int readInBody = 0;
    [SerializeField] int readInPrimary = 0;
    [SerializeField] int readInSecondary = 0;
    [SerializeField] int readInUlt = 0;

    //private variables 
    Ship.playerShipSettings saveStorage;
    private int currency = 0;
    int unlockedCount = (int)Ship.UltimateOption.COUNT;
    private Dictionary<int, bool> unlockedDic = new Dictionary<int, bool>();

    private void Update()
    {
        //Makes Sure the debug variables are always up to date
        readInColor = (int)saveStorage.activeColor;
        readInBody = (int)saveStorage.activeBody;
        readInPrimary = (int)saveStorage.activePrimary;
        readInSecondary = (int)saveStorage.activeSecondary;
        readInUlt = (int)saveStorage.activeUltimate;
    }


    /*
    Awake initializes the unlocked dictionary, then checks to see if the files exists for save data.
    If the file exists then it reads the data in.
    If the file doesn't exists it sets default values for the customization and then saves out the new file.
    */ 
    private void Awake()
    {
        dictionaryInitialize();
        if (!File.Exists(fileName))
        {
            setDefaults();
            saveData();
        }
        
        readData();
        
    }

    //Called to return the value of the player's currency.
    public int GetCurrency()
    {
        return currency;
    }

    //Called to update the player's currency.
    public void SetCurrency(int num)
    {
        currency += num;
    }

    //Called by the shop to get the values of the dictionary so it knows what is unlocked.
    public Dictionary<int, bool> getUnlockedDic()
    {
        return unlockedDic;
    }

    //Called to set the unlocked dicstionary to the updated version from the shop.
    public void setUnlockedDic(Dictionary<int, bool> diction)
    {
        unlockedDic = diction;
    }

    //Called when the games needs the active settings of the player from the save file.
    public void getActiveSettings(out Ship.playerShipSettings settings)
    {
        settings.activeColor = saveStorage.activeColor;
        settings.activeBody = saveStorage.activeBody;
        settings.activePrimary = saveStorage.activePrimary;
        settings.activeSecondary = saveStorage.activeSecondary;
        settings.activeUltimate = saveStorage.activeUltimate;
    }

    //Called when a change is made to the player ship and needs to be saved.
    public void saveShipPrefs(Ship.playerShipSettings ship)
    {
        saveStorage.activeColor = ship.activeColor;
        saveStorage.activeBody = ship.activeBody;
        saveStorage.activePrimary = ship.activePrimary;
        saveStorage.activeSecondary = ship.activeSecondary;
        saveStorage.activeUltimate = ship.activeUltimate;
    }

    //public methods
    /*
    Writes out the date thats is saved to saveStorage to the file.
    */ 
    public void saveData()
    {
        var writer = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate));
        writer.WriteLine(header);
        writer.WriteLine((int)saveStorage.activeColor);
        writer.WriteLine((int)saveStorage.activeBody);
        writer.WriteLine((int)saveStorage.activePrimary);
        writer.WriteLine((int)saveStorage.activeSecondary);
        writer.WriteLine((int)saveStorage.activeUltimate);
        writer.WriteLine(currency);
        writer.WriteLine("----Shop Data----");
        writer.WriteLine(unlockedCount);
        for (int i = 0; i < unlockedCount; i++)
        {
            writer.WriteLine(unlockedDic[i]);
        }
        writer.Close();
    }

    /*
    Reads in the data and stores it into saveStorage
    */
    public void readData()
    {
        var reader = new StreamReader(File.Open(fileName, FileMode.Open));
        if (reader.ReadLine() == header)
        {
            saveStorage.activeColor = (Ship.ColorOption)int.Parse(reader.ReadLine());
            saveStorage.activeBody = (Ship.BodyOption)int.Parse(reader.ReadLine());
            saveStorage.activePrimary = (Ship.PrimaryOption)int.Parse(reader.ReadLine());
            saveStorage.activeSecondary = (Ship.SecondaryOption)int.Parse(reader.ReadLine());
            saveStorage.activeUltimate = (Ship.UltimateOption)int.Parse(reader.ReadLine());
            currency = int.Parse(reader.ReadLine());
            reader.ReadLine();
            unlockedCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < unlockedCount; i++)
            {
                unlockedDic[i] = bool.Parse(reader.ReadLine());
            }
        }
        reader.Close();
    }

    //Sets the default values of saveStorage.
    void setDefaults()
    {
        saveStorage.activeColor = Ship.ColorOption.Red;
        saveStorage.activeBody = Ship.BodyOption.Body1;
        saveStorage.activePrimary = Ship.PrimaryOption.FusionBlaster;
        saveStorage.activeSecondary = Ship.SecondaryOption.Missiles;
        saveStorage.activeUltimate = Ship.UltimateOption.MineLauncher;
    }

    //Initializes default values for the unlocked dictionary.
    void dictionaryInitialize()
    {
        for (int i = 0; i < unlockedCount; i++)
        {
            if (i == (int)saveStorage.activeColor)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activeBody)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activePrimary)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activeSecondary)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activeUltimate)
            {
                unlockedDic.Add(i, true);
            }
            else
            {
                unlockedDic.Add(i, false);
            }
        }
    }
}
