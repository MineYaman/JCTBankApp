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
    public class CreditController : Controller
    {
        ICreditService _creditService = new CreditManager(new EfRepositoryCreditDal());

        // GET: Credit
        public ActionResult CreditTransactions()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreditTransactions(Credit model)
        {
            if (ModelState.IsValid)
            {
                _creditService.Create(model);
                return RedirectToAction("ListAccount", "Account");
            }
            else
            {
                return RedirectToAction("ListAccount", "Account");
            }
        }
    }
}