using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : iUnitOfWork
    {
        private readonly MvcAppG01DbContext _dbContext;

        public IEmployeeRepository EmployeeRepository { get; set; }
        public IDepartmentRepository DepartmentRepository { get; set; }

        public UnitOfWork(MvcAppG01DbContext dbContext) // Ask CLR for object from DbContext
        {
            EmployeeRepository = new EmployeeRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            _dbContext = dbContext;
        }

        public  async Task<int> CompleteAsync()
             =>  await _dbContext.SaveChangesAsync();
        

        public void Dispose()
            => _dbContext.Dispose();
        
    }
}
