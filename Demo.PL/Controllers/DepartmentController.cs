﻿using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly iUnitOfWork _unitOfWork;

        public DepartmentController(iUnitOfWork unitOfWork) // ask clr for creating object from class implement interface idepartmentrepository
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();

            return View(departments);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if(ModelState.IsValid) // server side validation
            {
                await _unitOfWork.DepartmentRepository.AddAsync(department);
                int result = await _unitOfWork.CompleteAsync();
                if(result >0)
                    TempData["Message"] = "Department Is Created";

                return RedirectToAction(nameof(Index));
            }
            return View(department);
            
        }

        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest(); // Status Code 400

            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id.Value);
            if (department is null)
                return NotFound();

            return View(ViewName,department);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            //if(id is null)
            //    return BadRequest();

            //var department = _departmentRepository.GetById(id.Value);
            //if(department is null)
            //    return NotFound();

            //return View(department);
            return await Details(id , "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department,[FromRoute] int id)
        {
            if (id != department.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepartmentRepository.Update(department);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
               
            }

            return View(department);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            //if(id is null)
            //    return BadRequest();

            //var department = _departmentRepository.GetById(id.Value);
            //if (department is null)
            //    return NotFound();

            //return View(department);
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department,[FromRoute] int id)
        {
            if (id != department.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepartmentRepository.Delete(department);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {

                    ModelState.AddModelError(string.Empty,ex.Message);
                    return View(department);

                }
            }
            return View(department);
        }
    }
}
