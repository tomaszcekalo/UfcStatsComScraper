﻿using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class FighterDetails
    {
        public string Record { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Reach { get; set; }
        public string Stance { get; set; }
        public string DateOfBirth { get; set; }

        //Career Statistics:
        public string StrikesLandedPerMinute { get; set; }

        public string StrikingAccuracy { get; set; }
        public string StrikesAbsorbedPerMinute { get; set; }
        public string StrikeDefence { get; set; }
        public string AverageTakedownsLandedPer15Minutes { get; set; }
        public string TakedownAccuracy { get; set; }
        public string TakedownDefense { get; set; }
        public string AverageSugmissionsAttemptedPer15Minutes { get; set; }
        public IEnumerable<object> Fights { get; set; }
    }
}