using System;
using System.Collections.Generic;
using System.Text;

namespace QuestBook_AdminPanel_SzM
{
    internal class Lobbies
    {
        public Lobbies(Guid id, string lobbyName, string dm, string ttType, string locationName, DateTime startDate, DateTime endDate, int playerLimit, int playerCount, int playerMin, string status)
        {
            Id = id;
            LobbyName = lobbyName;
            Dm = dm;
            TtType = ttType;
            this.locationName = locationName;
            StartDate = startDate;
            EndDate = endDate;
            PlayerLimit = playerLimit;
            PlayerCount = playerCount;
            PlayerMin = playerMin;
            Status = status;
        }

        public Guid Id { get; set; }
        public string LobbyName { get; set; }
        public string Dm { get; set; } 
        public string TtType { get; set; } 

        public string locationName { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public int PlayerLimit { get; set; } 
        public int PlayerCount { get; set; }
        public int PlayerMin { get; set; } 
        public string Status { get; set; } 
    }
}
