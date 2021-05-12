using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings/Debug Values")]
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected bool changesColor;
    [SerializeField] protected bool ultimate;
    [SerializeField] protected float rateOfFire;
    [SerializeField] protected float currUltValue = 0;
    [SerializeField] protected bool paused;

    [SerializeField] protected Ship shipMain;
    [SerializeField] protected Transform CrossHairTransform;
    [SerializeField] protected MeshRenderer[] meshes;
    [SerializeField] protected GameObject gunOrigin;
    [SerializeField] protected AudioSource weaponSound;

    //Returns the stat sheet for the weapons.
    public WeaponStats GetStats()
    {
        return stats;
    }

    //Return the rate of fire for the weapon.
    public float GetROF()
    {
        return rateOfFire;
    }

    /*
     * Checks if the weapon is an Ultimate or not.
     * Based of choice it pulls the ship's main body from a different spot.
     */
    public virtual void Awake()
    {
        if (ultimate)
        {
            shipMain = GetComponentInParent<WeaponLink>().getShipMain();
            CrossHairTransform = GetComponentInParent<WeaponLink>().getCrossHairTransform();

        }
        else
        {
            shipMain = GetComponentInParent<Transform>().GetComponentInParent<WeaponLink>().getShipMain();
            CrossHairTransform =  GetComponentInParent<Transform>().GetComponentInParent<WeaponLink>().getCrossHairTransform();
        }
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    /*
     * Update checks if the game is paused or not and ticks of the rate of fire timer/Ult charge timer.
     */
    public virtual void Update()
    {
        if (paused)
        {
            return;
        }

        rateOfFire += Time.deltaTime;

        if (ultimate)
        {
            if (currUltValue >= stats.ultChargeTime)
            {
                currUltValue = stats.ultChargeTime;
            }
            else
            {
                currUltValue += Time.deltaTime * stats.ultGainMultiplier;
            }
        }
    }

    //Checks if weapon can change color and then chanes it.
    public virtual void setColor(Material mat)
    {
        if (changesColor)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].material = mat;
            }
        }
    }

    //Sets the color on enable and adds the pause and unpause event into the event manager.
    private void OnEnable()
    {
        if (!GameManagerBase.Instance.isMulti())
        {
            //add pause and unpause to event
            EventManager.pauseEvent += Pause;
            EventManager.unPauseEvent += unPause;
        }
        if (changesColor)
        {
            if (shipMain)
            {
                setColor(shipMain.getActiveColor());
            }
        }
    }

    //Removes the pause and unpause event from the manager.
    private void OnDisable()
    {
        if (!GameManagerBase.Instance.isMulti())
        {
            //remove from event
            EventManager.pauseEvent -= Pause;
            EventManager.unPauseEvent -= unPause;
        }
    }

    //Is called when the player uses the fire key and fires differently based on what gamemode its in.
    public virtual void Fire()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            MultiplayerFire();
        }
        else
        {
            SingleplayerFire();
        }
    }

    //Used for a cheat code to fill ult.
    public virtual void fillUlt()
    {
        currUltValue = stats.ultChargeTime;
    }

    //Returns the current ult value for the player ui. 
    public virtual float getCurrentUltValue()
    {
        return currUltValue;
    }

    //Gets the mac value the ult charge can be for the player ui.
    public virtual float getUltChargeTime()
    {
        return stats.ultChargeTime;
    }

    //Abstract functions for the scripts that inherit this.
    public abstract void MultiplayerFire();
    public abstract void SingleplayerFire();

    //Pause and Unpause functions for the event manager.
    protected virtual void Pause()
    {
        paused = true;
    }

    protected virtual void unPause()
    {
        paused = false;
    }
}
