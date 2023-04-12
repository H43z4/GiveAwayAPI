using Models.DatabaseModels.PermitIssuance.Core;
using Models.ViewModels.PermitIssuance.Setup;
using Models.ViewModels.Stock;
using RepositoryLayer;
using SharedLib.Interfaces;
using System.Data;

namespace POS
{
    public interface IPOSService : ICurrentEPRSUser
    {
        Task<DataSet> SaveVendOrder(VwOrderMain orderMain);
    }

    public class POSService : IPOSService
    {
        readonly IDBHelper dbHelper;
        public VwEPRSUser VwEPRSUser { get; set; }

        public POSService(IDBHelper dbHelper)
        {
            //this.appDbContext = appDbContext;
            //this.adoNet = adoNet;
            this.dbHelper = dbHelper;
        }
        public async Task<DataSet> SaveVendOrder(VwOrderMain orderMain)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();


            paramDict.Add("@OrderId", orderMain.OrderId);
            paramDict.Add("@OrganisationId", orderMain.OrganisationId);
            paramDict.Add("@PersonId", orderMain.PersonId);
            paramDict.Add("@PermitNo", orderMain.PermitNo);
            paramDict.Add("@Tax", 0);
            paramDict.Add("@TotalQuantity", orderMain.TotalQuantity);
            paramDict.Add("@TotalAmount", orderMain.TotalAmount);
            paramDict.Add("@GrandTotal", orderMain.TotalAmount);
            paramDict.Add("@Discount", 0);
            paramDict.Add("@CreatedBy", this.VwEPRSUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[SaveVendShopOrder]", paramDict);
            string StockInApplicationId = "";
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "0")
            {
                StockInApplicationId = ds.Tables[0].Rows[0][0].ToString();
                if (StockInApplicationId != null && StockInApplicationId != "")
                {

                    var items = new List<StockInApplicationDetails>();
                    foreach (var item in orderMain.OrderDetail)
                    {
                        Dictionary<string, object> paramDict2 = new Dictionary<string, object>();
                        item.OrderId = Int64.Parse(StockInApplicationId);

                        paramDict2.Add("@OrderId", item.OrderId);
                        paramDict2.Add("@OrderDetailId", item.OrderDetailId);
                        paramDict2.Add("@ProductId", item.ProductId);
                        paramDict2.Add("@ProductPrice", item.ProductPrice);
                        paramDict2.Add("@ProductSize", item.ProductSize);
                        paramDict2.Add("@Quantity", item.Quantity);
                        paramDict2.Add("@TotalPrice", item.TotalPrice);
                        paramDict2.Add("@CreatedBy", this.VwEPRSUser.UserId);
                        this.dbHelper.GetDataSetByStoredProcedure("[Core].[SaveVendShopOrderDetails]", paramDict2);
                    }


                }
            }

            return ds;
        }
    }
}