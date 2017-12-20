using System;

namespace Enterprises.Framework.WebTest.AliPay
{
    public partial class Return : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string prcode = Request.QueryString("out_trade_no");
            //var entity = new BD_BUSINESSER_PREntity() { BPR_CODE = prcode };
            //entity.InitEntity<GingerBusinessLink>("BPR_CODE");
            //WhereItemCollection whereItems = new WhereItemCollection();
            //whereItems.Add(BD_BUSINESSER_PR_DETAILEntity.RefEntity.GetBPRD_BPR_IDItem().ToWhereItem(entity.BPR_ID));
            //OrderItemCollection orderItems = new OrderItemCollection();
            //var details = RemotingManager<GingerBusinessLink>.GenericBusinessObject.SelectEntities<BD_BUSINESSER_PR_DETAILEntity>(whereItems, orderItems);
            //foreach (var item in details)
            //{
            //    item.BPRD_MT_IDDisplayValue = MD_MATERIALEntity.GetMT_CODEItemValue<GingerBasicLink>(item.BPRD_MT_ID);
            //    item.BPRD_WH_IDDisplayValue = MD_WAREHOUSEEntity.GetWH_NAMEItemValue<GingerBasicLink>(item.BPRD_WH_ID);
            //    item.BPRD_MT_UNIT_IDDisplayValue = MD_UNITEntity.GetUT_NAMEItemValue<GingerBasicLink>(item.BPRD_MT_UNIT_ID);
            //}

            //this.RepItems.DataSource = details;
            //this.RepItems.DataBind();
            //tbBPR_CODE.Text = entity.BPR_CODEDisplayValue;
            //tbBPR_DESC.Text = entity.BPR_DESCDisplayValue;
            //tbBPR_TOTAL_CASH.Text = entity.BPR_TOTAL_CASHDisplayValue;
        }
    }
}