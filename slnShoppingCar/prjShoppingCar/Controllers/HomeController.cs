using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjShoppingCar.Models;

namespace prjShoppingCar.Controllers
{
    public class HomeController : Controller
    {
        dbShoppingCarEntities1 db = new dbShoppingCarEntities1();

        public ActionResult Index()
        {
            var products = db.tProduct.ToList();

            //會員未登入時
            if (Session["Member"] == null)
            {
                return View("Index", "_Layout", products);
            }

            //會員登入時
            return View("Index", "_LayoutMember", products);
        }

        //GET Home Login
        public ActionResult Login()
        {
            return View();
        }

        //POST Home Login
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            var member = db.tMember
                            .Where(m => m.fUserId == fUserId && m.fPwd == fPwd)
                            .FirstOrDefault();

            if (member == null) 
            {
                ViewBag.Message = "帳密錯誤";
                return View();
            }

            Session["WelCome"] = member.fName + "歡迎光臨";
            Session["Member"] = member;

            return RedirectToAction("Index");
        }
    
        //GET Home Register
        public ActionResult Register()
        {
            return View();
        }

        //POST Home Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            var member = db.tMember
                            .Where(m => m.fUserId == pMember.fUserId)
                            .FirstOrDefault();

            if (member == null)
            {
                //string strConnString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                //string com_str = $"INSERT INTO [dbo].[tMember] ([fUserId], [fPwd], [fName], [fEmail]) VALUES (@fUserId, @fPwd, @fName, @fEmail);";

                //using (SqlConnection con = new SqlConnection(strConnString))
                //{
                //    SqlCommand scom = new SqlCommand(com_str, con);
                //    scom.Parameters.AddWithValue("@fUserId", pMember.fUserId);
                //    scom.Parameters.AddWithValue("@fPwd", pMember.fPwd);
                //    scom.Parameters.AddWithValue("@fName", pMember.fName);
                //    scom.Parameters.AddWithValue("@fEmail", pMember.fEmail);

                //    try
                //    {
                //        con.Open();
                //        scom.ExecuteNonQuery();
                //    }
                //    catch (Exception ex)
                //    {                        
                //        Console.WriteLine(ex.Message);
                //    }
                //    finally
                //    {
                //        con.Close();
                //    }
                //}
                db.tMember.Add(pMember);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            ViewBag.Message = "此帳號已有人使用";
            return View(0);
        }

        //GET Index Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        //GET Index ShoppingCar
        public ActionResult ShoppingCar()
        {
            string fUserId = (Session["Member"] as tMember).fUserId;
            var orderDetails = db.tOrderDetail.Where(m => m.fUserId == fUserId && m.fIsApproved == "否").ToList();
            return View("ShoppingCar", "_LayoutMember", orderDetails);
        }

        //GET Index AddCar
        public ActionResult AddCar(string fPId)
        {
            string fUserId = (Session["Member"] as tMember).fUserId;
            var currentCar = db.tOrderDetail.Where(m => m.fPId == fPId && m.fIsApproved == "否" && m.fUserId == fUserId).FirstOrDefault();

            if (currentCar == null)
            {
                var product = db.tProduct.Where(m => m.fPId == fPId).FirstOrDefault();
                tOrderDetail orderDetial = new tOrderDetail();
                orderDetial.fUserId = fUserId;
                orderDetial.fPId = product.fPId;
                orderDetial.fName = product.fName;
                orderDetial.fPrice = product.fPrice;
                orderDetial.fQty = 1;
                orderDetial.fIsApproved = "否";
                db.tOrderDetail.Add(orderDetial);
            }
            else 
            {
                currentCar.fQty += 1;
            }
            db.SaveChanges();

            return RedirectToAction("ShoppingCar");
        }

        //GET Index DeleteCar
        public ActionResult DeleteCar(int fId)
        {
            var orderDetail = db.tOrderDetail.Where(m => m.fId == fId).FirstOrDefault();
            db.tOrderDetail.Remove(orderDetail);
            db.SaveChanges();

            return RedirectToAction("ShoppingCar");
        }
    }
}