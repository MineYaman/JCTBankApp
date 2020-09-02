using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JCTBank.BLL.Abstract;
using JCTBank.BLL.Concrete;
using JCTBank.DAL.Concrete.EfRepository;
using JCTBank.Entity;
using System.Web.Mvc;

namespace JCTBank.WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        IAccountService _accountService = new AccountManager(new EfRepositoryAccountDal());
        ICustomerService _customerService = new CustomerManager(new EfRepositoryCustomerDal());

        // GET: Account}

        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccount(Account model)
        {
            string LoginTCKN = Session["LoginUser"].ToString();

            var result = _customerService.GetByTCKN(LoginTCKN);

            var accounts = _accountService.GetAccountNo().Where(x=>x.AccountNo==result.CustomerNo).OrderBy(x=>x.AdditionalAccountNo);
            if (accounts==null)
            {
                model.IsDelete = false;
                model.AdditionalAccountNo = 1001;
                model.AccountNo = result.CustomerNo;
                model.Balance = 0;
                model.CustomerId = result.Id;
                _accountService.Create(model);
                return RedirectToAction("ListAccount", "Account");
            }
            else
            {
                model.IsDelete = false;
                model.AdditionalAccountNo = 1001 + accounts.Count();
                model.AccountNo = result.CustomerNo;
                model.Balance = 0;
                model.CustomerId = result.Id;
                _accountService.Create(model);
                return RedirectToAction("ListAccount", "Account");
            } 
        }

        [HttpGet]
        public ActionResult DeleteAccount(Account model)
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();

                var result = _customerService.GetByTCKN(LoginTCKN);
                var account = _accountService.GetById(model.Id);

                if (ModelState.IsValid) 
                {
                    if (account.Balance == 0)
                    {
                        model.IsDelete = true;
                        model.AccountNo = result.CustomerNo;
                        model.CustomerId = result.Id;
                        _accountService.Delete(model);
                        return RedirectToAction("ListAccount", "Account");
                    }
                    else
                    {
                        throw new Exception("Hesabınızı silmek için lütfen bakiyenizi sıfırlayınız");
                    }
                }
                else
                {
                    throw new Exception("Eksik Bilgi Girildi.. Tekrar Deneyiniz..");
                   
                }
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return RedirectToAction("ListAccount", "Account");

            }
        }

        public ActionResult ListAccount()
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();
                var result = _customerService.GetByTCKN(LoginTCKN);
                var listAccounts = _accountService.GetAll().Where(x => x.IsDelete == false && x.AccountNo==result.CustomerNo).ToList();

                    return View(listAccounts);
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("ListAccount");
            }
        }

        [HttpGet]
        public ActionResult WithdrawMoney(int id)
        {
            var model = _accountService.GetById(id);
            if (model == null)
            {
                throw new Exception(" Hesap Seçilmedi..");
            }
            return View("WithdrawMoney", model);
        }

        [HttpPost]
        public ActionResult WithdrawMoney(Account model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string LoginTCKN = Session["LoginUser"].ToString();
                    var result = _customerService.GetByTCKN(LoginTCKN);
                    var _account = _accountService.GetById(model.Id);

                    if (_account != null)
                    {
                        if (model.Balance < 0)
                        {
                            throw new Exception("Hesap Bakiyeniz 0'dan az olamaz yeni bir tutar giriniz!");
                            
                        }
                        else
                        {
                            if(_account.Balance >= model.Balance)
                            {
                                _account.Balance = (_account.Balance - model.Balance);
                                _accountService.Update(_account);
                                return RedirectToAction("ListAccount", "Account");
                            }
                            else
                            {
                                throw new Exception("Hesap Bakiyesi yetersiz..");
                            }     
                        }
                    }
                    else
                    {
                        throw new Exception("Böyle bir hesap bulunamadı.");
                    }
                }
                else
                {
                    return View("ListAccount");
                }
            }

            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("WithdrawMoney");
            }

        }

        public ActionResult DepositMoney(int id)
        {
            var model = _accountService.GetById(id);
            if (model == null)
            {
                throw new Exception(" Hesap Seçilmedi..");
            }
            return View("DepositMoney", model);
        }

        [HttpPost]
        public ActionResult DepositMoney(Account model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string LoginTCKN = Session["LoginUser"].ToString();
                    var result = _customerService.GetByTCKN(LoginTCKN);
                    var _account = _accountService.GetById(model.Id);

                    if (_account != null)
                    {
                        if (model.Balance < 0)
                        {
                            throw new Exception("Hesap Bakiyeniz 0'dan az olamaz yeni bir tutar giriniz!");
                        }

                        else
                        {
                            _account.Balance = (_account.Balance + model.Balance);
                            _accountService.Update(_account);
                            return RedirectToAction("ListAccount", "Account");
                        }

                    }
                    else
                    {
                        throw new Exception("Böyle bir hesap bulunamadı.");
                    }

                }
                else
                {
                    return View("ListAccount");
                }
            }

            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("DepositMoney");
            }

        }
    }
}