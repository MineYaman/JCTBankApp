using JCTBank.BLL.Abstract;
using JCTBank.BLL.Concrete;
using JCTBank.DAL.Concrete.EfRepository;
using JCTBank.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JCTBank.WebApp.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        ICustomerService _customerService = new CustomerManager(new EfRepositoryCustomerDal());
        // GET: Customer

        public ActionResult ShowProfile()
        {
            Customer loginCustomer = Session["LoginUser"] as Customer;
            string LoginTCKN = Session["LoginUser"].ToString();

            var result = _customerService.GetByTCKN(LoginTCKN);
            if (result == null)
            {
                return View();

            }
            var profileCustomer = _customerService.GetAll().Where(x => x.CustomerNo == result.CustomerNo).ToList();

            return View(profileCustomer);
        }

        public ActionResult UpdateCustomer(int Id)
        {
            var result = _customerService.GetById(Id);
            return View("UpdateCustomer",result);
        }

        [HttpPost]
        public ActionResult UpdateCustomer(Customer model)
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();
                var result = _customerService.GetByTCKN(LoginTCKN);
                model.TCKN = result.TCKN;
                model.CustomerNo = result.CustomerNo;
                result.Name = model.Name;
                result.Surname = model.Surname;
                result.TCKN = model.TCKN;
                result.CustomerNo = model.CustomerNo;
                result.IsDelete = false;
                result.Id = result.Id;
                result.PhoneNo = model.PhoneNo;
                result.Address = model.Address;
                result.Email = model.Email;
                result.Password = model.Password;
                result.RePassword = model.RePassword;
                _customerService.Update(model);
                return RedirectToAction("ShowProfile");

            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("UpdateCustomer", model);
            }
        }
    }
}