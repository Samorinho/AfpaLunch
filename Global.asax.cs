using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AfpaLunch
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AfpEATEntities db = new AfpEATEntities();
            List<SessionUtilisateur> sessionUtilisateurs = db.SessionUtilisateurs.ToList();

            db.SessionUtilisateurs.RemoveRange(sessionUtilisateurs);
            db.SaveChanges();
        }

        protected void Application_End()
        {

        }

        protected void Session_Start()
        {
            try
            {
                AfpEATEntities db = new AfpEATEntities();

                SessionUtilisateur sessionUtilisateur = new SessionUtilisateur();
                sessionUtilisateur.IdSession = Session.SessionID;
                sessionUtilisateur.DateSession = DateTime.Now;

                db.SessionUtilisateurs.Add(sessionUtilisateur);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string er = ex.Message;
            }
        }

        protected void Session_End()
        {
            AfpEATEntities db = new AfpEATEntities();
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            db.SessionUtilisateurs.Remove(sessionUtilisateur);
            db.SaveChanges();
        }
    }
}
