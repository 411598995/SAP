using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Task_CurrencyUpdator
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class CURRENCIES
    {

        private CURRENCIESScript scriptField;

        private System.DateTime lAST_UPDATEField;

        private CURRENCIESCURRENCY[] cURRENCYField;

        /// <remarks/>
        public CURRENCIESScript script
        {
            get
            {
                return this.scriptField;
            }
            set
            {
                this.scriptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime LAST_UPDATE
        {
            get
            {
                return this.lAST_UPDATEField;
            }
            set
            {
                this.lAST_UPDATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CURRENCY")]
        public CURRENCIESCURRENCY[] CURRENCY
        {
            get
            {
                return this.cURRENCYField;
            }
            set
            {
                this.cURRENCYField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CURRENCIESScript
    {

        private string typeField;

        private string charsetField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string charset
        {
            get
            {
                return this.charsetField;
            }
            set
            {
                this.charsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CURRENCIESCURRENCY
    {

        private string nAMEField;

        private byte uNITField;

        private string cURRENCYCODEField;

        private string cOUNTRYField;

        private decimal rATEField;

        private decimal cHANGEField;

        /// <remarks/>
        public string NAME
        {
            get
            {
                return this.nAMEField;
            }
            set
            {
                this.nAMEField = value;
            }
        }

        /// <remarks/>
        public byte UNIT
        {
            get
            {
                return this.uNITField;
            }
            set
            {
                this.uNITField = value;
            }
        }

        /// <remarks/>
        public string CURRENCYCODE
        {
            get
            {
                return this.cURRENCYCODEField;
            }
            set
            {
                this.cURRENCYCODEField = value;
            }
        }

        /// <remarks/>
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }

        /// <remarks/>
        public decimal RATE
        {
            get
            {
                return this.rATEField;
            }
            set
            {
                this.rATEField = value;
            }
        }

        /// <remarks/>
        public decimal CHANGE
        {
            get
            {
                return this.cHANGEField;
            }
            set
            {
                this.cHANGEField = value;
            }
        }
    }


}
