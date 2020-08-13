namespace MbtaCommon
{
    using System;
    using System.Text.Json.Serialization;

    public class ScheduleItem
    {
        public ScheduleItem()
        {
            Carrier = "MBTA";
        }
        public string Id { get; set; }

        [JsonIgnore]
        public string TripId { get; set; }

        [JsonIgnore]
        public string PredictionId { get; set; }

        public string Carrier { get; set; }

        public Direction Direction { get; set; }

        public DateTime Time { get; set; }

        public string Destination { get; set; }

        public string TrainNumber { get; set; }

        public string TrackNumber { get; set; }

        public string Status { get; set; }
    }
}
