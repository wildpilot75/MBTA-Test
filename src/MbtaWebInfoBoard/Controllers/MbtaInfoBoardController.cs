namespace MbtaWebInfoBoard.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [ApiController]
    [Route("[controller]")]
    public class MbtaInfoBoardController : ControllerBase
    {
        readonly MbtaInfoBoardDataProvider _data;

        public MbtaInfoBoardController(MbtaInfoBoardDataProvider data)
        {
            _data = data;
        }

        // GET: api/MbtaInfoBoard
        [HttpGet]
        public IEnumerable<ScheduleItemDataModel> Get() => 
            _data.Get();

        // GET: api/MbtaInfoBoard/5
        [HttpGet("{id}", Name = "Get")]
        public IEnumerable<ScheduleItemDataModel> Get(string id)
        {
            return _data.Get(id);
        }
    }
}
