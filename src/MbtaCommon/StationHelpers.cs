namespace MbtaCommon
{
    using System;
    using System.Collections.Generic;

    public static class StationHelpers
    {
        static Dictionary<Stations, string> _station_ids = new Dictionary<Stations, string>
        {
            { Stations.SouthStation, "South Station" },
            { Stations.NorthStation, "North Station" }
        };

        public static string GetStationId(Stations station)
        {
            if (!_station_ids.ContainsKey(station))
            {
                throw new ArgumentException($"cannot find id for Station {station.ToString()}");
            }

            return _station_ids[station];
        }

        public static string GetCurrentTime()
        {
            var dateTime = DateTime.Now;
            var stringTime = dateTime.ToString("HH:mm");
            if(stringTime.StartsWith("00:"))
            {
                stringTime = stringTime.Replace("00:", "24:");
            }

            return stringTime;
        }
    }
}
