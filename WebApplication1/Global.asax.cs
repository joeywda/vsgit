using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication1.Security;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //���g�v�����ҫe���檺�ʧ@
        //�b���Ω�]�w����(Role)
        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            //�����ШD���
            HttpRequest httpRequest = HttpContext.Current.Request;
            //�]�wJWT�K�_
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            //�]�wCookie�W��
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            //�ˬdCookie���O�_�s��TOKEN
            if (httpRequest.Cookies[cookieName] != null)
            {
                //�NTOKEN�٭�
                JwtObject JwtObject = JWT.Decode<JwtObject>(Convert.ToString(httpRequest.Cookies[cookieName].Value), Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
                //�N�ϥΪ̨����ƨ��X�A�ä��Φ��}�C
                string[] roles = JwtObject.Role.Split(new char[] { ',' });
                //�ۦ�إ� Identity ���N HttpContext.Current.User �� Identity
                //�N��ƶ�i Claim �����]�p
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name,JwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier,JwtObject.Account)
                };
                var claimsIdentity = new ClaimsIdentity(claims, cookieName);
                //�[�J identityprovider �o�� Claim �ϱo�ϥ�_�y�J @Html.AntiForgeryToken() ��q�L
                claimsIdentity.AddClaim(
new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
"My Identity", @"http://www.w3.org/2001/XMLSchema#string"));
                //���������ثe�o�� HttpContext �� User ����h
                HttpContext.Current.User = new GenericPrincipal(claimsIdentity, roles);
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }
        }
    }
}
