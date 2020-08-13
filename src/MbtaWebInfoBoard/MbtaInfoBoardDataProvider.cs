namespace MbtaWebInfoBoard
{
    using MbtaCommon;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class MbtaInfoBoardDataProvider : IDisposable
    {
        CancellationTokenSource _source;
        readonly IDictionary<string, IDictionary<string, ScheduleItemDataModel>> _schedules = new Dictionary<string, IDictionary<string, ScheduleItemDataModel>>();

        public MbtaInfoBoardDataProvider(Subscriber subscriber)
        {
            _source = new CancellationTokenSource();

            if (!subscriber.IsInitialized)
            {
                subscriber.Intialize(_source.Token);

                subscriber.Subscribe(StationHelpers.GetStationId(Stations.SouthStation), HandleSouthStationScheduleRecords);
                subscriber.Subscribe(StationHelpers.GetStationId(Stations.NorthStation), HandleNorthStationScheduleRecords);
            }

            _schedules.Add(StationHelpers.GetStationId(Stations.SouthStation), new Dictionary<string, ScheduleItemDataModel>());
            _schedules.Add(StationHelpers.GetStationId(Stations.NorthStation), new Dictionary<string, ScheduleItemDataModel>());
        }

        public IEnumerable<ScheduleItemDataModel> Get() =>
            _schedules[StationHelpers.GetStationId(Stations.SouthStation)].Values
            .Concat(_schedules[StationHelpers.GetStationId(Stations.NorthStation)].Values);

        public IEnumerable<ScheduleItemDataModel> Get(string id)
        {
            return (from item in _schedules[id] orderby item.Value.DateTime ascending select item.Value).Take(10);
        }

        public void HandleNorthStationScheduleRecords(string scheduleRecord)
        {
            Console.WriteLine("Handling record for NorthStation");
            var record = new ScheduleItemDataModel(JsonConvert.DeserializeObject<ScheduleItem>(scheduleRecord));
            var stationSchedules = _schedules[StationHelpers.GetStationId(Stations.NorthStation)];

            if (stationSchedules.ContainsKey(record.Id))
            {
                stationSchedules[record.Id] = record;
            }
            else
            {
                stationSchedules.Add(record.Id, record);
            }

            CleanSchedule(stationSchedules);
        }

        public void HandleSouthStationScheduleRecords(string scheduleRecord)
        {
            Console.WriteLine("Handling record for South Station");
            var record = new ScheduleItemDataModel(JsonConvert.DeserializeObject<ScheduleItem>(scheduleRecord));
            var stationSchedules = _schedules[StationHelpers.GetStationId(Stations.SouthStation)];

            if (stationSchedules.ContainsKey(record.Id))
            {
                stationSchedules[record.Id] = record;
            }
            else
            {
                stationSchedules.Add(record.Id, record);
            }

            CleanSchedule(stationSchedules);
        }

        void CleanSchedule(IDictionary<string, ScheduleItemDataModel> schedule)
        {
            foreach (var item in schedule)
            {
                var scheduleItem = item.Value;
                var currentTime = DateTime.Now;
                if (DateTime.Compare(scheduleItem.DateTime, currentTime) < 0)
                {
                    schedule.Remove(scheduleItem.Id);
                }
            }
        }

        public void Dispose()
        {
            _source.Cancel();
        }
    }
}
