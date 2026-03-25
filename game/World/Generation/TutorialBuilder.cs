using System;
using System.Collections.Generic;
using System.Linq;

namespace Game;

class TutorialRule
{
    public Func<bool> Condition;
    public string Text;
}

class TutorialBuilder
{
    private readonly Board board;
    private readonly Player player;

    private List<TutorialRule> rules;

    public TutorialBuilder(Board board, Player player)
    {
        this.board = board;
        this.player = player;

        BuildRules();
    }

    public List<string> GenerateInstructions()
    {
        return rules
            .Where(r => r.Condition())
            .Select(r => r.Text)
            .ToList();
    }

    private void BuildRules()
    {
        rules = new List<TutorialRule>
        {
            new TutorialRule
            {
                Condition = ShouldShowMovementTutorial,
                Text = "Move using WASD keys."
            },

            new TutorialRule
            {
                Condition = ShouldShowPickupTutorial,
                Text = "Press E to pick up items and weapons."
            },

            new TutorialRule
            {
                Condition = ShouldShowEquipmentTutorial,
                Text = "Equip weapons using D1 (left hand) and D2 (right hand)."
            },

            new TutorialRule
            {
                Condition = ShouldShowCurrencyTutorial,
                Text = "Collect currency to buy items."
            },

            new TutorialRule
            {
                Condition = ShouldShowWallsTutorial,
                Text = "Walls block movement."
            },

            new TutorialRule
            {
                Condition = ShouldShowInventoryTutorial,
                Text = "Use Up/Down arrows to select items and Q to drop them."
            },

            new TutorialRule
            {
                Condition = ShouldShowDropTutorial,
                Text = "Press L to drop the left-hand weapon, R to drop the right-hand weapon."
            }
        };
    }

    // ================= CONDITIONS =================
    
    private bool ShouldShowMovementTutorial()
    {
        return HasEmptyTiles();
    }
    
    private bool ShouldShowWallsTutorial()
    {
        // zwraca true jesli gracz soi obok sciany
        int px = player.X;
        int py = player.Y;

        // Sprawdzamy sąsiednie pola (Góra, Dół, Lewo, Prawo)
        return IsWall(px + 1, py) || IsWall(px - 1, py) || 
               IsWall(px, py + 1) || IsWall(px, py - 1);
    }
    
    private bool ShouldShowPickupTutorial()
    {
        // tylko jesli na polu na ktorym gracz stoi sa jakies przedmioty
        return board.items[player.X, player.Y].Count != 0;
    }
    
    private bool ShouldShowEquipmentTutorial()
    {
        // jesli cos w ekwipunku i rece puste
        bool hasAnythingInHands = player.LeftHandItem != null || player.RightHandItem != null;
        bool hasWeaponInInventory = player.inventory.OfType<OneHandedWeapon>().Any() || player.inventory.OfType<TwoHandedWeapon>().Any();
        return hasWeaponInInventory && !hasAnythingInHands;
    }
    
    private bool ShouldShowCurrencyTutorial()
    {
        return HasCurrency();
    }
    
    private bool ShouldShowInventoryTutorial()
    {
        return player.inventory.Count > 0;
    }
    
    private bool ShouldShowDropTutorial()
    {
        return (player.LeftHandItem != null || player.RightHandItem != null);
    }
    
    private bool HasEmptyTiles()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            if (board.grid[i, j] == GlobalConstants.EMPTY)
                return true;

        return false;
    }

    private bool HasCurrency()
    {
        // zeby nie sugerowac graczowi ze ma latwy dostep do pieniedzy (jak w przypadku wyswietlania tylko wtedy kiedy stanie na walute)
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            foreach (var item in board.items[i, j])
                if (item is Currency)
                    return true;

        return false;
    }

    private bool IsWall(int x, int y)
    {
        if (x < 0 || y < 0 || x >= GlobalConstants.WIDTH || y >= GlobalConstants.HEIGHT) 
            return false;
        return board.grid[x, y] == GlobalConstants.WALL;
    }
}