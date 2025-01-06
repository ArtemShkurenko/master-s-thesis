using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Staff.Models;

namespace Staff.Core
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();
        void LoadFromExcel(string filePath, bool hasStaffLevel);
        void ClearTrainingData();
        void SaveModel(ITransformer model);
        ITransformer GetModel();
    }
 }

