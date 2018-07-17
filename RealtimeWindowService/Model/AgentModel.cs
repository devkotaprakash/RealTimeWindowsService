using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProject.Model
{
    public class AgentStateList
    {
        public Agentstate[] agentStates { get; set; }
    }

    public class Agentstate
    {
        public int agentId { get; set; }
        public int agentStateId { get; set; }
        public string agentStateName { get; set; }
        public int businessUnitId { get; set; }
        public object contactId { get; set; }
        public bool isActive { get; set; }
        public bool isACW { get; set; }
        public bool isOutbound { get; set; }
        public string firstName { get; set; }
        public object fromAddress { get; set; }
        public string lastName { get; set; }
        public DateTime lastPollTime { get; set; }
        public DateTime lastUpdateTime { get; set; }
        public object mediaName { get; set; }
        public object mediaType { get; set; }
        public int openContacts { get; set; }
        public object outStateDescription { get; set; }
        public object outStateId { get; set; }
        public int skillId { get; set; }
        public object skillName { get; set; }
        public object startDate { get; set; }
        public int stationId { get; set; }
        public object stationPhoneNumber { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public object toAddress { get; set; }
        public string userName { get; set; }
    }
}
