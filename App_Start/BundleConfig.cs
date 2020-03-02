using System.Web;
using System.Web.Optimization;

namespace AfpaLunch
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération à l'adresse https://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/owl.carousel").Include(
                      "~/Scripts/owl.carousel.js"));

            //bundles.Add(new ScriptBundle("~/bundles/glide").Include(
            //          "~/Scripts/glide.js",
            //          "~/Scripts/glide.min.js",
            //          "~/Scripts/glide.modular.esm.js",
            //          "~/Scripts/glide.esm.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/font-awesome.min.css",
                      //"~/Content/glide.core.css",
                      //"~/Content/glide.core.min.css",
                      //"~/Content/glide.theme.css",
                      //"~/Content/glide.theme.min.css",
                      "~/Content/owl.carousel.min.css",
                      "~/Content/owl.carousel.css",
                      "~/Content/owl.theme.default.css",
                      "~/Content/owl.theme.default.min.css",
                      "~/Content/owl.theme.green.css",
                      "~/Content/animate.css",
                      "~/Content/magic.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.structure.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/site.css"));
        }
    }
}
