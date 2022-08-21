using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskContext _context;
        public TaskController(TaskContext context)
        {
            _context = context;
        }
        //GET обработка запроса на получение всех данных по данной модели TaskModel (метод 1)

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModelDTO>>> GetAll()
        {
            return await _context.taskModel
                .Select(x => TaskModelDTO(x))
                .ToListAsync();
        }

        //GET обработка запроса на получение данных по фильтру по параметру Id (метод 1)
        [HttpGet("FilterByID/{id}")]
        public async Task<ActionResult<TaskModelDTO>> GetByID(long id)
        {
            var taskModelDTOs = await _context.taskModel.FindAsync(id);

            if (taskModelDTOs == null)
            {
                return NotFound();
            }

            return TaskModelDTO(taskModelDTOs);
        }

        //GET обработка запроса на получение данных по фильтру по параметру code (метод 1)
        [HttpGet("FilterByCode/{code}")]
        public async Task<ActionResult<IEnumerable<TaskModelDTO>>> GetByCode(int code)
        {
            var taskModelDTOs = await _context.taskModel.Where(x => x.code == code).Select(g => TaskModelDTO(g)).ToListAsync();
            if (taskModelDTOs == null)
            {
                return NotFound();
            }
            return taskModelDTOs;         
        }

        //GET обработка запроса на получение данных по фильтру по параметру value (метод 1)
        [HttpGet("FilterByValue/{value}")]
        public async Task<ActionResult<IEnumerable<TaskModelDTO>>> GetByValue(string value)
        {
            var taskModelDTOs = await _context.taskModel.Where(x => x.value == value).Select(g => TaskModelDTO(g)).ToListAsync();
            if (taskModelDTOs == null)
            {
                return NotFound();
            }
            return taskModelDTOs;
        }

        //GET обработка запроса на получение данных по комбо-фильтру по параметру Id, code и value (метод 1)
        [HttpGet("FilterCombo/")]
        public async Task<ActionResult<IEnumerable<TaskModelDTO>>> GetByComboFilter(long? id, int? code, string? value)
        {
            List<TaskModelDTO> taskModelDTOs = new List<TaskModelDTO>(); 
            if (id != null && id !=0)
            {
                taskModelDTOs = await _context.taskModel.Where(x => x.id == id).Select(g => TaskModelDTO(g)).ToListAsync();
            }
                else if (code!=null)
                {
                taskModelDTOs = await _context.taskModel.Where(x => x.code == code).Select(g => TaskModelDTO(g)).ToListAsync();
                }
                    else if (value!=null)
                    {
                taskModelDTOs = await _context.taskModel.Where(x => x.value == value).Select(g => TaskModelDTO(g)).ToListAsync();
                    }
            
            if (taskModelDTOs.Count<1)
            {
                return NotFound();
            }
            return taskModelDTOs;
        }

        //POST обработка запроса на размещение данных по модели TaskModelDTO (Метод 2)
        [HttpPost]
        public async Task<ActionResult<TaskModel>> PostTodoItem(string jsonString)
        {
            List<TaskModel> taskModel = new List<TaskModel>();
            {
                //Парсим входную JSON-строку в лист из словарей (словари для пар ключ-значение)
                try
                {
                    var list = JsonConvert.DeserializeObject<List<Dictionary<object, object>>>(jsonString);

                    for (int i = 0; i < list.Count;i++)
                    {
                        //Добавляем значения из листа со словарями в taskModel1
                        TaskModel taskModel1 = new TaskModel();
                        taskModel1.code = Convert.ToInt32(list.ElementAt(i).ElementAt(0).Key);
                        taskModel1.value = list.ElementAt(i).ElementAt(0).Value.ToString();
                        taskModel.Add(taskModel1);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }                
            }
            if (taskModel != null)
            {
                //Очистим БД перед размещением данных
                _context.taskModel.RemoveRange(_context.taskModel);

                //Записываем данные в БД через DBcontext
                for (int i = 0; i < taskModel.Count; i++)
                {
                    _context.taskModel.Add(taskModel[i]);
                }

                //Сортируем данные в БД
                _context.taskModel.OrderBy(p => p.code);

                //Сохраняем данные в БД
                await _context.SaveChangesAsync();

                return Ok();
            }
            else return BadRequest("Nothing to post");
        }       

        //Небольшой обмен данными между экземплярами моделей
        private static TaskModelDTO TaskModelDTO(TaskModel taskmodel) =>
            new TaskModelDTO
            {
                code = taskmodel.code,
                value = taskmodel.value
            };
    }
}