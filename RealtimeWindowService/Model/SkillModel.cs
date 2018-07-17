using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProject.Model
{
    public class ServiceLevelSummaryList
    {
        public Servicelevelsummary[] serviceLevelSummaries { get; set; }
    }

    public class Servicelevelsummary
    {
        public int BusinessUnitId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int ContactsWithinSLA { get; set; }
        public int ContactsOutOfSLA { get; set; }
        public int TotalContacts { get; set; }
        public decimal ServiceLevel { get; set; }
    }

    public class SkillSummaryList
    {
        public Skillsummary[] skillSummaries { get; set; }
    }

    public class Skillsummary
    {
        public string businessUnitId { get; set; }
        public string businessUnitName { get; set; }
        public string abandonCount { get; set; }
        public string abandonRate { get; set; }
        public string agentsAcw { get; set; }
        public string agentsAvailable { get; set; }
        public string agentsIdle { get; set; }
        public string agentsLoggedIn { get; set; }
        public string agentsUnavailable { get; set; }
        public string agentsWorking { get; set; }
        public string averageHandleTime { get; set; }
        public string averageInqueueTime { get; set; }
        public string averageSpeedToAnswer { get; set; }
        public string averageTalkTime { get; set; }
        public string averageWrapTime { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int contactsActive { get; set; }
        public int contactsHandled { get; set; }
        public int contactsOffered { get; set; }
        public int contactsQueued { get; set; }
        public int contactsOutOfSLA { get; set; }
        public int contactsWithinSLA { get; set; }
        public int holdTime { get; set; }
        public string isOutbound { get; set; }
        public int longestQueueDur { get; set; }
        public string mediaTypeId { get; set; }
        public string mediaTypeName { get; set; }
        public string queueCount { get; set; }
        public int serviceLevel { get; set; }
        public string skillName { get; set; }
        public string skillId { get; set; }
        public string serviceLevelGoal { get; set; }
        public string serviceLevelThreshold { get; set; }
        public string totalContactTime { get; set; }
        public string dials { get; set; }
        public string connects { get; set; }
        public string connectsAHT { get; set; }
        public string rightPartyConnects { get; set; }
        public string rightPartyConnectsAHT { get; set; }
    }
}
