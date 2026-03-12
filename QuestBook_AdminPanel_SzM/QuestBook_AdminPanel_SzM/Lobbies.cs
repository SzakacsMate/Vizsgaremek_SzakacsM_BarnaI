using System;
using System.Collections.Generic;
using System.Text;

namespace QuestBook_AdminPanel_SzM
{
    internal class Lobbies
    {
        public Lobbies(string dm, string ttType, string locationName, DateTime startDate, DateTime endDate, int playerLimit, int playerCount)
        {
            Dm = dm;
            TtType = ttType;
            this.locationName = locationName;
            StartDate = startDate;
            EndDate = endDate;
            PlayerLimit = playerLimit;
            PlayerCount = playerCount;
        }

        public string Dm { get; set; } 
        public string TtType { get; set; } 

        public string locationName { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public int PlayerLimit { get; set; } 
        public int PlayerCount { get; set; } 
    }
}
