using JCTBank.BLL.Abstract;
using JCTBank.BLL.Concrete;
using JCTBank.DAL.Concrete.EfRepository;
using JCTBank.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JCTBank.WebApp.Controllers
{

    public class HomeController : Controller
    {
        ICustomerService _customerService = new CustomerManager(new EfRepositoryCustomerDal());

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        // GET: Home
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string TCKN , string  Password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _customer = _customerService.Login(TCKN, Password);
                    
                    FormsAuthentication.SetAuthCookie("Login", false);
                    Session.Add("LoginUser", TCKN);
                    /*Session["Id"] = _customer.Id;
                    Session["TCKN"] = _customer.TCKN;
                    Session["CustomerNo"] = _customer.CustomerNo;*/
                    return RedirectToAction("ShowProfile", "Customer");
                }
                else
                {
                    return View("Login");
                }
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("Login");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer model)
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                    return View("Register");
                }
                else
                {
                    var _tckn = _customerService.GetByTCKN(model.TCKN);
                    if (_tckn == null)
                    {
                        Random rastgele = new Random();
                        model.IsDelete = false;
                        model.CustomerNo = rastgele.Next(1, 1000);
                        _customerService.Create(model);
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        throw new Exception("TC Numaranız daha önce kaydedilmiştir.");
                    }
                        
                }
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("Register");
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }

    }
}