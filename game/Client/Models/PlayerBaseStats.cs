using game.Shared.DTOs.Stats;

namespace game.Client.Models;

public class PlayerBaseStats
{
    public int BaseStrength { get; set; }
    public int BaseDexterity { get; set; }
    public int BaseLuck { get; set; }
    public int BaseAggression { get; set; }
    public int BaseWisdom { get; set; }
    public int GetBaseStat(string statName) => statName switch
    {
        "STR" => BaseStrength,
        "DEX" => BaseDexterity,
        "LUCK" => BaseLuck,
        "AGR" => BaseAggression,
        "WIS" => BaseWisdom,
        _ => 0
    };
}