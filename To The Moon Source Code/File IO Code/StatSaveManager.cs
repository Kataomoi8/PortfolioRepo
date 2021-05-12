using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StatSaveManager : MonoBehaviour
{
    public struct StatData 
    {
        public int Kills;
        public int Deaths;
        public int Wins;
        public int Lose;
        public float TimePlayed;
        public Ship.BodyOption favoriteBody;
        public Ship.PrimaryOption favoritePrim;
        public Ship.SecondaryOption favoriteSec;
        public Ship.UltimateOption favoriteUlt;
    }

    //Instance object so there is only 1 in the scene at a time
    public static StatSaveManager Instance;
    public StatData saveStatData;
    
    [Header("File Name For Output")]
    [SerializeField] string fileName = "StatTest.txt";

    //Private Variables
    private Dictionary<int, int> partDictionary = new Dictionary<int, int>();
    int partCount = (int)Ship.UltimateOption.COUNT;


    /*
     * Start Checks to see if there is only 1 instance of the stat save manager.
     * If there isn't one it sets itself to the only instance.
     * If there is one, it deletes the duplicate.
     * Start also checks to see if a file exists or not for the stats.
     * If there isn't one it sets defaults and creates one and then reads in the stats.
     * If there is one it just reads in the stats.
     */
    void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        if (!File.Exists(fileName))
        {
            defaultDic();
            setDefaults();
            saveStat();
        }

        readStat();
    }

    /*
     * Sets the default values of Stats.
     */
    private void setDefaults()
    {
        saveStatData.Kills = 0;
        saveStatData.Deaths = 0;
        saveStatData.Wins = 0;
        saveStatData.Lose = 0;
        saveStatData.TimePlayed = 0;
        saveStatData.favoriteBody = Ship.BodyOption.Body1;
        saveStatData.favoritePrim = Ship.PrimaryOption.FusionBlaster;
        saveStatData.favoriteSec = Ship.SecondaryOption.Missiles;
        saveStatData.favoriteUlt = Ship.UltimateOption.MineLauncher;
    }

    //Called when a player wants to update their stats after a match.
    public void updateStatData(StatData stats)
    {
        readStat();
        saveStatData.Kills += stats.Kills;
        saveStatData.Deaths += stats.Deaths;
        saveStatData.Wins += stats.Wins;
        saveStatData.Lose += stats.Lose;
        saveStatData.TimePlayed += stats.TimePlayed;
        saveStat();
    }

    //Called after a match by the player to save out its stats from the match.
    public void saveStat()
    {
        var writer = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate));
        writer.WriteLine(saveStatData.Kills);
        writer.WriteLine(saveStatData.Deaths);
        writer.WriteLine(saveStatData.Wins);
        writer.WriteLine(saveStatData.Lose);
        writer.WriteLine(saveStatData.TimePlayed);
        writer.WriteLine((int)saveStatData.favoriteBody);
        writer.WriteLine((int)saveStatData.favoritePrim);
        writer.WriteLine((int)saveStatData.favoriteSec);
        writer.WriteLine((int)saveStatData.favoriteUlt);
        writer.WriteLine("----PART DATA----");
        writer.WriteLine(partCount);
        for (int i = 0; i < partCount; i++)
        {
            writer.WriteLine(partDictionary[i]);
        }
        writer.Close();
    }

    //Called when the player goes to check their stats to make sure they are the most up to date.
    public void readStat()
    {
        var reader = new StreamReader(File.Open(fileName, FileMode.Open));
        saveStatData.Kills = int.Parse(reader.ReadLine());
        saveStatData.Deaths = int.Parse(reader.ReadLine());
        saveStatData.Wins = int.Parse(reader.ReadLine());
        saveStatData.Lose = int.Parse(reader.ReadLine());
        saveStatData.TimePlayed = float.Parse(reader.ReadLine());
        saveStatData.favoriteBody = (Ship.BodyOption)int.Parse(reader.ReadLine());
        saveStatData.favoritePrim = (Ship.PrimaryOption)int.Parse(reader.ReadLine());
        saveStatData.favoriteSec = (Ship.SecondaryOption)int.Parse(reader.ReadLine());
        saveStatData.favoriteUlt = (Ship.UltimateOption)int.Parse(reader.ReadLine());
        reader.ReadLine();
        partCount = int.Parse(reader.ReadLine());
        for (int i = 0; i < partCount; i++)
        {
            partDictionary[i] = int.Parse(reader.ReadLine());
        }
        reader.Close();
    }

    //Returns the Part Dictionary for the stat menu
    public Dictionary<int, int> getPartDic()
    {
        return partDictionary;
    }

    //Updates the part dictionary in the manager to match the one from the stat menu
    public void setPartDic(Dictionary<int, int> diction)
    {
        partDictionary = diction;
    }

    //Updates the part dictionary to reflect how many times a part is used in game.
    public void updateDic(Ship.playerShipSettings activeParts)
    {
        for (int i = 0; i < partCount; i++)
        {
            if (i == (int)activeParts.activeColor)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activeBody)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activePrimary)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activeSecondary)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activeUltimate)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
        }

        findFavorite();
    }

    //Called when a part dictionary doesn't exist yet.
    void defaultDic()
    {
        for (int i = 0; i < partCount; i++)
        {
            partDictionary.Add(i, 0);
        }
    }

    //Sorts through the part dictionary to determine what part was used the most by the player.
    void findFavorite() 
    {
        for (int i = 8; i < partCount; i++)
        {
            if (i < (int)Ship.BodyOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoriteBody])
                {
                    saveStatData.favoriteBody = (Ship.BodyOption)i;
                }
            }
            else if (i >= (int)Ship.BodyOption.COUNT && i < (int)Ship.PrimaryOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoritePrim])
                {
                    saveStatData.favoritePrim = (Ship.PrimaryOption)i;
                }
            }
            else if(i >= (int)Ship.PrimaryOption.COUNT && i < (int)Ship.SecondaryOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoriteSec])
                {
                    saveStatData.favoriteSec = (Ship.SecondaryOption)i;
                }
            }
            else if (i >= (int)Ship.SecondaryOption.COUNT && i < (int)Ship.UltimateOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoriteUlt])
                {
                    saveStatData.favoriteUlt = (Ship.UltimateOption)i;
                }
            }
        }
    }
}
