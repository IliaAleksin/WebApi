using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Data.SqlClient;

namespace WebApi.Models
{
    public class TaskContext : DbContext
    {
        //DB-контекст для модели данных
        public TaskContext(DbContextOptions<TaskContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        //Используем для подключения БД
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        //Использование сервера SQL + строка подключение
            optionsBuilder.UseSqlServer(@"Server=(localdb)\\mssqllocaldb;Database=TaskDB;Trusted_Connection=True;");
        }
        */
        public DbSet<TaskModel> taskModel { get; set; }
    }
}