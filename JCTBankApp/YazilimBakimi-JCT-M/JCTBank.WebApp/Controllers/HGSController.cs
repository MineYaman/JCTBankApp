using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JCTBank.BLL.Abstract;
using JCTBank.BLL.Concrete;
using JCTBank.DAL.Concrete.EfRepository;
using JCTBank.Entity;
using System.Web.Mvc;
using System.Globalization;

namespace JCTBank.WebApp.Controllers
{
    [Authorize]
    public class HGSController : Controller
    {
        ICustomerService _customerService = new CustomerManager(new EfRepositoryCustomerDal());
        IHGSService _HGSService = new HGSManager(new EfRepositoryHGSDal());
        // GET: HGS

        public ActionResult SaleHGS()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaleHGS(HGS model)
        {
            try
            {
                bool confirmation;
                if(model.PlateNumber != null)
                {
                    var _plate = _HGSService.GetByPlateNumber(model.PlateNumber);
                    if(_plate == null)
                    {
                        string LoginTCKN = Session["LoginUser"].ToString();
                        var result = _customerService.GetByTCKN(LoginTCKN);

                        _HGSService.Create(new HGS()
                        {
                            CustomerId = result.Id,
                            CustomerTCKN = result.TCKN,
                            PlateNumber = model.PlateNumber,
                            Balance = 0,
                            WebOrMobil = "Web",
                            SaleDate = DateTime.Now
                        });


                        model.SaleDate = DateTime.Now;
                        model.WebOrMobil = "Web";

                        using (var hgsSoapClient = new HGSServices.HGSServiceSoapClient())
                        {
                            confirmation = hgsSoapClient.SaleHGS(model.Balance, result.Id, result.TCKN, model.PlateNumber, model.SaleDate, model.WebOrMobil);
                        }

                        if (confirmation)
                        {
                            string message = "İşlem Başarıyla Gerçekleşti";
                            return RedirectToAction("ListSale", "HGS", message);
                        }

                        return RedirectToAction("ListSale", "HGS");
                    }
                    else
                    {
                        throw new Exception("Aynı Araç Plaka Numarasına HGS Satın Alınamaz..");
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
                return View(model);
            }
        }

        public ActionResult ListSale()
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();
                var result = _customerService.GetByTCKN(LoginTCKN);
                var listSales = _HGSService.GetAll().Where(x => x.CustomerTCKN == result.TCKN).ToList();

                return View(listSales);
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("ListSale");
            }
        }

        public ActionResult LoadBalance(int id)
        {
            var model = _HGSService.GetById(id);
            if(model== null)
            {
                throw new Exception(" Hesap Seçilmedi..");
            }
            return View("LoadBalance",model);
        }

        [HttpPost]
        public ActionResult LoadBalance(HGS model)
        {
            try
            {
                if (model.Balance > 0)
                {
                    var result = _HGSService.GetById(model.Id);
                    result.Balance = result.Balance + model.Balance;

                    _HGSService.Update(new HGS()
                    {
                        Id = result.Id,
                        CustomerId = result.CustomerId,
                        CustomerTCKN = result.CustomerTCKN,
                        Balance = result.Balance,
                        PlateNumber = result.PlateNumber,
                        WebOrMobil = "Web",
                        SaleDate = DateTime.Now
                    });

                    using (var hgsSoapClient = new HGSServices.HGSServiceSoapClient())
                    {
                         hgsSoapClient.LoadBalance(result.PlateNumber, result.Balance);
                    }

                    return RedirectToAction("ListSale", "HGS");
                }

                else
                {
                    throw new Exception("Eksik Bilgi Girildi.. Tekrar Deneyiniz..");
                }
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View(model);
            }
        }

        public ActionResult UpdateBalance(int id)
        {
            var model = _HGSService.GetById(id);
            if (model == null)
            {
                throw new Exception(" Hesap Seçilmedi..");
            }

            using (var hgsSoapClient = new HGSServices.HGSServiceSoapClient())
            {
                hgsSoapClient.UpdateBalance(model.PlateNumber);
 
            } 

            return RedirectToAction("ListSale", "HGS");
        }

    }
}