using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]/:[id]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var openConnection = new MySqlConnection(connectionString);

            openConnection.Open();
            
            var command = new MySqlCommand("select * from lesson_signal", openConnection);
                
            var table = new DataTable();
            var adapter = new MySqlDataAdapter(command);
            adapter.Fill(table);
            
            openConnection.Close();

            var LessonSignalsDto = new List<LessonSignalDto>();

            foreach (DataRow row in table.Rows)
            {
                LessonSignalsDto.Add(new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["time_stamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                });
            }

            
            return LessonSignalsDto;
        }

        [HttpGet("{Id}")]

        public LessonSignalDto ShowSignal(long id)
        {   
            
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var openConnection = new MySqlConnection(connectionString);

            openConnection.Open();
            
            var command = new MySqlCommand("select * from lesson_signal where id = " + id.ToString(), openConnection);
                    
            var table = new DataTable();
            var adapter = new MySqlDataAdapter(command);
            adapter.Fill(table);
            
            openConnection.Close();

            var row = table.Rows;

            var LessonsignalDto = new LessonSignalDto
            {
                Timestamp = (DateTime) row[0]["time_stamp"],
                Type = (LessonSignalType) row[0]["signal_type"],
                UserId = (string) row[0]["user_id"]
            };

            return LessonsignalDto;
            
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();


            var connectionString = _configuration.GetConnectionString("BotDatabase");
       
            var openConnection= new MySqlConnection(connectionString);
            openConnection.Open();
            
            var command = new MySqlCommand("insert into lesson_signal (signal_type, user_id) " +
                                           "values (@signalType, @userId)", openConnection);
            
            command.Parameters.AddWithValue("@signalType", signalType);
            command.Parameters.AddWithValue("@userId", userId);
            
            command.ExecuteNonQuery();
            
            openConnection.Close();
            return Accepted();
        }
        
        [HttpDelete("{Id}")]
        
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add code to delete a record with the given id
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var  openConnection= new MySqlConnection(connectionString);
            
            openConnection.Open();
		
            var command = new MySqlCommand("delete from lesson_signal where id = " + id.ToString(), openConnection);

            command.ExecuteNonQuery();
            
            openConnection.Close();
            
            return Accepted();
        }
    }
}
