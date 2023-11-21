using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Demo.PL.ViewModels;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Demo.PL.Helper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Demo.PL.Controllers
{
	[Authorize]

	public class EmployeeController : Controller
    {

        private readonly iUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(iUnitOfWork unitOfWork, // ask clr for  object from class implemnt interface IUnitOfWork
            IMapper mapper) // Ask CLR for creating object from class Implement IEmployeeRepository
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchValue))
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();

            else
                employees = _unitOfWork.EmployeeRepository.GetEmployeesByName(SearchValue);


            var MappedEmployees = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
            return View(MappedEmployees);

        }
        public IActionResult Create()
        {
            //ViewBag.Departments = _departmentRepository.GetAll();
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid)
            {
                /// Manual Mapping
                ///var MappedEmployee = new Employee()
                ///{
                ///    Name = employeeVM.Name,
                ///    Age = employeeVM.Age,
                ///    Address = employeeVM.Address,
                ///    PhoneNumber = employeeVM.PhoneNumber,
                ///    DepartmentId = employeeVM.DepartmentId,
                ///};

                employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");
                var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                await _unitOfWork.EmployeeRepository.AddAsync(MappedEmployee);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(employeeVM);
        }
        public async Task<IActionResult> Details(int? Id,string ViewName = "Details")
        {
            if (Id == null)
                return BadRequest();
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(Id.Value);
            if (employee == null)
                return BadRequest();
            var MappedEmployee = _mapper.Map<Employee,EmployeeViewModel >(employee);
            return View(ViewName, MappedEmployee);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            return await Details(Id,"Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employeeVm, [FromRoute] int Id)
        {
            if (Id != employeeVm.Id)
                return BadRequest();
            try
            {
                if (ModelState.IsValid)
                {
                    if (employeeVm.Image is not null)
                    {
                        employeeVm.ImageName = DocumentSettings.UploadFile(employeeVm.Image, "Images");
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Please upload an image.");
                        return View(employeeVm);
                    }
                    
                    var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                    _unitOfWork.EmployeeRepository.Update(MappedEmployee);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                
            }

            return View(employeeVm);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            return await Details(Id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EmployeeViewModel employeeVm,[FromRoute] int Id)
        {
            if (Id != employeeVm.Id)
                return BadRequest();

            try
            {
                if (ModelState.IsValid)
                {
                    var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                    _unitOfWork.EmployeeRepository.Delete(MappedEmployee);
                    int result = await _unitOfWork.CompleteAsync();
                    if(result > 0)
                    {
                        DocumentSettings.DeleteFile(employeeVm.ImageName, "Images");
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

            }

            return View(employeeVm);
        }


    }
}
