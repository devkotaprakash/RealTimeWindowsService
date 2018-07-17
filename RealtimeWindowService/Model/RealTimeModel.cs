using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProject.Model
{

    public class SkillActivityList
    {
        public DateTime lastPollTime { get; set; }
        public Skillactivity[] skillActivity { get; set; }
    }

    public class Skillactivity
    {
        public DateTime serverTime { get; set; }
        public int businessUnitId { get; set; }
        public int agentsACW { get; set; }
        public int agentsAvailable { get; set; }
        public int agentsIdle { get; set; }
        public int agentsLoggedIn { get; set; }
        public int agentsUnavailable { get; set; }
        public int agentsWorking { get; set; }
        public int campaignId { get; set; }
        public string campaignName { get; set; }
        public int contactsActive { get; set; }
        public DateTime? earliestQueueTime { get; set; }
        public string emailFromAddress { get; set; }
        public bool isActive { get; set; }
        public int inSLA { get; set; }
        public bool isNaturalCalling { get; set; }
        public bool isOutbound { get; set; }
        public int mediaTypeId { get; set; }
        public string mediaTypeName { get; set; }
        public int outSLA { get; set; }
        public int queueCount { get; set; }
        public int serviceLevel { get; set; }
        public int serviceLevelGoal { get; set; }
        public int serviceLevelThreshold { get; set; }
        public string skillName { get; set; }
        public int skillId { get; set; }
        public int skillQueueCount { get; set; }
        public int personalQueueCount { get; set; }
        public int parkedCount { get; set; }
    }

    public class SkillRealTimeModel
    {
        public int AgentsACW { get; set; }
        public int AgentsAvailable { get; set; }
        public int AgentsIdle { get; set; }
        public int AgentsLoggedIn { get; set; }
        public int AgentsUnavailable { get; set; }
        public int AgentsWorking { get; set; }
        public int ContactsActive { get; set; }
        public int InSLA { get; set; }
        public int OutSLA { get; set; }
        public int QueueCount { get; set; }
        public int ServiceLevel { get; set; }
        public int ServiceLevelGoal { get; set; }
        public int ServiceLevelThreshold { get; set; }
        public string CampaignName { get; set; }

        public int CampaignId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int HoldTime { get; set; }
        public int LongestQueue { get; set; }


    }



}
