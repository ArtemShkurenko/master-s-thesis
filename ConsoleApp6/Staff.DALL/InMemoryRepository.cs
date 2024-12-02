using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Staff.Models;
using AutoMapper;
using ClosedXML.Excel;
using Staff.Core;

namespace Staff.DALL
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : StaffData, new()
    {
        public List<TEntity> _records = new List<TEntity>();
            //private int idCounter = 1;
        private readonly IMapper _mapper;
        public InMemoryRepository()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntity, TEntity>();
            });
            _mapper = config.CreateMapper();
        }
        private TEntity DeepCopy(TEntity entity)
        {
            return _mapper.Map<TEntity, TEntity>(entity);
        }
        /*internal StaffPrediction DeepCopy(StaffPrediction entity)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap(typeof(StaffPrediction), typeof(StaffPrediction));
                });
                var mapper = config.CreateMapper();
                var copyEntity = mapper.Map<StaffPrediction, StaffPrediction>(entity);
                return (copyEntity);
            }
        public StaffPrediction GetRecordById(int Id)
         {
             var entity = _records.FirstOrDefault(x => x.Id.Equals(Id));
             return entity;
         }
         public void Create(StaffPrediction entity)
         {
             var entityCopy = DeepCopy(entity);
             entityCopy.Id = idCounter++;
             _records.Add(entityCopy);
         }*/
        public IEnumerable<TEntity> GetAll()
            {
                return _records.Select(DeepCopy);
            }

        public void LoadFromExcel(string filePath, bool hasStaffLevel)
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                foreach (var row in rows.Skip(1)) // Пропуск заголовков
                {
                    var data = new TEntity
                    {
                        Sales = float.Parse(row.Cell(1).Value.ToString()),
                        Conversion = float.Parse(row.Cell(2).Value.ToString()),
                        Checks = float.Parse(row.Cell(3).Value.ToString()),
                        Area = float.Parse(row.Cell(4).Value.ToString())
                    };

                    if (hasStaffLevel)
                    {
                        data.StaffLevel = float.Parse(row.Cell(5).Value.ToString());
                    }

                    _records.Add(data);
                }
            }
        }
    }
}

