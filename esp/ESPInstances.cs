using GameNetcodeStuff;
using pdkmMenu;
using UnityEngine;

public class Turret_ESP : ESPComponent<Turret>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Traps_Label.Value;
    }

    protected override bool ShouldShowAuras()
    {
        var s = Settings;
        return s.ESP.Value && s.Traps_Auras.Value;
    }

    protected override string GetEntityLabel() => targetObject.name;
    protected override Color TextColor => Color.red;

    protected override Color GetauraColor()
    {
        var s = Settings;
        Color color = Color.HSVToRGB(s.Traps_AurasColorHue.Value, 1f, 1f);
        color.a = s.AurasOpacity.Value;
        return color;
    }
}

public class Landmine_ESP : ESPComponent<Landmine>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Traps_Label.Value;
    }

    protected override bool ShouldShowAuras()
    {
        var s = Settings;
        return s.ESP.Value && s.Traps_Auras.Value;
    }

    protected override string GetEntityLabel() => targetObject.name;
    protected override Color TextColor => Color.red;

    protected override Color GetauraColor()
    {
        var s = Settings;
        Color color = Color.HSVToRGB(s.Traps_AurasColorHue.Value, 1f, 1f);
        color.a = s.AurasOpacity.Value;
        return color;
    }
}

public class SpikeRoofTrap_ESP : ESPComponent<SpikeRoofTrap>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Traps_Label.Value;
    }

    protected override bool ShouldShowAuras()
    {
        var s = Settings;
        return s.ESP.Value && s.Traps_Auras.Value;
    }

    protected override string GetEntityLabel() => "Spike Trap";
    protected override Color TextColor => Color.red;

    protected override Color GetauraColor()
    {
        var s = Settings;
        Color color = Color.HSVToRGB(s.Traps_AurasColorHue.Value, 1f, 1f);
        color.a = s.AurasOpacity.Value;
        return color;
    }
}

public class Player_ESP : ESPComponent<PlayerControllerB>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Player_Label.Value;
    }

    protected override bool ShouldShowAuras()
    {
        var s = Settings;
        return s.ESP.Value && s.Player_Auras.Value;
    }

    protected override string GetEntityLabel() => targetObject.playerUsername;
    protected override Color TextColor => Color.white;

    protected override Color GetauraColor()
    {
        var s = Settings;
        Color color = Color.HSVToRGB(s.Player_AurasColorHue.Value, 1f, 1f);
        color.a = s.AurasOpacity.Value;
        return color;
    }
}

public class Item_ESP : ESPComponent<GrabbableObject>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Item_Label.Value;
    }

    protected override bool ShouldShowAuras()
    {
        var s = Settings;
        return s.ESP.Value && s.Item_Auras.Value;
    }

    protected override string GetEntityLabel() => targetObject.name;
    protected override Color TextColor => Color.green;

    protected override Color GetauraColor()
    {
        var s = Settings;
        Color color = Color.HSVToRGB(s.Item_AurasColorHue.Value, 1f, 1f);
        color.a = s.AurasOpacity.Value;
        return color;
    }
}

public class Enemy_ESP : ESPComponent<EnemyAI>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Enemy_Label.Value;
    }

    protected override bool ShouldShowAuras()
    {
        var s = Settings;
        return s.ESP.Value && s.Enemy_Auras.Value;
    }

    protected override string GetEntityLabel() => targetObject.enemyType.name;
    protected override Color TextColor => Color.red;

    protected override Color GetauraColor()
    {
        var s = Settings;
        Color color = Color.HSVToRGB(s.Enemy_AurasColorHue.Value, 1f, 1f);
        color.a = s.AurasOpacity.Value;
        return color;
    }
}

public class Door_ESP : ESPComponent<EntranceTeleport>
{
    private static ESPConfig Settings => Plugin.ESPSettings;

    protected override bool ShouldShowLabel()
    {
        var s = Settings;
        return s.ESP.Value && s.Doors_Label.Value;
    }

    protected override bool ShouldShowAuras() => false;
    protected override string GetEntityLabel() => "door";
    protected override Color TextColor => Color.white;
}