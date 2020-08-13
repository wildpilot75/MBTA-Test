namespace MbtaWebInfoBoard
{
    using MbtaCommon;
    using System;

    public class ScheduleItemDataModel
    {
        public ScheduleItemDataModel(ScheduleItem item)
        {
            Id = item.Id;
            Carrier = item.Carrier;
            Direction = item.Direction.ToString();
            DateTime = item.Time;
            Time = item.Time.ToString("HH:mm tt");
            Destination = item.Destination;
            Train = item.TrainNumber;
            if(String.IsNullOrWhiteSpace(item.TrackNumber))
            {
                Track = "TBD";
            }
            else
            {
                Track = item.TrackNumber;
            }

            if (String.IsNullOrWhiteSpace(item.Status))
            {
                Status = "On Time";
            }
            else
            {
                Status = item.Status;
            }
        }

        public string Id { get; set; }

        public string Carrier { get; set; }

        public string Direction { get; set; }

        public DateTime DateTime { get; set; }

        public string Time { get; set; }

        public string Destination { get; set; }

        public string Train { get; set; }

        public string Track { get; set; }

        public string Status { get; set; }
    }
}
