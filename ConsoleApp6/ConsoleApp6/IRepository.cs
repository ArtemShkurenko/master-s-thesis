using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Staff.Models;

namespace Staff.Core
{
    public interface IRepository<TEntity>
    {
       // TEntity GetRecordById(int Id);
        IEnumerable<TEntity> GetAll();
        void LoadFromExcel(string filePath, bool hasStaffLevel);
    }
 }

