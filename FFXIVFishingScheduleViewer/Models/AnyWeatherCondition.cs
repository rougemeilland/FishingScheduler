﻿namespace FFXIVFishingScheduleViewer.Models
{
    class AnyWeatherCondition
        : IWeatherFishingConditionElement
    {
        public double DifficultyValue => 1.0;

        public string Description => null;

        public EorzeaDateTimeHourRegions FindRegions(EorzeaDateTimeHourRegions wholeRange, bool useFishEye)
        {
            return wholeRange;
        }
    }
}
