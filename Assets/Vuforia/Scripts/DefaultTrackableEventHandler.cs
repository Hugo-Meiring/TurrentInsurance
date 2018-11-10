/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;
using System.Collections.Generic;

public enum AttackType 
{
    RAIL_GUN = 20,
    ROCKET = 15,
    GRENADE = 10,
    NUKE = 1000000,
    NONE = 0
}

public enum PlayerType {
    ONE,
    TWO 
}

public class Player {
    public PlayerType Type { get; set; }
    public int moneyPool { get; set; }
    public int shield { get; set; }

    public Player(PlayerType _type, int moneyPool, int shield) 
    {
        Debug.Log(">>> Creating player");
        Type = _type;
        this.moneyPool = moneyPool;
        this.shield = shield;
    }

    public int DamagePlayer(int damage) {
        Debug.Log(">>>> damaging player with " + damage);
        shield -= damage;
        return shield;
    }

    public int SpendMoney(int money) {
        moneyPool -= money;
        return money;
    }

    public bool isAlive() {
        return shield > 0;
    }

    public bool hasMoneyLeft() {
        return moneyPool > 0;
    }
}

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    // Players
    public static Player playerOne = new Player(PlayerType.ONE, 100000, 100);
    public static Player playerTwo = new Player(PlayerType.TWO, 100000, 100);
    static public bool hasPlayedTurn = false;
    public static Player currentPlayer;
    public static AttackType currentAttackType = AttackType.NONE;
    static public StateManager sm = TrackerManager.Instance.GetStateManager();

    //protected IEnumerable<TrackableBehaviour> cardsInScene;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        currentPlayer = playerOne;
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    protected virtual void Update()
    {
        if (NumberOfCards() > 2 && IsActionCardInScene()) {
            Debug.Log("Player " + currentPlayer + " is playing turn");
            hasPlayedTurn = true;  // set true
            //SwitchPlayer();
        }

        if (NumberOfCards() <= 2 && !IsActionCardInScene() && hasPlayedTurn)
        {
            //Apply damage
            currentPlayer.DamagePlayer((int) currentAttackType);
            Debug.Log("Player " + currentPlayer.Type.ToString() + " shild: " + currentPlayer.shield);
            TextMesh tp = GameObject.FindObjectOfType(typeof(HealthDisplay)).
            TextMesh tm = gameObject.GetComponent(typeof(TextMesh)) as TextMesh;
            ChangeShield(currentPlayer.shield);
            SwitchPlayer();
            hasPlayedTurn = false;
        }
            
            //else
            //{
                //hasPlayedTurn = false;   // set false
            //}
        //}
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    public int NumberOfCards() {
        //StateManager sm = TrackerManager.Instance.GetStateManager();

        IList<TrackableBehaviour> tbs = (System.Collections.Generic.IList<Vuforia.TrackableBehaviour>)sm.GetActiveTrackableBehaviours();
        return tbs.Count;
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }


    protected virtual void OnTrackingLost()
    {

        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    protected virtual bool IsActionCardInScene()
    {
        IList<TrackableBehaviour> cardsInScene = (System.Collections.Generic.IList<Vuforia.TrackableBehaviour>)GetCardsInScene();

        foreach (TrackableBehaviour card in cardsInScene)
        {
            bool isActionCard = GetAttackType(card) != AttackType.NONE;
            if (isActionCard)
            {
                currentAttackType = GetAttackType(card);
                return true;
            }
                
        }

        return false;
     }

    protected virtual IEnumerable<TrackableBehaviour> GetCardsInScene()
    {
        return sm.GetActiveTrackableBehaviours();
    }

    protected virtual AttackType GetAttackType(TrackableBehaviour card)
    {
        string cardName = card.TrackableName;
        switch (cardName)
        {
            case "Astronaut_scaled":
                return AttackType.RAIL_GUN;
            case "Fissure_scaled":
                return AttackType.ROCKET;
            case "Drone_scaled":
                return AttackType.GRENADE;
            case "Oxygen_scaled":
                return AttackType.NUKE;
            default:
                return AttackType.NONE;
        }
    }

    protected virtual void SwitchPlayer() {
        if (currentPlayer.Type == PlayerType.ONE) 
        {
            currentPlayer = playerTwo;
        } else {
            currentPlayer = playerOne;
        }
    }

    protected virtual void checkHealth()
    {
        if (!currentPlayer.isAlive())
        {
            //
        }
    }

    #endregion // PROTECTED_METHODS
}
