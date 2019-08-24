using System.Web.Optimization;

namespace LFTERPsys
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //*********************************Select2Style**********************************************************

            bundles.Add(new StyleBundle("~/Content/select2css").Include(
                     "~/Content/select2.css",
                     "~/Content/select2.min.css",
                     "~/Content/SharedCss/gradients.css"
                     ));
            //*******************************************************************************************************


            //*************************************Fa-Fa-Style*******************************************************
            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                     "~/Content/font-awesome.css",
                     "~/Content/font-awesome.min.css"
                     ));
            //*******************************************************************************************************


            //*************************************BootBoxStyles&Scripts*********************************************
            bundles.Add(new ScriptBundle("~/bundles/bootbox").Include(
                     "~/Scripts/bootbox.min.js"
                     ));

            bundles.Add(new StyleBundle("~/Content/animate").Include(
                     "~/Content/animate.css"
                     ));
            //*******************************************************************************************************
            bundles.Add(new StyleBundle("~/Content/EmployeeDesign").Include(
                   "~/Content/EmployeeInfoDataCss/details-page.css",
                   "~/Content/SharedCss/gradients.css"
                   
                   ));
            //*************************************Home-Page-Style*******************************************************
            bundles.Add(new StyleBundle("~/Content/HomeStyle").Include(
                     "~/Content/HomeCss/animate.min.css",
                     "~/Content/HomeCss/magnific-popup.css",
                     "~/Content/HomeCss/ionicons.min.css",
                     "~/Content/HomeCss/style.css"
                     ));
            //*******************************************************************************************************

            //*******************************HomePageScripts*************************************************
            bundles.Add(new ScriptBundle("~/bundles/HomeScript").Include(
                "~/Scripts/HomeScripts/easing.live.js",
                "~/Scripts/HomeScripts/hoverIntent.js",
                "~/Scripts/HomeScripts/jquery-migrate.min.js",
                "~/Scripts/HomeScripts/main.js",
                "~/Scripts/HomeScripts/sticky.js",
                "~/Scripts/HomeScripts/superfish.min.js",
                "~/Scripts/HomeScripts/wow.min.js",
               "~/Scripts/SharedScripts/fullcalendar.min.js",
               "~/Scripts/HomeScripts/viewcalendar.js"
                ));
            //********************************************************************************************************

            //**************************************PublishScripts&Css***********************************************
            //--------------------------------------Calendar-Style--------------------------------------------------
            bundles.Add(new StyleBundle("~/Content/CalendarStyle").Include(
                     "~/Content/HomeCss/CalendarCss/bootstrap-datetimepicker.min.css",
                     "~/Content/HomeCss/CalendarCss/fullcalendar.min.css",
                     "~/Content/HomeCss/CalendarCss/mycalendar.css"
                     ));
            //------------------------------------------------------------------------------------------------------
            //--------------------------------------CalenderScripts-------------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/Calendar").Include(
               "~/Scripts/LofruCalendarScripts/bootstrap-datetimepicker.min.js",
               "~/Scripts/SharedScripts/fullcalendar.min.js",
               "~/Scripts/LofruCalendarScripts/mycalendar.js"
                ));
            //------------------------------------------------------------------------------------------------------
            //*******************************************************************************************************

            //--------------------------------------Paging & Searching Scripts-------------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/Paging").Include(
               "~/Scripts/SharedScripts/jquery.dataTables.min.js",
               "~/Scripts/SharedScripts/dataTables.bootstrap4.min.js"
                ));
            //------------------------------------------------------------------------------------------------------

            //*****************************************EosDataScripts************************************************

            //--------------------------------MyEosScripts-----------------------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/MyEos").Include(
                    "~/Scripts/EosDataScripts/MyEos/workpercent.colorscheme.js",
                    "~/Scripts/EosDataScripts/MyEos/mvc.operations.js",
                    "~/Scripts/EosDataScripts/Shared/workpercent.validator.js"
                    ));
            //-------------------------------------------------------------------------------------------------------
            //--------------------------------EmployeeEosScripts-----------------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/EmployeeEos").Include(
                    "~/Scripts/EosDataScripts/EmployeeEos/mvc.operations.js",
                    "~/Scripts/EosDataScripts/Shared/workpercent.validator.js"
                    ));
            //-------------------------------------------------------------------------------------------------------

            //********************************************************************************************************

            //*******************************RoleDataScripts**********************************************************
            bundles.Add(new ScriptBundle("~/bundles/RoleData").Include(
                "~/Scripts/bootstrap-multiselect.js",
                "~/Scripts/RoleDataScripts/Shared/multipleselection.js"
                ));
            //********************************************************************************************************

            //*******************************EmployeeInfoDataScripts*************************************************
            bundles.Add(new ScriptBundle("~/bundles/EmployeeInfoData").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/SharedScripts/search.live.js",
                "~/Scripts/SharedScripts/upload.image.js"
                ));
            //********************************************************************************************************

        }
    }
}
