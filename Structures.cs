using System.Collections.Generic;

namespace Structures
{
    struct Species
    {
        public string SpeciesName;
        public string CommonName;
        public string FishLength;
        public string FishFate;
        public string FishNotes;
    }

    struct Drop
    {
        public int DropNumber;
        public string ObserverFishers;
        public string StartGPS;
        public string EndGPS;
        public string Depth;
        public string Notes;
        public string TimeDown;
        public string TimeUp;
        public List<Species> SpeciesList;
    }

    /* for transferring a drop */
    struct SpecialDrop
    {
        public List<string> list;
        public Drop drop;
    }

    /* for Ending the trip and trasnferring all drops */
    struct SpecialTrip
    {
        public List<string> list;
        public List<Drop> drops;
    }

    struct Trip
    {
        public int TripNumber;
        public string Date;
        public string NumberOfObservers;
        public string ObserverInitials;
        public string NumberOfAnglers;
        public string NumberOfObserverAnglers;
        public string CaptainName;
        public string VesselName;
        public string PortName;
        public string Condition_Sea;
        public string Condition_Sky;
        public string Condition_Wind;
        public string Condition_Swell;
        public string DepartureTime;
        public string ArrivalTime;
        public string Notes;
        public List<Drop> DropsList;
    }
}

