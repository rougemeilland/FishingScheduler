﻿using System;

namespace FFXIVFishingScheduleViewer.Models
{
    class FishMemoChangedEventArgs
        : EventArgs
    {
        public FishMemoChangedEventArgs(Fish fish, FishingSpot fishingSpot)
        {
            Fish = fish;
            FishingSpot = fishingSpot;
        }

        public Fish Fish { get; }
        public FishingSpot FishingSpot { get; }
    }
}
