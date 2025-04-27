using UnityEngine;

public static class InjurySystem
{
    public enum RiskLevel { NONE, LOW, MEDIUM, HIGH, VERY_HIGH }

    public struct InjuryChanceData
    {
        public float minorChance;
        public float moderateChance;
        public float severeChance;

        public float totalChance => minorChance + moderateChance + severeChance;
        public RiskLevel risk;
    }

    public static InjuryChanceData CalculateChance(PlayerController.InjuryStatus currentInjury, float playerStamina)
    {
        InjuryChanceData data = new InjuryChanceData();

        // For Severe Injury, already handled elsewhere (can't start chopping)
        if (currentInjury == PlayerController.InjuryStatus.Severe)
        {
            data.risk = RiskLevel.VERY_HIGH;
            data.minorChance = 0f;
            data.moderateChance = 0f;
            data.severeChance = 0f;
            return data;
        }

        // Calculate total chance of injury based on stamina
        float totalInjuryChance = 0f;
        if (currentInjury == PlayerController.InjuryStatus.Healthy)
        {
            if (playerStamina <= 30f)
            {
                totalInjuryChance = 0.35f; // 35% chance of injury
                
                data.minorChance = 0.40f * totalInjuryChance;
                data.moderateChance = 0.40f * totalInjuryChance;
                data.severeChance = 0.20f * totalInjuryChance;
                
                data.risk = RiskLevel.HIGH;
            }
            else if (playerStamina <= 40f)
            {
                totalInjuryChance = 0.25f; // 25% chance of injury
                
                data.minorChance = 0.55f * totalInjuryChance;
                data.moderateChance = 0.35f * totalInjuryChance;
                data.severeChance = 0.10f * totalInjuryChance;
                
                data.risk = RiskLevel.MEDIUM;
            }
            else if (playerStamina <= 50f)
            {
                totalInjuryChance = 0.15f; // 15% chance of injury
                
                data.minorChance = 0.65f * totalInjuryChance;
                data.moderateChance = 0.30f * totalInjuryChance;
                data.severeChance = 0.05f * totalInjuryChance;
                
                data.risk = RiskLevel.LOW;
            }
            else
            {
                totalInjuryChance = 0f; // No injury chance
                data.risk = RiskLevel.LOW;
            }
        }
        else if (currentInjury == PlayerController.InjuryStatus.Minor)
        {
            if (playerStamina <= 30f)
            {
                totalInjuryChance = 0.50f; // 50% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.70f * totalInjuryChance;
                data.severeChance = 0.30f * totalInjuryChance;
                
                data.risk = RiskLevel.VERY_HIGH;
            }
            else if (playerStamina <= 40f)
            {
                totalInjuryChance = 0.40f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.70f * totalInjuryChance;
                data.severeChance = 0.30f * totalInjuryChance;
                
                data.risk = RiskLevel.HIGH;
            }
            else if (playerStamina <= 50f)
            {
                totalInjuryChance = 0.20f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.70f * totalInjuryChance;
                data.severeChance = 0.30f * totalInjuryChance;
                
                data.risk = RiskLevel.MEDIUM;
            }
            else if (playerStamina <= 65f)
            {
                totalInjuryChance = 0.15f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.70f * totalInjuryChance;
                data.severeChance = 0.30f * totalInjuryChance;
                
                data.risk = RiskLevel.LOW;
            }
            else if (playerStamina <= 75f)
            {
                totalInjuryChance = 0.10f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.80f * totalInjuryChance;
                data.severeChance = 0.20f * totalInjuryChance;
                
                data.risk = RiskLevel.LOW;
            }
            else
            {
                totalInjuryChance = 0.05f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.90f * totalInjuryChance;
                data.severeChance = 0.10f * totalInjuryChance;
                
                data.risk = RiskLevel.LOW;
            }
        }
        else if (currentInjury == PlayerController.InjuryStatus.Moderate)
        {
            if (playerStamina <= 30f)
            {
                totalInjuryChance = 0.80f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.0f * totalInjuryChance;
                data.severeChance = 1f * totalInjuryChance;
                
                data.risk = RiskLevel.VERY_HIGH;
            }
            else if (playerStamina <= 40f)
            {
                totalInjuryChance = 0.50f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.0f * totalInjuryChance;
                data.severeChance = 1f * totalInjuryChance;
                
                data.risk = RiskLevel.VERY_HIGH;
            }
            else if (playerStamina <= 50f)
            {
                totalInjuryChance = 0.40f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.0f * totalInjuryChance;
                data.severeChance = 1f * totalInjuryChance;
                
                data.risk = RiskLevel.HIGH;
            }
            else if (playerStamina <= 65f)
            {
                totalInjuryChance = 0.30f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.0f * totalInjuryChance;
                data.severeChance = 1f * totalInjuryChance;
                
                data.risk = RiskLevel.MEDIUM;
            }
            else if (playerStamina <= 75f)
            {
                totalInjuryChance = 0.20f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.0f * totalInjuryChance;
                data.severeChance = 1f * totalInjuryChance;
                
                data.risk = RiskLevel.MEDIUM;
            }
            else
            {
                totalInjuryChance = 0.15f; // 35% chance of injury
                
                data.minorChance = 0.0f * totalInjuryChance;
                data.moderateChance = 0.0f * totalInjuryChance;
                data.severeChance = 1f * totalInjuryChance;
                
                data.risk = RiskLevel.LOW;
            }
        }
        
        return data;
        
    }
}
