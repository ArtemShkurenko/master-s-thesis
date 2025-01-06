using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Staff.Models;
using AutoMapper;
using ClosedXML.Excel;
using Staff.Core;
using Microsoft.ML;

namespace Staff.DALL
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : StaffData, new()
    {
        public List<TEntity> _records = new List<TEntity>();
        private ITransformer _model;
        public IEnumerable<TEntity> GetAll()
            {
                return _records;
            }

        public void LoadFromExcel(string filePath, bool hasStaffLevel)
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                foreach (var row in rows.Skip(1))
                {
                    var data = new TEntity
                    {
                        Name = int.Parse(row.Cell(1).Value.ToString()),
                        Sales = float.Parse(row.Cell(2).Value.ToString()),
                        Conversion = float.Parse(row.Cell(3).Value.ToString()),
                        Checks = float.Parse(row.Cell(4).Value.ToString()),
                        Area = float.Parse(row.Cell(5).Value.ToString())
                        
                    };

                    if (hasStaffLevel)
                    {
                        data.StaffLevel = float.Parse(row.Cell(6).Value.ToString());
                    }

                    _records.Add(data);
                }
            }
        }
        public void SaveModel(ITransformer model)
        {
            _model = model;
        }

        public ITransformer GetModel() => _model;
        public void ClearTrainingData()
        {
            _records.Clear();  
        }
    }
}

