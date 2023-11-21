using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly MvcAppG01DbContext _dbContext;

        public EmployeeRepository(MvcAppG01DbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Employee> GetEmployeesByAdress(string adress)
            => _dbContext.Employees.Where(E => E.Address == adress);

        public IQueryable<Employee> GetEmployeesByName(string SearchValue)
            => _dbContext.Employees.Where(E=>E.Name.ToLower().Trim().Contains(SearchValue.ToLower().Trim()));
        
    }
}
