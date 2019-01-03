using System;
using System.Collections.Generic;

using System.Text;

namespace ABCommerce.ABCModals
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class DataExtract
    {

        private List< DataExtractOrder> orderField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Order")]
        public List< DataExtractOrder> Order
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

        private string pONoField;

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

        private string outletCodeField;

        private string outletField;

        private string outletIDField;

        private string shipFirstNameField;

        private string shipLastNameField;

        private string shipCompanyField;

        private string shipAdd1Field;

        private string shipAdd2Field;

        private string shipAdd3Field;

        private string shipAdd4Field;

        private string shipPostCodeField;

        private string shipRegionField;

        private string shipRegionIDField;

        private string shipMobileField;

        private string shipPhoneDayField;

        private string shipPhoneEveField;

        private string shipEmailField;

        private string billFirstNameField;

        private string billLastNameField;

        private string billCompanyField;

        private string billAdd1Field;

        private string billAdd2Field;

        private string billAdd3Field;

        private string billAdd4Field;

        private string billPostCodeField;

        private string billRegionField;

        private string billMobileField;

        private string billPhoneDayField;

        private string billPhoneEveField;

        private string paymentTypeIDField;

        private string paymentTypeField;

        private string trackRefField;

        private string statusMsgField;

        private string giftMsgField;

        private string contIDField;

        private string custCodeField;

        private string custCodeNoField;

        private string companyField;

        private string contTypeField;

        private string isSpecialField;

        private string primCustCodeField;

        private string primCustCodeNoField;

        private string primContIDField;

        private string sUFirstNameField;

        private string sULastNameField;

        private string sUContIDField;

        private string commentsField;

        private string payProvRefField;

        private List< DataExtractOrderOrderItem> orderItemField;

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
        public string PONo
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
        public string OutletCode
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
        public string Outlet
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
        public string OutletID
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
        public string ShipCompany
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
        public string ShipAdd2
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
        public string ShipAdd3
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
        public string ShipPostCode
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
        public string ShipMobile
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
        public string ShipPhoneDay
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
        public string ShipPhoneEve
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
        public string ShipEmail
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
        public string BillFirstName
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
        public string BillLastName
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
        public string BillCompany
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
        public string BillAdd1
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
        public string BillAdd2
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
        public string BillAdd3
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
        public string BillAdd4
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
        public string BillPostCode
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
        public string BillRegion
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
        public string BillMobile
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
        public string BillPhoneDay
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
        public string BillPhoneEve
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
        public string TrackRef
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
        public string StatusMsg
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
        public string GiftMsg
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
        public string CustCode
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
        public string CustCodeNo
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
        public string Company
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
        public string PrimCustCode
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
        public string PrimCustCodeNo
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
        public string PrimContID
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
        public string SUFirstName
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
        public string SULastName
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
        public string SUContID
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
        public string Comments
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
        public List< DataExtractOrderOrderItem> OrderItem
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

        private string promTextField;

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
        public string PromText
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
