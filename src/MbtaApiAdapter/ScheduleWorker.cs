namespace MbtaApiAdapter
{
    using MbtaCommon;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class ScheduleWorker : IDisposable
    {
        CancellationToken _token;
        RootConfiguration _configuration;
        Publisher _publisher;

        public ScheduleWorker(RootConfiguration configuration, CancellationToken token, Publisher publisher)
        {
            _configuration = configuration;
            _token = token;
            _publisher = publisher;
        }

        public void DoWork()
        {
            var taskFactory = new TaskFactory(_token);
            taskFactory.StartNew(() =>
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_configuration.ApiUrl);

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("X-API-Key", _configuration.ApiKey);

                    while (true)
                    {
                        var southStationScheduleData = CallApi(
                            httpClient, 
                            _configuration.Schedule, 
                            StationHelpers.GetStationId(Stations.SouthStation),
                            _configuration.PageSize, 
                            StationHelpers.GetCurrentTime()).GetAwaiter().GetResult();

                        var southStationSchedule = ParseSchedule(southStationScheduleData, httpClient);
                        PublishSchedule(southStationSchedule, StationHelpers.GetStationId(Stations.SouthStation));

                        var northStationScheduleData = CallApi(
                            httpClient,
                            _configuration.Schedule,
                            StationHelpers.GetStationId(Stations.NorthStation),
                            _configuration.PageSize,
                            StationHelpers.GetCurrentTime()).GetAwaiter().GetResult();

                        var northStationSchedule = ParseSchedule(northStationScheduleData, httpClient);
                        PublishSchedule(northStationSchedule, StationHelpers.GetStationId(Stations.NorthStation));

                        Thread.Sleep(30000);
                    }
                }
            });
        }

        async Task<string> CallApi(HttpClient httpClient, string path, string id, int? pageLimit, string minTime)
        {
            path = path.Replace("{Id}", id);

            if(pageLimit.HasValue)
            {
                path = path.Replace("{PageSize}", pageLimit.ToString());
            }

            if(!String.IsNullOrEmpty(minTime))
            {
                path = path.Replace("{MinTime}", minTime);
            }
            var httpResponse = await httpClient.GetAsync(path);
            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsStringAsync();
            }

            throw new HttpRequestException($"Call to MBTA API failed {httpResponse.StatusCode.ToString()}");
        }

        Dictionary<string, ScheduleItem> ParseSchedule(string scheduleData, HttpClient httpClient)
        {
            var schedule = new Dictionary<string, ScheduleItem>();
            var predictions = new Dictionary<string, JToken>();
            var trips = new Dictionary<string, JToken>();

            var data = JSonHelpers.GetData(JObject.Parse(scheduleData));

            var included = JObject.Parse(scheduleData)["included"];

            foreach (var include in included.Children())
            {
                if(include["type"].ToString() == "prediction")
                {
                    predictions.Add(include["id"].ToString(), include);
                }

                if(include["type"].ToString() == "trip")
                {
                    trips.Add(include["id"].ToString(), include);
                }
            }

            foreach (var childToken in data.Children())
            {
                var scheduleId = childToken["id"];
                var relationships = childToken["relationships"];
                var attributes = childToken["attributes"];
                var route = relationships["route"];
                

                var arrivalTime = attributes["arrival_time"];
                var departureTime = attributes["departure_time"];

                var scheduleItem = new ScheduleItem();
                scheduleItem.Id = scheduleId.ToString();

                if (String.IsNullOrWhiteSpace(arrivalTime.ToString()))
                {
                    scheduleItem.Direction = Direction.Departure;
                    scheduleItem.Time = DateTime.Parse(departureTime.ToString());
                }
                else
                {
                    scheduleItem.Direction = Direction.Arrival;
                    scheduleItem.Time = DateTime.Parse(arrivalTime.ToString());
                }

                var trip = relationships["trip"];
                scheduleItem.TripId = JSonHelpers.GetRelationshipId(trip);

                var prediction = relationships["prediction"];
                scheduleItem.PredictionId = JSonHelpers.GetRelationshipId(prediction);

                schedule.Add(scheduleId.ToString(), scheduleItem);
            }

            schedule = PopulateTripInfo(schedule, trips);
            schedule = PopulatePredictionInfo(schedule, predictions);

            return schedule;
        }

        Dictionary<string, ScheduleItem> PopulateTripInfo(Dictionary<string, ScheduleItem> schedule, Dictionary<string, JToken> trips)
        {

            foreach (var scheduleItem in schedule)
            {
                if (!String.IsNullOrWhiteSpace(scheduleItem.Value.TripId) && trips.TryGetValue(scheduleItem.Value.TripId, out var trip))
                {
                    var attributes = trip["attributes"];

                    scheduleItem.Value.Destination = attributes["headsign"].ToString();
                    scheduleItem.Value.TrainNumber = attributes["name"].ToString();
                }

            }

            return schedule;
        }

        Dictionary<string, ScheduleItem> PopulatePredictionInfo(Dictionary<string, ScheduleItem> schedule, Dictionary<string, JToken> predictions)
        {
            foreach (var scheduleItem in schedule)
            {
                if (!String.IsNullOrWhiteSpace(scheduleItem.Value.PredictionId) && predictions.TryGetValue(scheduleItem.Value.PredictionId, out var prediction))
                {
                    var attributes = prediction["attributes"];

                    scheduleItem.Value.Status = attributes["status"].ToString();
                    var relationships = prediction["relationships"];
                    var stop = relationships["stop"];
                    var stopId = JSonHelpers.GetRelationshipId(stop);

                    if(stopId.Contains('-'))
                    {
                        scheduleItem.Value.TrackNumber = stopId.Substring(stopId.IndexOf('-') +1);
                    }
                }
            }

            return schedule;
        }

        void PublishSchedule(Dictionary<string, ScheduleItem> schedule, string station)
        {
            foreach (var scheduleItem in schedule)
            {
                _publisher.Publish(station, JsonConvert.SerializeObject(scheduleItem.Value));
            }

            Console.WriteLine($"Published {schedule.Count} messagers to topic {station}");
        }

        public void Dispose()
        {
            
        }
    }
}
