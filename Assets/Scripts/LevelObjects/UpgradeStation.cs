using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

public class UpgradeStation : MonoBehaviour, Interactable
{
    Upgrade[] upgrades;

    public void Interact() {
        if (upgrades == null)
            upgrades = FindFirstObjectByType<Player>().GetUpgrades(Upgrades.UpgradeStation.BIOTECH).NotMax(FindFirstObjectByType<Player>()).RandomCount(3);

        BiotechUpgradeManager.singleton.SetUpgrades(FindFirstObjectByType<Player>(), upgrades);

        MenuController.OpenMenu(MenuController.MenuType.BIOTECH);
    }
}