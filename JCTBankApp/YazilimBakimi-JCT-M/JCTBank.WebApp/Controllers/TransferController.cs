using JCTBank.BLL.Abstract;
using JCTBank.BLL.Concrete;
using JCTBank.DAL.Concrete.EfRepository;
using JCTBank.Entity;
using JCTBank.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JCTBank.WebApp.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        ITransferService _transferService = new TransferManager(new EfRepositoryTransferDal());
        IAccountService _accountService = new AccountManager(new EfRepositoryAccountDal());
        ICustomerService _customerService = new CustomerManager(new EfRepositoryCustomerDal());

        // GET: Transfer
        public ActionResult ListTransfer()
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();
                var result = _customerService.GetByTCKN(LoginTCKN);
                var listTransfers = _transferService.GetAll().Where(x => x.SendingCustomerNo == result.CustomerNo).ToList();

                return View(listTransfers);
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("ListTransfer");
            }
        }

        public ActionResult MoneyTransfer()
        {
            string LoginTCKN = Session["LoginUser"].ToString();
            var result = _customerService.GetByTCKN(LoginTCKN);
            var listAccounts = _accountService.GetAll().Where(x => x.IsDelete == false && x.AccountNo == result.CustomerNo).ToList();
            TransferModel transferModel = new TransferModel { Accounts = listAccounts };

            return View(transferModel);
        }

        [HttpPost]
        public ActionResult MoneyTransfer(TransferModel model,int sendId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sendCustomer = _accountService.GetById(sendId);
                    if (sendCustomer != null && sendCustomer.Balance > 0)
                    {
                        var receivedCustomer = _customerService.GetByNo(model.ReceiverNo);
                        if (receivedCustomer != null)
                        {
                            var received = _accountService.GetByCustomerNo(model.ReceiverNo, model.AdditionalReceiverNo);
                            if (received != null)
                            {
                                if (sendCustomer.Balance >= model.Balance && model.Balance > 0)
                                {
                                    _transferService.Create(new Transfer()
                                    {
                                        TransferType = "MoneyTransfer",
                                        WebOrMobil = "Web",
                                        Date = DateTime.Now,
                                        SendingCustomerNo = sendCustomer.AccountNo,
                                        AdditionalSendingCustomerNo = sendCustomer.AdditionalAccountNo,
                                        ReceiverNo = received.AccountNo,
                                        AdditionalReceiverNo = received.AdditionalAccountNo,
                                        Balance = model.Balance
                                    });

                                    _accountService.Update(new Account()
                                    {
                                        Id = sendCustomer.Id,
                                        AccountNo = sendCustomer.AccountNo,
                                        AdditionalAccountNo = sendCustomer.AdditionalAccountNo,
                                        Balance = sendCustomer.Balance - model.Balance,
                                        CustomerId = sendCustomer.CustomerId,
                                        IsDelete = sendCustomer.IsDelete
                                    });


                                    _accountService.Update(new Account()
                                    {
                                        Id = received.Id,
                                        AccountNo = received.AccountNo,
                                        AdditionalAccountNo = received.AdditionalAccountNo,
                                        Balance = received.Balance + model.Balance,
                                        CustomerId = received.CustomerId,
                                        IsDelete = received.IsDelete
                                    });

                                    return RedirectToAction("ListTransfer", "Transfer");
                                }
                                else
                                {
                                    throw new Exception("Bakiye Sıfırın Altında YA DA HESABINIZDA YETERLİ MİKTAR YOK.  Havale Gerçekleştirilemedi..");
                                }
                            }

                            else
                            {
                                throw new Exception("Hesap Numarası Bulunamadı. Lütfen kontrol ediniz..");
                            }
                        }

                        else
                        {
                            throw new Exception("Alan Müşteri Numarası Bulunamadı. Lütfen kontrol ediniz..");
                        }
                    }

                    else
                    {
                        throw new Exception("Hesap Bulunamadı veya Hesaptaki Bakiyeniz 0 ın Altında. Lütfen kontrol ediniz..");
                    }
                }

                else
                {
                    throw new Exception("Eksik Bilgi Girildi.. Tekrar Deneyiniz..");
                }
            }
            catch (Exception error)
            {
                var sendCustomer = _accountService.GetById(sendId);
                var listAccounts = _accountService.GetAll().Where(x => x.IsDelete == false && x.AccountNo == sendCustomer.AccountNo).ToList();
                model = new TransferModel { Accounts = listAccounts };

                ModelState.AddModelError("", error.Message);
                return View("MoneyTransfer",model);
            }

            
        }

        public ActionResult Virement()
        {
            string LoginTCKN = Session["LoginUser"].ToString();
            var result = _customerService.GetByTCKN(LoginTCKN);
            var listAccounts = _accountService.GetAll().Where(x => x.IsDelete == false && x.AccountNo == result.CustomerNo).ToList();

            TransferModel transferModel = new TransferModel { Accounts = listAccounts };

            return View(transferModel);
        }

        [HttpPost]
        public ActionResult Virement(int sendId, int receivedId, TransferModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var sendCustomer = _accountService.GetById(sendId);
                    var receivedCustomer = _accountService.GetById(receivedId);
                    if (sendCustomer != null && sendCustomer.Balance > 0)
                    {
                        if (receivedCustomer != null)
                        {
                            if(sendCustomer.AdditionalAccountNo != receivedCustomer.AdditionalAccountNo)
                            {
                                if(sendCustomer.Balance >= model.Balance && model.Balance>0)
                                {
                                    _transferService.Create(new Transfer()
                                    {
                                        TransferType = "Virement",
                                        WebOrMobil = "Web",
                                        Date = DateTime.Now,
                                        SendingCustomerNo = sendCustomer.AccountNo,
                                        AdditionalSendingCustomerNo = sendCustomer.AdditionalAccountNo,
                                        ReceiverNo = receivedCustomer.AccountNo,
                                        AdditionalReceiverNo = receivedCustomer.AdditionalAccountNo,
                                        Balance = model.Balance
                                    });

                                    _accountService.Update(new Account()
                                    {
                                        Id = sendCustomer.Id,
                                        AccountNo = sendCustomer.AccountNo,
                                        AdditionalAccountNo = sendCustomer.AdditionalAccountNo,
                                        Balance = sendCustomer.Balance - model.Balance,
                                        CustomerId = sendCustomer.CustomerId,
                                        IsDelete = sendCustomer.IsDelete
                                    });


                                    _accountService.Update(new Account()
                                    {
                                        Id = receivedCustomer.Id,
                                        AccountNo = receivedCustomer.AccountNo,
                                        AdditionalAccountNo = receivedCustomer.AdditionalAccountNo,
                                        Balance = receivedCustomer.Balance + model.Balance,
                                        CustomerId = receivedCustomer.CustomerId,
                                        IsDelete = receivedCustomer.IsDelete
                                    });

                                    return RedirectToAction("ListTransfer", "Transfer");
                                }
                                else
                                {
                                    throw new Exception("Hesabınızda yeterli miktar yok ya da girdiğiniz bakiye 0 ın altında ...");
                                }

                            }

                            else
                            {
                                throw new Exception("Gönderen Hesap Numarasıyla Alan Hesap Numarası Aynı Olamaz..");
                            }
                        }

                        else
                        {
                            throw new Exception("Alan Müşteri Numarası Bulunamadı. Lütfen kontrol ediniz..");
                        }
                    }

                    else
                    {
                        throw new Exception("Hesap Bulunamadı veya Hesaptaki Bakiyeniz 0 ın Altında. Lütfen kontrol ediniz..");
                    }
                }

                else
                {
                    throw new Exception("Eksik Bilgi Girildi.. Tekrar Deneyiniz..");
                }
            }
            catch (Exception error)
            {
                var sendCustomer = _accountService.GetById(sendId);
                var listAccounts = _accountService.GetAll().Where(x => x.IsDelete == false && x.AccountNo == sendCustomer.AccountNo).ToList();
                model = new TransferModel { Accounts = listAccounts };

                ModelState.AddModelError("", error.Message);
                return View("Virement", model);
            }
        }
    }
}