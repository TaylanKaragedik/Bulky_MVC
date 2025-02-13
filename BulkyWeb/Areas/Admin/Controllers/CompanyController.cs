﻿using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        public IActionResult Upsert(int? id)
        {
            
            if (id == null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company company = _unitOfWork.Company.Get(u => u.Id == id);
				return View(company);
			}
            
        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj) 
        {
            
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);

                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "The company created succesfully";
                return RedirectToAction("Index");
            }
            else
            {
				return View(CompanyObj);
			}
            
        }
        public IActionResult DeletePost(int? id)
        {
            Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
            if (obj == null)
            { return NotFound(); }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "The deleted successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        [HttpDelete]
		public IActionResult Delete(int? id)
		{
            var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Delete Succesful" });
		}
		#endregion
	}
}
