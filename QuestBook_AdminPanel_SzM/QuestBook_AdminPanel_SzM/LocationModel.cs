using System;
using System.Collections.Generic;
using System.Text;

namespace QuestBook_AdminPanel_SzM
{
    internal class LocationModel
    {
        public LocationModel(Guid id, string locationName, string adress, string description)
        {
            Id = id;
            LocationName = locationName;
            Adress = adress;
            Description = description;
        }

        public Guid Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
