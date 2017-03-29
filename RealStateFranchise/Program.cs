using RealStateFranchise.ViewModels;
using RSF.Repository.Entities;
using Starcounter;

namespace RealStateFranchise
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            RegisterHandlers();

        }

        private static void RegisterHandlers()
        {
            Handle.GET("/RealStateFranchise/master", () =>
            {
                return Db.Scope(() =>
                {
                    if (Session.Current != null && Session.Current.Data != null)
                    {
                        return Session.Current.Data;
                    }
                    if (Session.Current == null)
                    {
                        Session.Current = new Session(SessionOptions.PatchVersioning);
                    }

                    var master = new MasterPage();
                    var dashboardPage = new DashboardPage();
                    var corporateCompany = Db.SQL<Company>("Select c from Company c");

                    if (corporateCompany != null)
                    {
                        dashboardPage.Data = corporateCompany;
                    }
                    master.CurrentPage = dashboardPage;
                    master.Session = Session.Current;
                    return master;
                });
            });

            Handle.GET("/RealStateFranchise/partial/dashboard", () => new DashboardPage());

            Handle.GET("/RealStateFranchise", () =>
            {
                return Db.Scope(() =>
                {
                    var companies = Db.SQL<Company>($"select c from Company c");
                    return WrapPage<DashboardPage>("/RealStateFranchise/partial/dashboard", companies);
                });

            });

            Handle.GET("/RealStateFranchise/partial/corporation", () => new CorporationHome());
            Handle.GET("/RealStateFranchise/corporation/{?}", (string id) =>
            {
                return Db.Scope(() =>
                {
                    var company = DbHelper.FromID(DbHelper.Base64DecodeObjectID(id));
                    return WrapPage<CorporationHome>("/RealStateFranchise/partial/corporation", company);
                });
            });

            Handle.GET("/RealStateFranchise/partial/Franchise", () => new FranciseOfficeDetailsWithSale());


            Handle.GET("/RealStateFranchise/Franchise/{?}", (string id) =>
            {
                return Db.Scope(() =>
                {
                    var data = DbHelper.FromID(DbHelper.Base64DecodeObjectID(id));
                    return WrapPage<FranciseOfficeDetailsWithSale>("/RealStateFranchise/partial/Franchise", data);

                });
            });
        }

        private static Json WrapPage<T>(string partialPath, object data = null) where T : Json
        {
            var masterPage = (MasterPage)Self.GET("/RealStateFranchise/master");

            if (masterPage.CurrentPage == null || masterPage.CurrentPage.GetType() != typeof(T))
            {
                masterPage.CurrentPage = Self.GET(partialPath);
            }

            if (data != null)
            {
                masterPage.CurrentPage.Data = data;
            }
            return masterPage;
        }
    }
}