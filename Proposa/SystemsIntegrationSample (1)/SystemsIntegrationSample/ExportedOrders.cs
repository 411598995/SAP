using System;
using System.Collections.Generic;
using System.Text;

namespace SystemsIntegrationSample
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class DataExtract
    {

        private DataExtractOrder orderField;

        /// <remarks/>
        public DataExtractOrder Order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DataExtractOrder
    {

        private string orderIDField;

        private string ordNoField;

        private object pONoField;

        private string statIDField;

        private string statCodeField;

        private string statusField;

        private string dateField;

        private string currCodeField;

        private string currField;

        private byte noItemsField;

        private decimal itemsTotalField;

        private decimal totalField;

        private decimal amtPaidField;

        private byte shippingField;

        private decimal shippingFeeField;

        private byte discountField;

        private string emailField;

        private string delTypeField;

        private object outletCodeField;

        private object outletField;

        private object outletIDField;

        private string shipFirstNameField;

        private string shipLastNameField;

        private object shipCompanyField;

        private string shipAdd1Field;

        private object shipAdd2Field;

        private object shipAdd3Field;

        private string shipAdd4Field;

        private object shipPostCodeField;

        private string shipRegionField;

        private string shipRegionIDField;

        private object shipMobileField;

        private ulong shipPhoneDayField;

        private object shipPhoneEveField;

        private object shipEmailField;

        private object billFirstNameField;

        private object billLastNameField;

        private object billCompanyField;

        private object billAdd1Field;

        private object billAdd2Field;

        private object billAdd3Field;

        private object billAdd4Field;

        private object billPostCodeField;

        private object billRegionField;

        private object billMobileField;

        private object billPhoneDayField;

        private object billPhoneEveField;

        private string paymentTypeIDField;

        private string paymentTypeField;

        private object trackRefField;

        private object statusMsgField;

        private object giftMsgField;

        private string contIDField;

        private object custCodeField;

        private object custCodeNoField;

        private object companyField;

        private string contTypeField;

        private string isSpecialField;

        private object primCustCodeField;

        private object primCustCodeNoField;

        private object primContIDField;

        private object sUFirstNameField;

        private object sULastNameField;

        private object sUContIDField;

        private object commentsField;

        private string payProvRefField;

        private DataExtractOrderOrderItem[] orderItemField;

        /// <remarks/>
        public string OrderID
        {
            get
            {
                return this.orderIDField;
            }
            set
            {
                this.orderIDField = value;
            }
        }

        /// <remarks/>
        public string OrdNo
        {
            get
            {
                return this.ordNoField;
            }
            set
            {
                this.ordNoField = value;
            }
        }

        /// <remarks/>
        public object PONo
        {
            get
            {
                return this.pONoField;
            }
            set
            {
                this.pONoField = value;
            }
        }

        /// <remarks/>
        public string StatID
        {
            get
            {
                return this.statIDField;
            }
            set
            {
                this.statIDField = value;
            }
        }

        /// <remarks/>
        public string StatCode
        {
            get
            {
                return this.statCodeField;
            }
            set
            {
                this.statCodeField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        public string CurrCode
        {
            get
            {
                return this.currCodeField;
            }
            set
            {
                this.currCodeField = value;
            }
        }

        /// <remarks/>
        public string Curr
        {
            get
            {
                return this.currField;
            }
            set
            {
                this.currField = value;
            }
        }

        /// <remarks/>
        public byte NoItems
        {
            get
            {
                return this.noItemsField;
            }
            set
            {
                this.noItemsField = value;
            }
        }

        /// <remarks/>
        public decimal ItemsTotal
        {
            get
            {
                return this.itemsTotalField;
            }
            set
            {
                this.itemsTotalField = value;
            }
        }

        /// <remarks/>
        public decimal Total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        public decimal AmtPaid
        {
            get
            {
                return this.amtPaidField;
            }
            set
            {
                this.amtPaidField = value;
            }
        }

        /// <remarks/>
        public byte Shipping
        {
            get
            {
                return this.shippingField;
            }
            set
            {
                this.shippingField = value;
            }
        }

        /// <remarks/>
        public decimal ShippingFee
        {
            get
            {
                return this.shippingFeeField;
            }
            set
            {
                this.shippingFeeField = value;
            }
        }

        /// <remarks/>
        public byte Discount
        {
            get
            {
                return this.discountField;
            }
            set
            {
                this.discountField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public string DelType
        {
            get
            {
                return this.delTypeField;
            }
            set
            {
                this.delTypeField = value;
            }
        }

        /// <remarks/>
        public object OutletCode
        {
            get
            {
                return this.outletCodeField;
            }
            set
            {
                this.outletCodeField = value;
            }
        }

        /// <remarks/>
        public object Outlet
        {
            get
            {
                return this.outletField;
            }
            set
            {
                this.outletField = value;
            }
        }

        /// <remarks/>
        public object OutletID
        {
            get
            {
                return this.outletIDField;
            }
            set
            {
                this.outletIDField = value;
            }
        }

        /// <remarks/>
        public string ShipFirstName
        {
            get
            {
                return this.shipFirstNameField;
            }
            set
            {
                this.shipFirstNameField = value;
            }
        }

        /// <remarks/>
        public string ShipLastName
        {
            get
            {
                return this.shipLastNameField;
            }
            set
            {
                this.shipLastNameField = value;
            }
        }

        /// <remarks/>
        public object ShipCompany
        {
            get
            {
                return this.shipCompanyField;
            }
            set
            {
                this.shipCompanyField = value;
            }
        }

        /// <remarks/>
        public string ShipAdd1
        {
            get
            {
                return this.shipAdd1Field;
            }
            set
            {
                this.shipAdd1Field = value;
            }
        }

        /// <remarks/>
        public object ShipAdd2
        {
            get
            {
                return this.shipAdd2Field;
            }
            set
            {
                this.shipAdd2Field = value;
            }
        }

        /// <remarks/>
        public object ShipAdd3
        {
            get
            {
                return this.shipAdd3Field;
            }
            set
            {
                this.shipAdd3Field = value;
            }
        }

        /// <remarks/>
        public string ShipAdd4
        {
            get
            {
                return this.shipAdd4Field;
            }
            set
            {
                this.shipAdd4Field = value;
            }
        }

        /// <remarks/>
        public object ShipPostCode
        {
            get
            {
                return this.shipPostCodeField;
            }
            set
            {
                this.shipPostCodeField = value;
            }
        }

        /// <remarks/>
        public string ShipRegion
        {
            get
            {
                return this.shipRegionField;
            }
            set
            {
                this.shipRegionField = value;
            }
        }

        /// <remarks/>
        public string ShipRegionID
        {
            get
            {
                return this.shipRegionIDField;
            }
            set
            {
                this.shipRegionIDField = value;
            }
        }

        /// <remarks/>
        public object ShipMobile
        {
            get
            {
                return this.shipMobileField;
            }
            set
            {
                this.shipMobileField = value;
            }
        }

        /// <remarks/>
        public ulong ShipPhoneDay
        {
            get
            {
                return this.shipPhoneDayField;
            }
            set
            {
                this.shipPhoneDayField = value;
            }
        }

        /// <remarks/>
        public object ShipPhoneEve
        {
            get
            {
                return this.shipPhoneEveField;
            }
            set
            {
                this.shipPhoneEveField = value;
            }
        }

        /// <remarks/>
        public object ShipEmail
        {
            get
            {
                return this.shipEmailField;
            }
            set
            {
                this.shipEmailField = value;
            }
        }

        /// <remarks/>
        public object BillFirstName
        {
            get
            {
                return this.billFirstNameField;
            }
            set
            {
                this.billFirstNameField = value;
            }
        }

        /// <remarks/>
        public object BillLastName
        {
            get
            {
                return this.billLastNameField;
            }
            set
            {
                this.billLastNameField = value;
            }
        }

        /// <remarks/>
        public object BillCompany
        {
            get
            {
                return this.billCompanyField;
            }
            set
            {
                this.billCompanyField = value;
            }
        }

        /// <remarks/>
        public object BillAdd1
        {
            get
            {
                return this.billAdd1Field;
            }
            set
            {
                this.billAdd1Field = value;
            }
        }

        /// <remarks/>
        public object BillAdd2
        {
            get
            {
                return this.billAdd2Field;
            }
            set
            {
                this.billAdd2Field = value;
            }
        }

        /// <remarks/>
        public object BillAdd3
        {
            get
            {
                return this.billAdd3Field;
            }
            set
            {
                this.billAdd3Field = value;
            }
        }

        /// <remarks/>
        public object BillAdd4
        {
            get
            {
                return this.billAdd4Field;
            }
            set
            {
                this.billAdd4Field = value;
            }
        }

        /// <remarks/>
        public object BillPostCode
        {
            get
            {
                return this.billPostCodeField;
            }
            set
            {
                this.billPostCodeField = value;
            }
        }

        /// <remarks/>
        public object BillRegion
        {
            get
            {
                return this.billRegionField;
            }
            set
            {
                this.billRegionField = value;
            }
        }

        /// <remarks/>
        public object BillMobile
        {
            get
            {
                return this.billMobileField;
            }
            set
            {
                this.billMobileField = value;
            }
        }

        /// <remarks/>
        public object BillPhoneDay
        {
            get
            {
                return this.billPhoneDayField;
            }
            set
            {
                this.billPhoneDayField = value;
            }
        }

        /// <remarks/>
        public object BillPhoneEve
        {
            get
            {
                return this.billPhoneEveField;
            }
            set
            {
                this.billPhoneEveField = value;
            }
        }

        /// <remarks/>
        public string PaymentTypeID
        {
            get
            {
                return this.paymentTypeIDField;
            }
            set
            {
                this.paymentTypeIDField = value;
            }
        }

        /// <remarks/>
        public string PaymentType
        {
            get
            {
                return this.paymentTypeField;
            }
            set
            {
                this.paymentTypeField = value;
            }
        }

        /// <remarks/>
        public object TrackRef
        {
            get
            {
                return this.trackRefField;
            }
            set
            {
                this.trackRefField = value;
            }
        }

        /// <remarks/>
        public object StatusMsg
        {
            get
            {
                return this.statusMsgField;
            }
            set
            {
                this.statusMsgField = value;
            }
        }

        /// <remarks/>
        public object GiftMsg
        {
            get
            {
                return this.giftMsgField;
            }
            set
            {
                this.giftMsgField = value;
            }
        }

        /// <remarks/>
        public string ContID
        {
            get
            {
                return this.contIDField;
            }
            set
            {
                this.contIDField = value;
            }
        }

        /// <remarks/>
        public object CustCode
        {
            get
            {
                return this.custCodeField;
            }
            set
            {
                this.custCodeField = value;
            }
        }

        /// <remarks/>
        public object CustCodeNo
        {
            get
            {
                return this.custCodeNoField;
            }
            set
            {
                this.custCodeNoField = value;
            }
        }

        /// <remarks/>
        public object Company
        {
            get
            {
                return this.companyField;
            }
            set
            {
                this.companyField = value;
            }
        }

        /// <remarks/>
        public string ContType
        {
            get
            {
                return this.contTypeField;
            }
            set
            {
                this.contTypeField = value;
            }
        }

        /// <remarks/>
        public string IsSpecial
        {
            get
            {
                return this.isSpecialField;
            }
            set
            {
                this.isSpecialField = value;
            }
        }

        /// <remarks/>
        public object PrimCustCode
        {
            get
            {
                return this.primCustCodeField;
            }
            set
            {
                this.primCustCodeField = value;
            }
        }

        /// <remarks/>
        public object PrimCustCodeNo
        {
            get
            {
                return this.primCustCodeNoField;
            }
            set
            {
                this.primCustCodeNoField = value;
            }
        }

        /// <remarks/>
        public object PrimContID
        {
            get
            {
                return this.primContIDField;
            }
            set
            {
                this.primContIDField = value;
            }
        }

        /// <remarks/>
        public object SUFirstName
        {
            get
            {
                return this.sUFirstNameField;
            }
            set
            {
                this.sUFirstNameField = value;
            }
        }

        /// <remarks/>
        public object SULastName
        {
            get
            {
                return this.sULastNameField;
            }
            set
            {
                this.sULastNameField = value;
            }
        }

        /// <remarks/>
        public object SUContID
        {
            get
            {
                return this.sUContIDField;
            }
            set
            {
                this.sUContIDField = value;
            }
        }

        /// <remarks/>
        public object Comments
        {
            get
            {
                return this.commentsField;
            }
            set
            {
                this.commentsField = value;
            }
        }

        /// <remarks/>
        public string PayProvRef
        {
            get
            {
                return this.payProvRefField;
            }
            set
            {
                this.payProvRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OrderItem")]
        public DataExtractOrderOrderItem[] OrderItem
        {
            get
            {
                return this.orderItemField;
            }
            set
            {
                this.orderItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DataExtractOrderOrderItem
    {

        private string orderIDField;

        private string itemIDField;

        private string ordNoField;

        private string codeField;

        private string titleField;

        private decimal priceField;

        private byte priceDiscField;

        private byte qtyField;

        private decimal totalField;

        private decimal origPriceField;

        private object promTextField;

        private byte weightField;

        /// <remarks/>
        public string OrderID
        {
            get
            {
                return this.orderIDField;
            }
            set
            {
                this.orderIDField = value;
            }
        }

        /// <remarks/>
        public string ItemID
        {
            get
            {
                return this.itemIDField;
            }
            set
            {
                this.itemIDField = value;
            }
        }

        /// <remarks/>
        public string OrdNo
        {
            get
            {
                return this.ordNoField;
            }
            set
            {
                this.ordNoField = value;
            }
        }

        /// <remarks/>
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public decimal Price
        {
            get
            {
                return this.priceField;
            }
            set
            {
                this.priceField = value;
            }
        }

        /// <remarks/>
        public byte PriceDisc
        {
            get
            {
                return this.priceDiscField;
            }
            set
            {
                this.priceDiscField = value;
            }
        }

        /// <remarks/>
        public byte Qty
        {
            get
            {
                return this.qtyField;
            }
            set
            {
                this.qtyField = value;
            }
        }

        /// <remarks/>
        public decimal Total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        public decimal OrigPrice
        {
            get
            {
                return this.origPriceField;
            }
            set
            {
                this.origPriceField = value;
            }
        }

        /// <remarks/>
        public object PromText
        {
            get
            {
                return this.promTextField;
            }
            set
            {
                this.promTextField = value;
            }
        }

        /// <remarks/>
        public byte Weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
            }
        }
    }


}
